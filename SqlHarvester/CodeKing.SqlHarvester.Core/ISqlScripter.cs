using System;
using System.Data;

namespace CodeKing.SqlHarvester.Core
{
    public interface ISqlScripter : IDisposable
    {
        #region Properties

        DataColumnCollection Columns { get; }

        bool HasNonPrimaryKey { get; }

        bool HasPrimaryKey { get; }

        bool HasSeedIdentityKey { get; }

        #endregion

        #region Public Methods

        IDataReader ExecuteForContent(string query);

        string GetParameterName(DataColumn column);

        string GetSqlDataType(DataColumn column);

        string GetTest(DataColumn column);

        bool IsActive(DataColumn column);

        bool IsPrimaryKey(DataColumn column);

        #endregion
    }
}
