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
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

using CodeKing.SqlHarvester.Configuration;
using CodeKing.SqlHarvester.Core;
using CodeKing.SqlHarvester.Core.Data;
using CodeKing.SqlHarvester.Data;

namespace CodeKing.SqlHarvester
{
    internal class HarvestService : IDisposable
    {
        #region Constants and Fields

        private readonly IDataCommand database;

        private readonly SqlHarvesterConfiguration sqlConfiguration;

        private int index = 1000;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HarvestService"/> class.
        /// </summary>
        public HarvestService()
            : this(SqlHarvesterConfiguration.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarvestService"/> class.
        /// </summary>
        /// <param name="sqlConfiguration">The SQL configuration.</param>
        public HarvestService(SqlHarvesterConfiguration sqlConfiguration)
        {
            this.sqlConfiguration = sqlConfiguration;
            database = new Database(sqlConfiguration.ConnectionString);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the harvestor.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        public virtual IHarvester CreateHarvestor(ScriptInfo info)
        {
            index += 5;
            return new SqlHarvester(new SqlScripterFactory(database), info, sqlConfiguration.OutputDirectory, index);
        }

        /// <summary>
        /// Creates the harvestor.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        public virtual ISeeder CreateSeeder()
        {
            return new SqlSeeder(database, sqlConfiguration.OutputDirectory);
        }

        /// <summary>
        /// Exports the content using the default configuration settings.
        /// </summary>
        /// <returns></returns>
        public FileInfo[] Export()
        {
            return Export(sqlConfiguration.ScriptInfoCollection);
        }

        /// <summary>
        /// Exports the specified script info.
        /// </summary>
        /// <param name="scriptInfo">The script info.</param>
        /// <returns></returns>
        public FileInfo[] Export(ScriptInfoCollection scriptInfo)
        {
            List<FileInfo> files = new List<FileInfo>();
            scriptInfo = ParseCollection(scriptInfo);
            foreach (ScriptInfo info in scriptInfo)
            {
                if (info.ScriptMode == ScriptMode.NotSet)
                {
                    info.ScriptMode = sqlConfiguration.DefaultScriptMode;
                }
                using (IHarvester harvester = CreateHarvestor(info))
                {
                    FileInfo file = harvester.WriteHeader();
                    Trace.WriteLineIf(Tracer.Trace.TraceInfo, string.Format(" * {0}", file.Name));
                    if (harvester.WriteContent())
                    {
                        harvester.WriteFooter();
                        files.Add(file);
                    }
                    else
                    {
                        // no content, abort file
                        Trace.WriteLineIf(
                            Tracer.Trace.TraceVerbose, string.Format("Abort table {0} as no content", info.Name));
                        harvester.Cancel();
                    }
                }
            }
            return files.ToArray();
        }

        /// <summary>
        /// Imports scripts into a target database.
        /// </summary>
        /// <returns></returns>
        public bool Import()
        {
            using (ISeeder seeder = CreateSeeder())
            {
                database.BeginTransaction();
                string[] files = seeder.GetFiles();
                foreach (string file in files)
                {
                    Trace.WriteLineIf(Tracer.Trace.TraceInfo, string.Format(" * {0}", Path.GetFileName(file)));
                    seeder.ImportFile(file);
                }
                database.Commit();
            }
            return true;
        }

        /// <summary>
        /// Imports the configuration.
        /// </summary>
        /// <param name="args">The args.</param>
        public void ImportConfiguration(string[] args)
        {
            foreach (string arg in args)
            {
                string key, value;
                key = arg.TrimStart(new[] { '-', '/' });
                if (key.StartsWith("tables:"))
                {
                    if (sqlConfiguration is SqlHarvesterConfiguration)
                    {
                        SqlHarvesterConfiguration sqlConfig = sqlConfiguration;
                        value = key.Split(new[] { ':' }, 2)[1];
                        sqlConfig.ScriptInfoCollection.Clear();
                        CreateTablesStack(value, sqlConfig);
                    }
                }
                else
                {
                    if (key.Contains(":"))
                    {
                        string[] props = key.Split(new[] { ':' }, 2);
                        key = props[0].Trim().TrimStart('"').TrimEnd('\"');
                        value = props[1].Trim().TrimStart('"').TrimEnd('\"');
                    }
                    else
                    {
                        value = "true";
                    }
                    if (key == "?")
                    {
                        key = "help";
                    }
                    sqlConfiguration[key] = value;
                }
            }
            database.ConnectionString = sqlConfiguration.ConnectionString;
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            if (database != null)
            {
                database.Rollback();
            }
        }

        #endregion

        #endregion

        #region Methods

        private void CreateTablesStack(string value, SqlHarvesterConfiguration configuration)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Tables data is undefined");
            }
            string[] rawtable = value.Split(':');
            foreach (string tableData in rawtable)
            {
                configuration.ScriptInfoCollection.Add(new ScriptInfo(tableData));
            }
        }

        private ScriptInfoCollection ParseCollection(ScriptInfoCollection config)
        {
            if (config.Contains("*"))
            {
                using (
                    IDbCommand cmd =
                        database.CreateCommand(
                            "Select Object_Name(object_id) as TableName From sys.objects Where type='U'"))
                {
                    cmd.CommandType = CommandType.Text;
                    using (IDataReader reader = database.ExecuteReader(cmd))
                    {
                        ScriptInfoCollection collection = new ScriptInfoCollection();
                        while (reader.Read())
                        {
                            collection.Add(
                                new ScriptInfo((string)reader["TableName"], string.Empty, config["*"].ScriptMode));
                        }
                        return collection;
                    }
                }
            }
            else
            {
                return config;
            }
        }

        #endregion
    }
}
