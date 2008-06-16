using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CodeKing.SqlHarvester.Data;

namespace CodeKing.SqlHarvester
{
    internal class SqlScripterFactory
    {
        private IDataCommand database;

        public SqlScripterFactory(IDataCommand database)
        {
            this.database = database;
        }

        public virtual ISqlScripter CreateInstance(ScriptInfo info)
        {
            return new SqlScripter(database, info);
        }
    }
}
