using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data.Mapping
{
	public abstract class MappedDbSelectOperation<TEntity> : MappedDbQueryOperation<TEntity, IEnumerable<TEntity>>
	{
		#region Constructors
		protected MappedDbSelectOperation()
			: base()
		{
		}
		
		protected MappedDbSelectOperation(IMetaMappingFactory factory)
			: base(factory)
		{
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
		/// <param name="context">Provides the current operation context.</param>
		/// <param name="command">The command instance to execute.</param>
		protected override IEnumerable<TEntity> ExecuteCommand(DbOperationContext context, 
			DbCommandDescriptor command)
		{
			return context.Provider.Execute(command).AsDataReader(reader => {
				List<TEntity> collection = new List<TEntity>(BufferSize);
				while (reader.Read())
					collection.Add(Serializer.Deserialize(reader)); // TODO: Этого мало, необходимо сделать обработку ResultShape'ов.

				return collection;
			});
		}
		#endregion
	}
}
