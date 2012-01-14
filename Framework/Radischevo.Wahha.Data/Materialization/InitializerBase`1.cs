using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class InitializerBase<TEntity>
	{
		#region Instance Fields
		private Type _materializerType;
		private EntityCreatorBuilder<TEntity> _creator;
		private EntityLoaderBuilder<TEntity> _loader;
		#endregion

		#region Constructors
		protected InitializerBase(Type materializerType)
		{
			Precondition.Require(materializerType, 
				() => Error.ArgumentNull("materializerType"));

			_materializerType = materializerType;
		}
		#endregion

		#region Instance Properties
		protected Type MaterializerType
		{
			get
			{
				return _materializerType;
			}
		}

		protected virtual EntityCreatorBuilder<TEntity> Creator
		{
			get
			{
				if (_creator == null)
					_creator = new EntityCreatorBuilder<TEntity>();

				return _creator;
			}
		}

		protected virtual EntityLoaderBuilder<TEntity> Loader
		{
			get
			{
				if (_loader == null)
					_loader = new EntityLoaderBuilder<TEntity>();

				return _loader;
			}
		}
		#endregion
	}
}
