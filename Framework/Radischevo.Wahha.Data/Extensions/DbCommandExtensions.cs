using System;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Provides additional extension methods 
    /// for the <see cref="System.Data.IDbCommand"/> interface 
    /// implementations.
    /// </summary>
    public static class DbCommandExtensions
    {
        #region Static Extension Methods
        /// <summary>
        /// Executes the query, captures the first 
        /// column of the first row in the resultset returned 
        /// by the query, and then converts the result to the 
        /// <typeparamref name="TResult"/> type. 
        /// If conversion is not possible, a default value is returned.
        /// </summary>
        /// <param name="command">An <see cref="IDbCommand"/> 
        /// command instance</param>
        /// <typeparam name="TResult">The type of the return value</typeparam>
        public static TResult ExecuteScalar<TResult>(this IDbCommand command)
        {
            return Converter.ChangeType<TResult>(command.ExecuteScalar());
        }

        /// <summary>
        /// Creates a new instance of an <see cref="System.Data.IDbDataParameter"/> object 
        /// with specified name and value and appends it to the parameter list.
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="parameterName">Specifies the name of the parameter</param>
        /// <param name="value">Specifies the value of the parameter. If a null value is provided, 
        /// parameter value will be set to <see cref="DBNull.Value"/>.</param>
        public static IDbDataParameter AddParameter(this IDbCommand command, 
            string parameterName, object value)
        {
            return AddParameter(command, parameterName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="System.Data.IDbDataParameter"/> object 
        /// with specified name and value and appends it to the parameter list.
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="parameterName">Specifies the name of the parameter</param>
        /// <param name="value">Specifies the value of the parameter. If a null value is provided, 
        /// parameter value will be set to <see cref="DBNull.Value"/>.</param>
        public static IDbDataParameter AddParameter(this IDbCommand command,
            string parameterName, object value, ParameterDirection direction)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Direction = direction;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);

            return parameter;
        }
        #endregion
    }
}
