/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
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
