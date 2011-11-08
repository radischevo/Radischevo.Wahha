using System;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents the operation that executes an SQL statement 
	/// against the data source and returns a number of rows affected.
	/// </summary>
	public abstract class DbNonQueryOperation : InvalidatingDbCommandOperation<int>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbNonQueryOperation"/> class.
		/// </summary>
		public DbNonQueryOperation()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns a number of rows affected.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		/// <param name="command">The command instance to execute.</param>
		protected override int ExecuteCommand(DbOperationContext context, 
			DbCommandDescriptor command)
		{
			return context.DataProvider.Execute(command).AsNonQuery();
		}
		#endregion
	}
}
