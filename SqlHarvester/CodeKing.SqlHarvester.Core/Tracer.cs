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
using System.Diagnostics;

namespace CodeKing.SqlHarvester.Core
{
    internal static class Tracer
    {
        #region Constants and Fields

        public static TraceSwitch Trace = new TraceSwitch("SqlHarvester", "SqlHarvester trace switch");

        #endregion
    }
}
