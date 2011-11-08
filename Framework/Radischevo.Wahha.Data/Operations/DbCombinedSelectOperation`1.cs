using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents an operation, that first updates and then queries the database using 
	/// the specified command and returns a collection of entities of specified type.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public abstract class DbCombinedSelectOperation<TEntity> : DbSelectOperation<TEntity>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbCombinedSelectOperation{TEntity}"/> class.
		/// </summary>
		protected DbCombinedSelectOperation()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbCombinedSelectOperation{TEntity}"/> class.
		/// </summary>
		/// <param name="materializer">The <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.</param>
		protected DbCombinedSelectOperation(IDbMaterializer<TEntity> materializer)
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
		protected override IEnumerable<TEntity> ExecuteInternal(DbOperationContext context)
		{
			context.CacheProvider.Invalidate(Tags);
			return base.ExecuteInternal(context);
		}
		#endregion
	}
}
