using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace CodeKing.SqlHarvester
{
    public interface ISqlScripter : IDisposable
    {
        bool HasSeedIdentityKey
        {
            get;
        }

        bool HasPrimaryKey
        {
            get;
        }

        bool HasNonPrimaryKey
        {
            get;
        }

        DataColumnCollection Columns
        {
            get;
        }

        string GetParameterName(DataColumn column);

        string GetSqlDataType(DataColumn column);

        bool IsActive(DataColumn column);

        bool IsPrimaryKey(DataColumn column);

        string GetTest(DataColumn column);

        IDataReader ExecuteForContent(string query);
   }
}
