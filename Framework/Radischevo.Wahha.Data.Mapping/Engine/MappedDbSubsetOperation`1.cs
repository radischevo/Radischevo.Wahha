using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data.Mapping
{
	public abstract class MappedDbSubsetOperation<TEntity> : MappedDbSelectOperation<TEntity>
	{
		#region Constructors
		protected MappedDbSubsetOperation()
			: base()
		{
		}
		
		protected MappedDbSubsetOperation(IMetaMappingFactory factory)
			: base(factory)
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
		/// Executes the provided <paramref name="command"/> 
		/// against the provided data source and returns the result.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		/// <param name="command">The command instance to execute.</param>
		protected override IEnumerable<TEntity> ExecuteCommand(DbOperationContext context, 
			DbCommandDescriptor command)
		{
			return context.Provider.Execute(command).AsDataReader(reader => {
				List<TEntity> collection = new List<TEntity>(BufferSize);
				int count = 0;

				while (reader.Read())
					collection.Add(Serializer.Deserialize(reader)); // TODO: Добавить поддержку ResultShapes.

				if (reader.NextResult() && reader.Read())
					count = reader.GetValue<int>(0);

				return collection.ToSubset(count);
			});
		}
		#endregion
	}
}
