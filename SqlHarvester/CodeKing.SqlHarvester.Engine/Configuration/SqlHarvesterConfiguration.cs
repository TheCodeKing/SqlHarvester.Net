using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using System.Configuration;
using CodeKing.SqlHarvester.Configuration;
using System.Diagnostics;

namespace CodeKing.SqlHarvester.Configuration
{
    public class SqlHarvesterConfiguration : HarvestConfigurationSection
    {
        private static SqlHarvesterConfiguration defaultInstance;
        private static object objectLock_Default = new object();

        /// <summary>
        /// Gets or sets the default instance of the SqlHarvesterConfiguration class.
        /// </summary>
        /// <value>The default.</value>
        public static SqlHarvesterConfiguration Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    lock (objectLock_Default)
                    {
                        if (defaultInstance == null)
                        {
                            defaultInstance = (SqlHarvesterConfiguration)ConfigurationManager.GetSection("SqlHarvesterConfiguration");

                            if (defaultInstance == null)
                            {
                                defaultInstance = new SqlHarvesterConfiguration();
                            }
                        }
                    }
                }

                return defaultInstance;
            }
            set
            {
                defaultInstance = value;
            }
        }

        internal bool Usage
        {
            get
            {
                return this.ContainsKey("help");
            }
        }

        /// <summary>
        /// Gets the output directory.
        /// </summary>
        /// <value>The output directory.</value>
        [ConfigurationProperty("outputDirectory", DefaultValue="Scripts")]
        public string OutputDirectory
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (base["outputDirectory"] as string));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running in verbose mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is verbose; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerbose
        {
            get
            {
                return !string.IsNullOrEmpty(verbose);
            }
        }

        public TraceLevel VerboseLevel
        {
            get
            {
                int level;
                if (int.TryParse(verbose, out level))
                {
                    if (level <= 4 && level>=0)
                    {
                        return (TraceLevel)level;
                    }
                }
                return Tracer.Trace.Level;
            }
        }

        [ConfigurationProperty("verbose", DefaultValue = null)]
        private string verbose
        {
            get
            {
                return base["verbose"];
            }
            set
            {
                base["verbose"] = value;
            }
        }

        /// <summary>
        /// Gets the execution mode.
        /// </summary>
        /// <value>The execution mode.</value>
        public Mode Mode
        {
            get
            {
                if (this.ContainsKey("export"))
                {
                    return Mode.Export;
                }
                else if (this.ContainsKey("import"))
                {
                    return Mode.Import;
                }
                else
                {
                    return Mode.NotSet;
                }
            }
        }

        /// <summary>
        /// Gets the Sql scripting mode.
        /// </summary>
        /// <value>The script mode.</value>
        [ConfigurationProperty("defaultScriptMode", DefaultValue = ScriptMode.Delete)]
        public ScriptMode DefaultScriptMode
        {
            get
            {
                return (ScriptMode)this.GetConfigurationValue("defaultScriptMode");
            }
            set
            {
                this.SetConfigurationValue("defaultScriptMode", value);
            }
        }

        /// <summary>
        /// Gets or sets the script info collection.
        /// </summary>
        /// <value>The script info collection.</value>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ScriptInfoCollection ScriptInfoCollection
        {
            get
            {
                return (ScriptInfoCollection)GetConfigurationValue("");
            }
            set
            {
                SetConfigurationValue("", value);
            }
        }

        /// <summary>
        /// Gets or sets the filename start sequence.
        /// </summary>
        /// <value>The filename start sequence.</value>
        public int FilenameStartSequence
        {
            get
            {
                int num = 0;
                int.TryParse(filenameStartSequence, out num);
                return num;
            }
            set
            {
                filenameStartSequence = Convert.ToString(value);
            }
        }

        [ConfigurationProperty("filenameStartSequence", DefaultValue = "100000", IsRequired = false)]
        private string filenameStartSequence
        {
            get
            {
                return base["filenameStartSequence"] as string;
            }
            set
            {
                base["filenameStartSequence"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string for accessing the database.
        /// </summary>
        /// <value>The connection string.</value>
        [ConfigurationProperty("connectionString", DefaultValue = null, IsRequired = false)]
        public string ConnectionString
        {
            get
            {
                return GetConfigurationValue("connectionString") as string;
            }
        }
    }
}
