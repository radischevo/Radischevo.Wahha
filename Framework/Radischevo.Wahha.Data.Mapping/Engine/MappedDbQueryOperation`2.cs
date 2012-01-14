using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Mapping.Configurations;

namespace Radischevo.Wahha.Data.Mapping
{
	public abstract class MappedDbQueryOperation<TEntity, TResult> : CacheableDbCommandOperation<TResult>
	{
		#region Nested Types
		private sealed class DbSerializerWrapper : IDbSerializer<TEntity>
		{
			#region Instance Fields
			private IDbSerializer _serializer;
			#endregion
			
			#region Constructors
			public DbSerializerWrapper (IDbSerializer serializer)
			{
				Precondition.Require (serializer, () => Error.ArgumentNull ("serializer"));
				_serializer = serializer;
			}
			#endregion
			
			#region Instance Methods
			public IValueSet Serialize (TEntity entity, DbQueryStatement statement)
			{
				return _serializer.Serialize (entity, statement);
			}
	
			public TEntity Deserialize (IDbValueSet source)
			{
				return (TEntity)_serializer.Deserialize (source);
			}
			#endregion
	
			#region IDbSerializer Members
			IValueSet IDbSerializer.Serialize (object entity, DbQueryStatement statement)
			{
				return Serialize ((TEntity)entity, statement);
			}
	
			object IDbSerializer.Deserialize (IDbValueSet source)
			{
				return Deserialize (source);
			}
			#endregion
		}
		#endregion
		
		#region Instance Fields
		private IDbSerializer<TEntity> _serializer;
		#endregion
		
		#region Constructors
		protected MappedDbQueryOperation ()
			: this(Configuration.Instance.Factory)
		{
		}
		
		protected MappedDbQueryOperation (IMetaMappingFactory factory)
			: base()
		{
			Precondition.Require (factory, () => Error.ArgumentNull ("factory"));
			MetaType metaType = factory.CreateMapping (typeof(TEntity));
			
			_serializer = CastSerializer (metaType.Serializer);
		}
		#endregion
		
		#region Instance Properties
		protected IDbSerializer<TEntity> Serializer 
		{
			get 
			{
				return _serializer;
			}
		}
		#endregion
		
		#region Static Methods
		private static IDbSerializer<TEntity> CastSerializer (IDbSerializer serializer)
		{
			IDbSerializer<TEntity> casted = (serializer as IDbSerializer<TEntity>);
			if (casted == null)
				return new DbSerializerWrapper (serializer);
			
			return casted;
		}
		#endregion
	}
}

