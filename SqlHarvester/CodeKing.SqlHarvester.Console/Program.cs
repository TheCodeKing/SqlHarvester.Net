using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CodeKing.SqlHarvester.Configuration;
using System.Configuration;
using System.Diagnostics;

namespace CodeKing.SqlHarvester
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                SqlHarvesterConfiguration config = SqlHarvesterConfiguration.Default;
                using (HarvestService service = new HarvestService(config))
                {
                    service.ImportConfiguration(args);
                    if (config.IsVerbose)
                    {
                        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                        Tracer.Trace.Level = config.VerboseLevel;
                    }
                    if (config.Usage)
                    {
                        WriteUsage();
                        return 0;
                    } 
                    else if (config.Mode == Mode.Export)
                    {
                        Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Begin scripting...");
                        FileInfo[] files = service.Export();
                        Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Scripting success...");
                        return 0;
                    }
                    else if (config.Mode == Mode.Import)
                    {
                        Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Begin seeding...");
                        if (service.Import())
                        {
                            Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Seeding seeding...");
                            return 0;
                        }
                        else
                        {
                            return 5;
                        }
                    }
                    else
                    {
                        return 4;
                    }
                }
            }
            catch (ConfigurationErrorsException ce)
            {
                Trace.WriteLineIf(Tracer.Trace.TraceError, ce.Message);
                return 2;
            }
            catch (Exception e)
            {
                Trace.WriteLineIf(Tracer.Trace.TraceError, e.Message);
                return 3;
            }
        }

        private static void WriteUsage()
        {
            Console.WriteLine("Imports or exports database content");
            Console.WriteLine();
            Console.WriteLine("SqlHarvester [-export|-import] [<option>]");
            Console.WriteLine();
            Console.WriteLine("   -export \tExports content from the target database.");
            Console.WriteLine("   -import \tImports content into the target database.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -connectionString:<connectionString> ");
            Console.WriteLine("    The connection-string used to connect to the target database.");
            Console.WriteLine();
            Console.WriteLine("   -tables[:tableExpression1,[:tableExpression2], ...] ");
            Console.WriteLine("    Defines one or more tables from which to script content.");
            Console.WriteLine();
            Console.WriteLine("   -defaultScriptMode:[NotSet|Delete|NoDelete]");
            Console.WriteLine("    Specifies if content is scripted with deletes by default.");
            Console.WriteLine();
            Console.WriteLine("   -outputDirectory:<filePath>");
            Console.WriteLine("    The location of script files for scripting or seeding.");
            Console.WriteLine();
            Console.WriteLine("   -verbose:<level>");
            Console.WriteLine("    Specifies the output verbose level (0-4).");
            Console.WriteLine();
        }
    }
}
