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
using System.Data;
using System.IO;
using System.Text;

using CodeKing.SqlHarvester.Core;
using CodeKing.SqlHarvester.Core.Data;
using CodeKing.SqlHarvester.Data;

namespace CodeKing.SqlHarvester
{
    internal class SqlSeeder : ISeeder
    {
        #region Constants and Fields

        private readonly string outputDirectory;

        private IDataCommand database;

        private StreamReader itemReader;

        #endregion

        #region Constructors and Destructors

        public SqlSeeder(IDataCommand database, string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
            this.database = database;
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            if (database != null)
            {
                database.Rollback();
                database = null;
            }
            if (itemReader != null)
            {
                itemReader.Close();
                itemReader = null;
            }
        }

        #endregion

        #region ISeeder

        public string[] GetFiles()
        {
            string[] files = Directory.GetFiles(outputDirectory, "*.sql", SearchOption.TopDirectoryOnly);
            Array.Sort(files, StringComparer.InvariantCultureIgnoreCase);
            return files;
        }

        public void ImportFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (!file.Exists)
            {
                throw new ApplicationException(string.Format("file {0} does not exist", Path.GetFileName(fileName)));
            }
            using (FileStream strm = file.OpenRead())
            {
                using (var reader = new StreamReader(strm))
                {
                    var builder = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        string query = reader.ReadLine();
                        if (query == "GO")
                        {
                            ExecuteSql(builder.ToString());
                            builder = new StringBuilder();
                        }
                        else
                        {
                            builder.Append(Environment.NewLine);
                            builder.Append(query);
                        }
                    }
                    if (builder.Length > 0)
                    {
                        ExecuteSql(builder.ToString());
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        private void ExecuteSql(string query)
        {
            if (IsValidQuery(query))
            {
                using (IDbCommand cmd = database.CreateCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    database.ExecuteNonQuery(cmd);
                }
            }
        }

        /// <summary>
        /// Determines whether to execute the sql against the database. This requires the command 
        /// is more than simply a "GO" command.
        /// </summary>
        /// <param name="query">The sql query.</param>
        /// <returns></returns>
        private bool IsValidQuery(string query)
        {
            query = query.Trim();
            if (query.Length < 2)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
