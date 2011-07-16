using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

using CodeKing.SqlHarvester.Core;
using CodeKing.SqlHarvester.Core.Configuration;

namespace CodeKing.SqlHarvester.Configuration
{
    public class SqlHarvesterConfiguration : HarvestConfigurationSection
    {
        #region Constants and Fields

        private static readonly object objectLock_Default = new object();

        private static SqlHarvesterConfiguration defaultInstance;

        #endregion

        #region Properties

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
                            defaultInstance =
                                (SqlHarvesterConfiguration)ConfigurationManager.GetSection("SqlHarvesterConfiguration");

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

        /// <summary>
        /// Gets the Sql scripting mode.
        /// </summary>
        /// <value>The script mode.</value>
        [ConfigurationProperty("defaultScriptMode", DefaultValue = ScriptMode.Delete)]
        public ScriptMode DefaultScriptMode
        {
            get
            {
                return (ScriptMode)GetConfigurationValue("defaultScriptMode");
            }
            set
            {
                SetConfigurationValue("defaultScriptMode", value);
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

        /// <summary>
        /// Gets the execution mode.
        /// </summary>
        /// <value>The execution mode.</value>
        public Mode Mode
        {
            get
            {
                if (ContainsKey("export"))
                {
                    return Mode.Export;
                }
                else if (ContainsKey("import"))
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
        /// Gets the output directory.
        /// </summary>
        /// <value>The output directory.</value>
        [ConfigurationProperty("outputDirectory", DefaultValue = "Scripts")]
        public string OutputDirectory
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (base["outputDirectory"]));
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

        public TraceLevel VerboseLevel
        {
            get
            {
                int level;
                if (int.TryParse(verbose, out level))
                {
                    if (level <= 4 && level >= 0)
                    {
                        return (TraceLevel)level;
                    }
                }
                return Tracer.Trace.Level;
            }
        }

        internal bool Usage
        {
            get
            {
                return ContainsKey("help");
            }
        }

        [ConfigurationProperty("filenameStartSequence", DefaultValue = "100000", IsRequired = false)]
        private string filenameStartSequence
        {
            get
            {
                return base["filenameStartSequence"];
            }
            set
            {
                base["filenameStartSequence"] = value;
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

        #endregion
    }
}
