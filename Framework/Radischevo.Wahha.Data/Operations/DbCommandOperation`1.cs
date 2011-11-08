using System;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Defines the base class for the database operation 
	/// that executes the provided <see cref="Radischevo.Wahha.Data.DbCommandDescriptor">command</see> 
	/// and returns the result.
	/// </summary>
	/// <typeparam name="TResult">The type of operation result.</typeparam>
	public abstract class DbCommandOperation<TResult> : DbOperation<TResult>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbCommandOperation{TResult}"/> class.
		/// </summary>
		protected DbCommandOperation()
		{
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Creates the <see cref="Radischevo.Wahha.Data.DbCommandDescriptor">command</see> 
		/// instance which will be executed by this operation.
		/// </summary>
		protected abstract DbCommandDescriptor CreateCommand();

		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the result.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		protected override TResult ExecuteInternal(DbOperationContext context)
		{
			DbCommandDescriptor command = CreateCommand();
			Precondition.Require(command, () => Error.CommandIsNotInitialized());

			return ExecuteCommand(context, command);
		}

		/// <summary>
		/// When overridden in a derived class, executes the provided <paramref name="command"/> 
		/// against the provided data source and returns the result.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		/// <param name="command">The command instance to execute.</param>
		protected abstract TResult ExecuteCommand(DbOperationContext context, DbCommandDescriptor command);
		#endregion
	}
}
