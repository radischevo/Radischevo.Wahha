using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents an operation, that queries the database using 
	/// the specified command and returns a collection of entities 
	/// of specified type.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public abstract class DbSelectOperation<TEntity> : DbQueryOperation<TEntity, IEnumerable<TEntity>>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbSelectOperation{TEntity}"/> class.
		/// </summary>
		protected DbSelectOperation()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbSelectOperation{TEntity}"/> class.
		/// </summary>
		/// <param name="materializer">The <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.</param>
		protected DbSelectOperation(IDbMaterializer<TEntity> materializer)
			: base()
		{
			Materializer = materializer;
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the initial size of the 
		/// resulting collection.
		/// </summary>
		protected virtual int BufferSize
		{
			get
			{
				return 30;
			}
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the provided <paramref name="command"/> 
		/// against the provided data source and returns the result.
		/// </summary>
		/// <param name="provider">The database communication provider 
		/// using to retrieve or store the data.</param>
		/// <param name="command">The command instance to execute.</param>
		protected override IEnumerable<TEntity> ExecuteCommand(IDbDataProvider provider, 
			DbCommandDescriptor command)
		{
			return provider.Execute(command).AsDataReader(reader => {
				List<TEntity> collection = new List<TEntity>(BufferSize);
				while (reader.Read())
					collection.Add(Materializer.Materialize(reader));

				return collection;
			});
		}
		#endregion
	}
}
