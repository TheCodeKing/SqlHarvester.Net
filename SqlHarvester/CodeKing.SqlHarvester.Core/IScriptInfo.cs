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
namespace CodeKing.SqlHarvester.Core
{
    public interface IScriptInfo
    {
        #region Properties

        string Filter { get; set; }

        string Name { get; set; }

        string QualifiedName { get; }

        ScriptMode ScriptMode { get; set; }

        #endregion
    }
}
