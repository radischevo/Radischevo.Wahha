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
		/// When overridden in a derived class, validates 
		/// the command arguments and returns a boolean value 
		/// indicating whether the command is valid for execution.
		/// </summary>
		protected override bool Validate()
		{
			Precondition.Require(_command, () =>
				Error.OperationCommandIsNotInitialized());

			return base.Validate();
		}
		#endregion
	}
}
