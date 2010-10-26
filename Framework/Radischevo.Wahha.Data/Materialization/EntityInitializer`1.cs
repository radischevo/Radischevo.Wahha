using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class EntityInitializer<TEntity> : InitializerBase<TEntity>
		where TEntity : class
	{
		#region Constructors
		public EntityInitializer(Type materializerType)
			: base(materializerType)
		{
		}
		#endregion

		#region Instance Methods
		public TEntity Initialize(TEntity entity, IValueSet source)
		{
			Precondition.Require(source, () => Error.ArgumentNull("source"));

			if (Object.ReferenceEquals(entity, null))
				return CreateValue(source);
			
			return LoadValue(entity, source);
		}

		private TEntity CreateValue(IValueSet source)
		{
			Func<IValueSet, TEntity> action = Creator.Build(MaterializerType);
			return action(source);
		}

		private TEntity LoadValue(TEntity entity, IValueSet source)
		{
			Func<TEntity, IValueSet, TEntity> action = Loader.Build(MaterializerType);
			return action(entity, source);
		}
		#endregion
	}
}
