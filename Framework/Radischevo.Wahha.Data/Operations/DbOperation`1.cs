using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Defines a contract for the atomic interaction with the database source.
	/// </summary>
	/// <typeparam name="TResult">The CLR type of the result.</typeparam>
	public abstract class DbOperation<TResult> : IDbOperation<TResult>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbOperation{TResult}" /> class.
		/// </summary>
		protected DbOperation()
		{
		}
		#endregion

		#region Instance Methods
		private void ValidateOperation()
		{
			ValidationContext context = new ValidationContext();
			Validate(context);

			if (!context.IsValid)
				throw new ValidationException(context);
		}

		/// <summary>
		/// When overridden in a derived class, determines 
		/// whether the operation is valid for execution 
		/// and populates the provided context with detected errors.
		/// </summary>
		protected virtual void Validate(ValidationContext context)
		{
		}

		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the result.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		public TResult Execute(DbOperationContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));

			ValidateOperation();
			return ExecuteInternal(context);
		}

		/// <summary>
		/// When overridden in a derived class, executes the operation 
		/// against the provided data source.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		protected abstract TResult ExecuteInternal(DbOperationContext context);
		#endregion

		#region Interface Members
		/// <summary>
		/// Executes the operation against the provided data source.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		void IContextualOperation<DbOperationContext>.Execute(DbOperationContext context)
		{
			Execute(context);
		}
		#endregion
	}
}
