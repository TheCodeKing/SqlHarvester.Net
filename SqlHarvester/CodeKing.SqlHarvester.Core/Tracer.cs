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
