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
		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the result.
		/// </summary>
		/// <param name="provider">The database communication provider 
		/// using to retrieve or store the data.</param>
		public TResult Execute(IDbDataProvider provider)
		{
			Precondition.Require(provider, () =>
				Error.ArgumentNull("provider"));

			return ExecuteInternal(provider);
		}

		/// <summary>
		/// When overridden in a derived class, executes the operation 
		/// against the provided data source.
		/// </summary>
		/// <param name="provider">The database communication provider 
		/// to retrieve or store the data.</param>
		protected abstract TResult ExecuteInternal(IDbDataProvider provider);
		#endregion

		#region Interface Members
		/// <summary>
		/// Executes the operation against the provided data source.
		/// </summary>
		/// <param name="provider">The database communication provider 
		/// using to store the data.</param>
		void IContextualOperation<IDbDataProvider>.Execute(IDbDataProvider context)
		{
			Execute(context);
		}
		#endregion
	}
}
