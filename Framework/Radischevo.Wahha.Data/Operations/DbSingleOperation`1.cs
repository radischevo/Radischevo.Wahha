using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents an operation, that queries the database using 
	/// the specified command and returns a single entity of specified type.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public abstract class DbSingleOperation<TEntity> : DbQueryOperation<TEntity, TEntity>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbSingleOperation{TEntity}"/> class.
		/// </summary>
		protected DbSingleOperation()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbSingleOperation{TEntity}"/> class.
		/// </summary>
		/// <param name="materializer">The <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.</param>
		protected DbSingleOperation(IDbMaterializer<TEntity> materializer)
			: base()
		{
			Materializer = materializer;
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the provided <paramref name="command"/> 
		/// against the provided data source and returns the result.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		/// <param name="command">The command instance to execute.</param>
		protected override TEntity ExecuteCommand(DbOperationContext context, 
			DbCommandDescriptor command)
		{
			return context.Provider.Execute(command).AsDataReader<TEntity>(reader => {
				if (reader.Read())
					return Materializer.Materialize(reader);

				return default(TEntity);
			});
		}
		#endregion
	}
}
