using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using CodeKing.SqlHarvester.Properties;
using System.Text.RegularExpressions;
using System.Data;

namespace CodeKing.SqlHarvester
{
    internal class SqlHarvester : IHarvester
    {
        private ISqlScripter sqlScripter;
        private ScriptInfo scriptInfo;
        private StreamWriter writer;
        private FileInfo file;
        private int batchSize = 20;
        private int counter = 0;
        private int index;

        public SqlHarvester(SqlScripterFactory factory, ScriptInfo scriptInfo, string outputDirectory, int index)
        {
            if (scriptInfo == null)
            {
                throw new ArgumentException("scriptInfo");
            }
            if (factory == null)
            {
                throw new ArgumentException("factory");
            }
            this.index = index;
            this.sqlScripter = factory.CreateInstance(scriptInfo);
            this.scriptInfo = scriptInfo;
            this.file = GetFile(outputDirectory);
            Stream stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
            this.writer = new StreamWriter(stream);
        }

        private FileInfo GetFile(string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            return new FileInfo(Path.Combine(outputDirectory, string.Concat(index,".", CleanName(scriptInfo.Name), ".sql")));
        }

        private string CleanName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(Convert.ToString(c), string.Empty);
            }
            return name;
        }

        public FileInfo WriteHeader()
        {
            writer.WriteLine(string.Format("/* Scripting table {0} **/", scriptInfo.QualifiedName));
            writer.WriteLine();

            writer.WriteLine(Resources.on_count);
            writer.WriteLine("GO");
            writer.WriteLine();

            WriteConstraints(true);

            if (scriptInfo.ScriptMode == ScriptMode.Delete)
            {
                string queryString;
                if (string.IsNullOrEmpty(scriptInfo.Filter))
                {
                    queryString = string.Format("DELETE FROM {0}", scriptInfo.QualifiedName);
                }
                else
                {
                    queryString = string.Format("DELETE FROM {0} WHERE {1}", scriptInfo.QualifiedName, scriptInfo.Filter);
                }
                writer.WriteLine(queryString);
                writer.WriteLine("GO");
                writer.WriteLine();
            }
  
            string proc;
            if (sqlScripter.HasPrimaryKey && sqlScripter.HasNonPrimaryKey)
            {
                proc = Resources.insert_update;
            }
            else if (sqlScripter.HasPrimaryKey)
            {
                proc = Resources.checked_insert;
            }
            else
            {
                proc = Resources.insert;
            }
            writer.WriteLine(ReplaceTokens(proc));

            return file;
        }

        private void WriteConstraints(bool drop)
        {
            string constraints = Resources.constraints;
            constraints = Regex.Replace(constraints, "{check}", drop?"NOCHECK":"CHECK", RegexOptions.IgnoreCase);
            constraints = Regex.Replace(constraints, "{tablename}", scriptInfo.Name, RegexOptions.IgnoreCase);
            writer.WriteLine(constraints);
            writer.WriteLine();
        }

        public bool WriteContent()
        {
            if (sqlScripter.HasSeedIdentityKey)
            {
                writer.WriteLine(ReplaceTokens(Resources.identity_on));
                writer.WriteLine("GO");
                writer.WriteLine();
            }

            string queryString;
            if (string.IsNullOrEmpty(scriptInfo.Filter))
            {
                queryString = string.Format("SELECT * FROM {0}", scriptInfo.QualifiedName);
            }
            else
            {
                queryString = string.Format("SELECT * FROM {0} WHERE {1}", scriptInfo.QualifiedName, scriptInfo.Filter);
            }

            bool hasData = false;
            using (IDataReader reader = sqlScripter.ExecuteForContent(queryString))
            {
                while (reader.Read())
                {
                    hasData = true;
                    WriteDataContent(reader);
                }
                writer.WriteLine(string.Concat("GO"));
            }

            writer.WriteLine();
            if (sqlScripter.HasSeedIdentityKey)
            {
                writer.WriteLine(ReplaceTokens(Resources.identity_off));
                writer.WriteLine("GO");
            }
            writer.WriteLine();
            return hasData;
        }

        public void WriteFooter()
        {
            writer.WriteLine(ReplaceTokens(Resources.footer));
            writer.WriteLine();
            WriteConstraints(false);
        }

        private string ReplaceTokens(string input)
        {
            input = Regex.Replace(input, "{tablename}", scriptInfo.QualifiedName, RegexOptions.IgnoreCase);
            List<string> columnNames = new List<string>();
            List<string> columns = new List<string>();
            List<string> setClause = new List<string>();
            List<string> whereClause = new List<string>();
            List<string> insertParams = new List<string>();
            foreach (DataColumn item in sqlScripter.Columns)
            {
                if (sqlScripter.IsActive(item))
                {
                    string param = string.Format("{0} {1}", sqlScripter.GetParameterName(item), sqlScripter.GetSqlDataType(item));
                    columns.Add(param);
                    if (sqlScripter.IsPrimaryKey(item))
                    {
                        param = sqlScripter.GetTest(item);
                        whereClause.Add(param);
                        
                    }
                    else
                    {
                        param = string.Format("[{0}]={1}", item, sqlScripter.GetParameterName(item));
                        setClause.Add(param);
                    }
                    insertParams.Add(sqlScripter.GetParameterName(item));
                    columnNames.Add(string.Concat("[", item.ColumnName, "]"));
                }
            }
            input = Regex.Replace(input, "{columns}", string.Concat("\r\n\t\t ", string.Join("\r\n\t\t,", columnNames.ToArray())), RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "{parameters}", string.Concat("\t ", string.Join("\r\n\t,", columns.ToArray())), RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "{setclause}", string.Concat(" ",string.Join("\r\n\t\t\t\t,", setClause.ToArray())), RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "{whereclause}", string.Concat(" ", string.Join("\r\n\t\t\t\t AND ", whereClause.ToArray())), RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "{values}", string.Concat("\r\n\t\t ", string.Join("\r\n\t\t,", insertParams.ToArray())), RegexOptions.IgnoreCase);
 
            return input;
        }

        public void Cancel()
        {
            writer.Close();
            writer = null;
            if (file.Exists)
            {
                file.Delete();
            }
        }

        private void WriteDataContent(IDataReader reader)
        {
            counter++;
            if (counter % batchSize == 0 && counter > 0)
            {
                writer.WriteLine("GO");
            }
            writer.Write(Resources.item);
            writer.Write(" ");
            WriteDataContentValues(reader);
            writer.WriteLine();
        }

        private void WriteDataContentValues(IDataReader reader)
        {
            string comma = "";
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i).Trim();
                DataColumn column = sqlScripter.Columns[name];
                if (sqlScripter.IsActive(column))
                {
                    writer.Write(comma);
                    WriteDataValue(reader.GetFieldType(i), reader, column, i);
                    comma = ", ";
                }
            }
        }

        private void WriteDataValue(Type type, IDataReader reader, DataColumn column, int index)
        {
            if (reader.IsDBNull(index))
            {
                writer.Write(sqlScripter.GetParameterName(column));
                writer.Write("=NULL");
            }
            else
            {
                writer.Write(sqlScripter.GetParameterName(column));
                writer.Write("=");
                if (type == typeof(Byte[]))
                {
                    writer.Write((string.Concat("0x", BitConverter.ToString((Byte[])reader[index]).Replace("-", string.Empty))));
                }
                else if (type == typeof(DateTime))
                {
                    DateTime date = Convert.ToDateTime(reader[index]);
                    string dateStr = date.ToString("dd MMM yyyy HH:mm:ss:fff");
                    writer.Write(string.Concat("'", dateStr, "'"));
                }
                else
                {
                    writer.Write(string.Concat("N'", Convert.ToString(reader[index]).Replace("'", "''"), "'"));
                }
            }
        }

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }
    }
}
