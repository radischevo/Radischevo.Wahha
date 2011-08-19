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
		#region Instance Fields
		private DbCommandDescriptor _command;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbCommandOperation{TResult}"/> class.
		/// </summary>
		protected DbCommandOperation()
		{
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets or sets the <see cref="Radischevo.Wahha.Data.DbCommandDescriptor">command</see> 
		/// which will be executed by this operation.
		/// </summary>
		public DbCommandDescriptor Command
		{
			get
			{
				return _command;
			}
			set
			{
				Precondition.Require(value, () =>
					Error.ArgumentNull("value"));

				_command = value;
			}
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the result.
		/// </summary>
		/// <param name="provider">The database communication provider 
		/// using to retrieve or store the data.</param>
		public override TResult Execute(IDbDataProvider provider)
		{
			Precondition.Require(Command, () => 
				Error.OperationCommandIsNotInitialized());

			return base.Execute(provider);
		}
		#endregion
	}
}
