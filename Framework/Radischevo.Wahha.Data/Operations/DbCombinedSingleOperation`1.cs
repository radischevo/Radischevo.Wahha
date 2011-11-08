using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents an operation, that first updates and then queries the database using 
	/// the specified command and returns a single entity of specified type.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public abstract class DbCombinedSingleOperation<TEntity> : DbSingleOperation<TEntity>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbCombinedSingleOperation{TEntity}"/> class.
		/// </summary>
		protected DbCombinedSingleOperation()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbCombinedSingleOperation{TEntity}"/> class.
		/// </summary>
		/// <param name="materializer">The <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.</param>
		protected DbCombinedSingleOperation(IDbMaterializer<TEntity> materializer)
			: base(materializer)
		{
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the result.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		protected override TEntity ExecuteInternal(DbOperationContext context)
		{
			context.Cache.Invalidate(Tags);
			return base.ExecuteInternal(context);
		}
		#endregion
	}
}
