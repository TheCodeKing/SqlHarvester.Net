using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CodeKing.SqlHarvester.Data;
using System.Data;
using System.Data.SqlClient;

namespace CodeKing.SqlHarvester
{
    internal class SqlScripter : ISqlScripter
    {
        private IDataCommand database;
        private ScriptInfo scriptInfo;
        private DataSet tableInfoData;
        private bool? hasSeedIdentityKey;

        public bool HasNonPrimaryKey
        {
            get
            {
                return (Columns.Count>GetPrimaryKeys().Length);
            }
        }

        public bool HasPrimaryKey
        {
            get
            {
                return GetPrimaryKeys().Length>0;
            }
        }

        public bool HasSeedIdentityKey
        {
            get
            {
                if (hasSeedIdentityKey == null)
                {
                    DataTable table = tableInfoData.Tables["ColumnTypeData"];
                    hasSeedIdentityKey = false;
                    foreach (DataRow data in table.Rows)
                    {
                        if (((bool)data["IsIdentity"]))
                        {
                            hasSeedIdentityKey = true;
                            break;
                        }
                    }
                }
                return (bool)hasSeedIdentityKey;
            }
        }

        public DataColumnCollection Columns
        {
            get
            {
                return tableInfoData.Tables["TableSchema"].Columns;
            }
        }

        public string GetParameterName(DataColumn column)
        {
            return string.Concat("@", column.ColumnName.Replace(" ","_").Trim()); ;
        }

        public string GetSqlDataType(DataColumn column)
        {
            DataRow row = FindRow(column);
            if (row!=null)
            {
                string type = Convert.ToString(row["ColumnType"]).Trim();
                string prec = Convert.ToString(row["Precision"]).Trim();
                string length = Convert.ToString(row["Max_Length"]).Trim();
                if (length == "-1")
                {
                    length = "max";
                }
                length = ParseLength(type, length);
                string scale = Convert.ToString(row["Scale"]).Trim();
                bool hasPrec = (string.IsNullOrEmpty(prec) && prec != "0");
                switch (type)
                {
                    case "int":
                    case "bigint":
                    case "uniqueidentifier":
                    case "tinyint":
                    case "image":
                    case "text":
                    case "ntext":
                    case "datetime":
                    case "bit":
                    case "real":
                    case "money":
                    case "smalldatetime":
                    case "smallint":
                    case "smallmoney":
                    case "sql_variant":
                    case "timestamp":
                    case "xml":
                        return type;
                    default:
                        if (!hasPrec)
                        {
                            return string.Concat(type, "(", length, ")");
                        }
                        else
                        {
                            return string.Concat(type, "(", prec, ",", scale, ")");
                        }
                }
            }
            return "";
        }

        private string[] GetPrimaryKeys()
        {
            List<string> keys = new List<string>();
            DataColumn[] columns = tableInfoData.Tables["TableSchema"].PrimaryKey;
            foreach (DataColumn col in columns)
            {
                keys.Add(col.ColumnName);
            }
            return keys.ToArray();
        }

        private DataRow FindRow(DataColumn column)
        {
            DataRow[] rows = tableInfoData.Tables["ColumnTypeData"].Select(string.Format("ColumnName='{0}'", column.ColumnName));
            if (rows.Length == 1)
            {
                return rows[0];
            }
            else
            {
                return null;
            }
        }

        public bool IsPrimaryKey(DataColumn column)
        {
            foreach (DataColumn item in column.Table.PrimaryKey)
            {
                if (item == column)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsActive(DataColumn column)
        {
            DataRow row = FindRow(column);
            if (row!=null)
            {
                return !((bool)row["IsComputed"]) && ((string)row["ColumnType"]).ToLowerInvariant() != "timestamp"; 
            }
            else
            {
                return false;
            }
        }

        public string GetTest(DataColumn column)
        {
            if (!column.AllowDBNull)
            {
                return string.Format("[{0}]={1}", column, GetParameterName(column));
            }
            else
            {
                return string.Format("[{0}]={1}", column, GetParameterName(column));
            }


        }

        public SqlScripter(IDataCommand database, ScriptInfo scriptInfo)
        {
            this.database = database;
            this.scriptInfo = scriptInfo;
            LoadSchema();
        }

        private void LoadSchema()
        {
            this.tableInfoData = new DataSet();

            DataTable tableSchema = new DataTable("TableSchema");
            this.tableInfoData.Tables.Add(tableSchema);

            using (SqlDataAdapter adapter = new SqlDataAdapter(string.Format("Select * From {0}", scriptInfo.QualifiedName), database.ConnectionString))
            {
                adapter.FillSchema(tableSchema, SchemaType.Source);
            }

            DataTable columnTypeData = new DataTable("ColumnTypeData");
            this.tableInfoData.Tables.Add(columnTypeData);

            string query = string.Format("select c.Name as ColumnName, c.Is_Identity as IsIdentity, c.Is_Computed IsComputed, t.Name as ColumnType, c.Max_Length, c.Scale, c.Precision From sys.columns c inner join sys.types t on c.user_type_id = t.user_type_id Where Object_Name(c.object_id) = '{0}'", scriptInfo.Name);
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, database.ConnectionString))
            {
                adapter.Fill(columnTypeData);
            }
        }

        private string ParseLength(string type, string length)
        {
            if (type.StartsWith("n"))
            {
                int l = 0;
                if (int.TryParse(length, out l))
                {
                    l = l / 2;
                    return l.ToString();
                }
            }
            return length;
        }

        public IDataReader ExecuteForContent(string query)
        {
            IDbCommand cmd = database.CreateCommand(query);
            cmd.CommandType = CommandType.Text;
            return database.ExecuteReader(cmd);
        }

        public void Dispose()
        {
        }
   }
}
