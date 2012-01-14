using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class DbSerializer<TEntity> : IDbSerializer<TEntity>, IDbMaterializer<TEntity>
	{
		#region Constructors
		protected DbSerializer()
		{
		}
		#endregion
		
		#region Instance Methods
		protected abstract TEntity CreateInstance (IDbValueSet source);
		
		public abstract IValueSet Serialize (TEntity entity, DbQueryStatement statement);

		public TEntity Deserialize (IDbValueSet source)
		{
			Precondition.Require(source, () => Error.ArgumentNull("source"));
			TEntity instance = CreateInstance(source);
			
			return Deserialize (instance, source);
		}
		
		protected abstract TEntity Deserialize (TEntity entity, IDbValueSet source);
		#endregion

		#region Interface Implementation
		IValueSet IDbSerializer.Serialize (object entity, DbQueryStatement statement)
		{
			return Serialize((TEntity)entity, statement);
		}

		object IDbSerializer.Deserialize (IDbValueSet dataRow)
		{
			return Deserialize(dataRow);
		}
		
		TEntity IDbMaterializer<TEntity>.Materialize (IDbValueSet source)
		{
			return Deserialize(source);
		}

		public TEntity Materialize (TEntity entity, IDbValueSet source)
		{
			return Deserialize(entity, source);
		}
		#endregion
	}
}

