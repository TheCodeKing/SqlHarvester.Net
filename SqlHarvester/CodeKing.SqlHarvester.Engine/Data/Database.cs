using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using CodeKing.SqlHarvester.Data;

namespace CodeKing.SqlHarvester.Data
{
	/// <summary>
    /// A datalayer used for excecuting commands on the database. 
    /// </summary>
    internal class Database : IDataCommand
    {
        private string connectionString;
        private IDbTransaction sqlTransaction;
        private int connectionTimeout = 120; // 2 mins
        private bool isTransaction = false;

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                if (connectionString!=null && connectionString.Contains("Connection Timeout"))
                {
                    return connectionString;
                }
                else
                {
                    return string.Format("{0};Connection Timeout={1};", connectionString, connectionTimeout);
                }
            }
            set
            {
                connectionString = value;
            }
        }

        /// <summary>
        /// Adds the parameter with value to the given command. This is becuase IDbCommand
        /// does not include the AddWithValue method.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual void AddParameterWithValue(IDbCommand command, string name, object value)
        {
            SqlCommand cmd = command as SqlCommand;
            if (cmd==null)
            {
                throw new DatabaseException("The command object must be of type SqlCommand");
            }
            cmd.Parameters.AddWithValue(name, value);
        }

        /// <summary>
        /// Creates the concrete command instance to use with this implementation.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public virtual IDbCommand CreateCommand(string commandText)
        {
            if (sqlTransaction != null)
            {
                SqlTransaction trans = sqlTransaction as SqlTransaction;
                return new SqlCommand(commandText, trans.Connection, trans);
            }
            else
            {
                return new SqlCommand(commandText);
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        private IDbConnection GetConnectionCheckTransaction()
        {
            // if should be transactional
            if (isTransaction)
            {
                // if not yet a transaction start one
                if (sqlTransaction == null)
                {
                    IDbConnection conn = GetConnection();
                    sqlTransaction = conn.BeginTransaction();
                    Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Begin transaction");
                    return conn;
                }
                else
                {
                    // return existing transaction
                    return sqlTransaction.Connection;
                }
            }
            else
            {
                // create new connection
                return GetConnection();
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public IDbConnection GetConnection(IDbCommand cmd)
        {
            IDbConnection conn = GetConnectionCheckTransaction();
            cmd.Connection = conn;
            if (isTransaction)
            {
                cmd.Transaction = sqlTransaction;
            }
            return conn;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public virtual object ExecuteScalar(IDbCommand cmd)
        {
            IDbConnection myConn = null;
            try
            {
                myConn = GetConnection(cmd);
                return cmd.ExecuteScalar();
            }
            finally
            {
                if (!isTransaction)
                {
                    // only close connection if not in a transaction
                    if (myConn != null && myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Executes the non query
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(IDbCommand cmd)
        {
            IDbConnection myConn = null;
            try
            {
                myConn = GetConnection(cmd);
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                if (!isTransaction)
                {
                    // only close connection if not in a transaction
                    if (myConn != null && myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(IDbCommand cmd)
        {

            IDbConnection myConn = null;
            try
            {
                myConn = GetConnection(cmd);
                if (isTransaction)
                {
                    return cmd.ExecuteReader();
                }
                else
                {
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (SqlException e)
            {
                if (!isTransaction)
                {
                    // only close connection if not in a transaction
                    if (myConn != null && myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
                throw e;
            }
            finally
            {
                // keep connection open as passing back a reader
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public Database(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionTimeout">The connection timeout.</param>
        public Database(string connectionString, int connectionTimeout)
        {
            this.connectionString = connectionString;
            this.connectionTimeout = connectionTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseAccess"/> class.
        /// </summary>
        public Database()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (sqlTransaction != null)
            {
                Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Rolling back transaction");
                sqlTransaction.Rollback();
                sqlTransaction.Dispose();
                sqlTransaction = null;
                isTransaction = false;
            }
        }

        /// <summary>
        /// Commits the active transaction.
        /// </summary>
        public void Commit()
        {
            if (sqlTransaction != null)
            {
                Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Commiting transaction");
                sqlTransaction.Commit();
                sqlTransaction.Dispose();
                sqlTransaction = null;
                isTransaction = false;
            }
        }

        /// <summary>
        /// Rollback the active transaction.
        /// </summary>
        public void Rollback()
        {
            if (sqlTransaction != null)
            {
                Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Explicitly rolling back transaction");
                sqlTransaction.Rollback();
                sqlTransaction.Dispose();
                sqlTransaction = null;
                isTransaction = false;
            }
        }

        /// <summary>
        /// Begins a new active transaction.
        /// </summary>
        public void BeginTransaction()
        {
            Trace.WriteLineIf(Tracer.Trace.TraceVerbose, "Set Transaction mode true");
            isTransaction = true;
        }
    }
}
