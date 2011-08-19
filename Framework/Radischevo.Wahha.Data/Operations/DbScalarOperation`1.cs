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
	public class DbScalarOperation<TResult> : CachedDbCommandOperation<TResult>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbScalarOperation{TResult}"/> class.
		/// </summary>
		public DbScalarOperation()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the scalar result.
		/// </summary>
		/// <param name="provider">The database communication provider 
		/// using to retrieve or store the data.</param>
		protected override TResult ExecuteInternal(IDbDataProvider provider)
		{
			return provider.Execute(Command).AsScalar<TResult>();
		}
		#endregion
	}
}
