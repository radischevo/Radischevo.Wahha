using System;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Provides additional extension methods for the 
    /// <see cref="System.Data.IDbConnection"/> interface 
    /// implementations.
    /// </summary>
    public static class DbConnectionExtensions
    {
        #region Static Extension Methods
        /// <summary>
        /// Executes the specified <paramref name="action"/> against the connection.
        /// </summary>
        /// <param name="connection">The connection instance.</param>
        /// <param name="action">An action to execute.</param>
        public static void Execute(this IDbConnection connection, Action<IDbConnection> action)
        {
            Precondition.Require(connection, Error.ArgumentNull("connection"));
            Precondition.Require(action, Error.ArgumentNull("action"));

            ConnectionState state = connection.State;            
            try
            {
                if (state == ConnectionState.Closed)
                    connection.Open();

                action(connection);
            }
            finally
            {
                if (state == ConnectionState.Closed)
                    connection.Close();
            }
        }

        /// <summary>
        /// Executes the specified <paramref name="action"/> against the connection and 
        /// returns its result.
        /// </summary>
        /// <typeparam name="TResult">The type of the action result.</typeparam>
        /// <param name="connection">The connection instance.</param>
        /// <param name="action">An action to execute.</param>
        public static TResult Execute<TResult>(this IDbConnection connection, 
            Func<IDbConnection, TResult> action)
        {
            Precondition.Require(connection, Error.ArgumentNull("connection"));
            Precondition.Require(action, Error.ArgumentNull("action"));

            TResult result = default(TResult);
            
            ConnectionState state = connection.State;            
            try
            {
                if (state == ConnectionState.Closed)
                    connection.Open();

                result = action(connection);
            }
            finally
            {
                if (state == ConnectionState.Closed)
                    connection.Close();
            }
            return result;
        }
        #endregion
    }
}
