using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CodeKing.SqlHarvester.Data
{
    /// <summary>
    /// The abstracted command object used for passing instructions to the data layer.
    /// </summary>
    public interface IDataCommand : IDisposable
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the parameter with value to the given command. This is becuase IDbCommand
        /// does not include the AddWithValue method.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddParameterWithValue(IDbCommand command, string name, object value);

        /// <summary>
        /// Creates the concrete command object to use with this instance.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        IDbCommand CreateCommand(string commandText);

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        IDataReader ExecuteReader(IDbCommand cmd);

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        object ExecuteScalar(IDbCommand cmd);

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        int ExecuteNonQuery(IDbCommand cmd);

        /// <summary>
        /// Commits the active transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rolls back the active transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Begins a new active transaction.
        /// </summary>
        void BeginTransaction();
    }
}
