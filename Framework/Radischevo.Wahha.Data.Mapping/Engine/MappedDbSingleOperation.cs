using System;

namespace Radischevo.Wahha.Data.Mapping
{
	public abstract class MappedDbSingleOperation<TEntity> : MappedDbQueryOperation<TEntity, TEntity>
	{
		#region Constructors
		protected MappedDbSingleOperation()
			: base()
		{
		}
		
		protected MappedDbSingleOperation(IMetaMappingFactory factory)
			: base(factory)
		{
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the provided <paramref name="command"/> 
		/// against the provided data source and returns the result.
		/// </summary>
		/// <param name="context">Provides the current operation context.</param>
		/// <param name="command">The command instance to execute.</param>
		protected override TEntity ExecuteCommand(DbOperationContext context, DbCommandDescriptor command)
		{
			return context.Provider.Execute(command).AsDataReader<TEntity>(reader => {
				if (reader.Read())
					return Serializer.Deserialize (reader); // TODO: Этого мало, необходимо сделать обработку ResultShape'ов.

				return default(TEntity);
			});
		}
		#endregion
	}
}

