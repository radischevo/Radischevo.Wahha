using System;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public static class DbDataProviderExtensions
	{
		/// <summary>
		/// Creates a <see cref="System.Data.IDbCommand"/> instance, which is 
		/// appropriate for the current provider, using the specified 
		/// <paramref name="commandText"/> and returns a 
		/// <see cref="Radischevo.Wahha.Data.DbCommandResult"/> wrapper.
		/// </summary>
		/// <param name="commandText">The text of the query to execute.</param>
		public static DbCommandResult Execute(this IDbDataProvider provider, 
			string commandText)
		{
			return Execute(provider, new DbCommandDescriptor(commandText));
		}

		/// <summary>
		/// Creates a <see cref="System.Data.IDbCommand"/> instance, which is 
		/// appropriate for the current provider, using the specified 
		/// <paramref name="commandText"/> and <paramref name="parameters" /> 
		/// and returns a <see cref="Radischevo.Wahha.Data.DbCommandResult"/> wrapper.
		/// </summary>
		/// <param name="commandText">The text of the query to execute.</param>
		/// <param name="parameters">The object containing command parameters.</param>
		public static DbCommandResult Execute(this IDbDataProvider provider, 
			string commandText, object parameters)
		{
			return Execute(provider, new DbCommandDescriptor(commandText, 
				CommandType.Text, parameters));
		}

		/// <summary>
		/// Creates a <see cref="System.Data.IDbCommand"/> instance, which is 
		/// appropriate for the current provider, using the specified 
		/// <paramref name="commandText"/> and <paramref name="parameters" /> 
		/// and returns a <see cref="Radischevo.Wahha.Data.DbCommandResult"/> wrapper.
		/// </summary>
		/// <param name="commandText">The text of the query to execute.</param>
		/// <param name="parameters">The list of command parameters.</param>
		public static DbCommandResult Execute(this IDbDataProvider provider, 
			string commandText, object[] parameters)
		{
			return Execute(provider, new DbCommandDescriptor(commandText, 
				CommandType.Text, parameters));
		}

		/// <summary>
		/// Creates a <see cref="Radischevo.Wahha.Data.DbCommandResult"/> wrapper for 
		/// the specified <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The object, describing the command to execute.</param>
		public static DbCommandResult Execute(this IDbDataProvider provider, DbCommandDescriptor command)
		{
			Precondition.Require(provider, () => Error.ArgumentNull("provider"));
			command.Timeout = Configurations.Configuration.Instance.Database.CommandTimeout;

			return new DbCommandResult(provider, command);
		}
	}
}
