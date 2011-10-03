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
		/// <param name="provider">The database communication provider 
		/// using to retrieve or store the data.</param>
		/// <param name="command">The command instance to execute.</param>
		protected override int ExecuteCommand(IDbDataProvider provider, 
			DbCommandDescriptor command)
		{
			return provider.Execute(command).AsNonQuery();
		}
		#endregion
	}
}
