using CodeKing.SqlHarvester.Core;
using CodeKing.SqlHarvester.Core.Data;
using CodeKing.SqlHarvester.Data;

namespace CodeKing.SqlHarvester
{
    internal class SqlScripterFactory
    {
        #region Constants and Fields

        private readonly IDataCommand database;

        #endregion

        #region Constructors and Destructors

        public SqlScripterFactory(IDataCommand database)
        {
            this.database = database;
        }

        #endregion

        #region Public Methods

        public virtual ISqlScripter CreateInstance(ScriptInfo info)
        {
            return new SqlScripter(database, info);
        }

        #endregion
    }
}
