using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CodeKing.SqlHarvester
{
    internal static class Tracer
    {
        public static TraceSwitch Trace = new TraceSwitch("SqlHarvester", "SqlHarvester trace switch");
    }
}
