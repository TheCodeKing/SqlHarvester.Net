using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using CodeKing.SqlHarvester.Properties;
using System.Text.RegularExpressions;
using System.Data;
using CodeKing.SqlHarvester.Data;

namespace CodeKing.SqlHarvester
{
    internal class SqlSeeder : ISeeder
    {
        private StreamReader reader;
        private string outputDirectory;
        private IDataCommand database;

        public SqlSeeder(IDataCommand database, string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
            this.database = database;
        }

        public string[] GetFiles()
        {
            string[] files = Directory.GetFiles(outputDirectory, "*.sql", SearchOption.TopDirectoryOnly);
            Array.Sort(files, StringComparer.InvariantCultureIgnoreCase);
            return files;
        }

        public void ImportFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (!file.Exists)
            {
                throw new ApplicationException(string.Format("file {0} does not exist", Path.GetFileName(fileName)));
            }
            using (FileStream strm = file.OpenRead())
            {
                using (StreamReader reader = new StreamReader(strm))
                {
                    StringBuilder builder = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        string query = reader.ReadLine();
                        if (query == "GO")
                        {
                            ExecuteSql(builder.ToString());
                            builder = new StringBuilder();
                        }
                        else
                        {
                            builder.Append(Environment.NewLine);
                            builder.Append(query);
                        }
                    }
                    if (builder != null && builder.Length > 0)
                    {
                        ExecuteSql(builder.ToString());
                        builder = new StringBuilder();
                    }
                }
            }
        }

        private void ExecuteSql(string query)
        {
            if (IsValidQuery(query))
            {
                using (IDbCommand cmd = database.CreateCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    database.ExecuteNonQuery(cmd);
                }
            }
        }

        /// <summary>
        /// Determines whether to execute the sql against the database. This requires the command 
        /// is more than simply a "GO" command.
        /// </summary>
        /// <param name="query">The sql query.</param>
        /// <returns></returns>
        private bool IsValidQuery(string query)
        {
            query = query.Trim();
            if (query.Length < 2)
            {
                return false;
            }
            return true;

        }

        public void Dispose()
        {
            if (database != null)
            {
                database.Rollback();
                database = null;
            }
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
        }
    }
}
