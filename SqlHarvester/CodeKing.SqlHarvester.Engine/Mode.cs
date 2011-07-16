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
namespace CodeKing.SqlHarvester
{
    /// <summary>
    /// Defines the execution mode.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// The mode is undefined.
        /// </summary>
        NotSet,
        /// <summary>
        /// Seeds content into a target database.
        /// </summary>
        Import,
        /// <summary>
        /// Scripts content from source database.
        /// </summary>
        Export
    }
}
