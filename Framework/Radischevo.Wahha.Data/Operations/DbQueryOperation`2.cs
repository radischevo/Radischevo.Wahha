using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Defines the base class for the operation, that queries the database using 
	/// the specified command and converts the result to the specified entity type.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity for materialization.</typeparam>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	public abstract class DbQueryOperation<TEntity, TResult> : CacheableDbCommandOperation<TResult>
	{
		#region Instance Fields
		private IDbMaterializer<TEntity> _materializer;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.DbQueryOperation{TEntity,TResult}"/> class.
		/// </summary>
		protected DbQueryOperation(IDbMaterializer<TEntity> materializer)
		{
			Materializer = materializer;
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets or sets the <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.
		/// </summary>
		public IDbMaterializer<TEntity> Materializer
		{
			get
			{
				return _materializer;
			}
			set
			{
				Precondition.Require(value, () =>
					Error.ArgumentNull("value"));

				_materializer = value;
			}
		}
		#endregion
	}
}
