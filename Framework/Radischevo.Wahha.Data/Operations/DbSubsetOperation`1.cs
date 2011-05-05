using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents an operation, that queries the database using 
	/// the specified command and returns a partial collection 
	/// of entities of specified type.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public class DbSubsetOperation<TEntity> : DbSelectOperation<TEntity>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbSubsetOperation{TEntity}"/> class.
		/// </summary>
		public DbSubsetOperation()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbSubsetOperation{TEntity}"/> class.
		/// </summary>
		/// <param name="materializer">The <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.</param>
		public DbSubsetOperation(IDbMaterializer<TEntity> materializer)
			: base(materializer)
		{
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the initial size of the 
		/// resulting collection.
		/// </summary>
		protected override int BufferSize
		{
			get
			{
				return 15;
			}
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the result.
		/// </summary>
		/// <param name="provider">The database communication provider 
		/// using to retrieve or store the data.</param>
		protected override IEnumerable<TEntity> ExecuteInternal(IDbDataProvider provider)
		{
			return provider.Execute(Command).AsDataReader(reader => {
				List<TEntity> collection = new List<TEntity>(BufferSize);
				int count = 0;

				while (reader.Read())
					collection.Add(Materializer.Materialize(reader));

				if (reader.NextResult() && reader.Read())
					count = reader.GetValue<int>(0);

				return collection.ToSubset(count);
			});
		}
		#endregion
	}
}
