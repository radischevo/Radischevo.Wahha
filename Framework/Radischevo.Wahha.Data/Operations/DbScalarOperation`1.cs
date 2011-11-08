using System;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents the operation that queries the database using 
	/// the specified command and converts the first column of 
	/// the first row in the resultset to the 
	/// <typeparamref name="TResult"/> type. If no rows returned or field type 
	/// does not support the required conversion, a default value is returned.
	/// </summary>
	public abstract class DbScalarOperation<TResult> : CachedDbCommandOperation<TResult>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbScalarOperation{TResult}"/> class.
		/// </summary>
		protected DbScalarOperation()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the scalar result.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		/// <param name="command">The command instance to execute.</param>
		protected override TResult ExecuteCommand(DbOperationContext context, 
			DbCommandDescriptor command)
		{
			return context.Provider.Execute(command).AsScalar<TResult>();
		}
		#endregion
	}
}
