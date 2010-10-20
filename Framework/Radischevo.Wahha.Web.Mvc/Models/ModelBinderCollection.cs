using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class ModelBinderCollection
    {
        #region Instance Fields
        private IModelBinder _defaultBinder;
        private Dictionary<Type, IModelBinder> _mappings;
		private List<ModelBinderProvider> _providers;
        #endregion

        #region Constructors
        public ModelBinderCollection()
        {
            _mappings = new Dictionary<Type, IModelBinder>();
			_providers = new List<ModelBinderProvider>();
        }
        #endregion

        #region Instance Properties
        public IModelBinder DefaultBinder
        {
            get
            {
                if (_defaultBinder == null)
                    _defaultBinder = new ComplexTypeModelBinder();

                return _defaultBinder;
            }
            set
            {
                _defaultBinder = value;
            }
        }
        #endregion

        #region Instance Methods
		private IModelBinder GetAttributedBinder(Type modelType)
		{
			ModelBinderAttribute[] attrs = modelType
				.GetCustomAttributes<ModelBinderAttribute>(true).ToArray();

			switch (attrs.Length)
			{
				case 0:
					return null;
				case 1:
					return attrs[0].GetBinder();
				default:
					throw Error.MultipleModelBinderAttributes(modelType);
			}
		}

		private IModelBinder GetMappedBinder(Type modelType)
		{
			IModelBinder binder;
			if (_mappings.TryGetValue(modelType, out binder))
				return binder;

			return null;
		}

		private IModelBinder GetProvidedBinder(Type modelType)
		{
			return _providers.OrderBy(s => s.Order)
				.Select(s => s.GetBinder(modelType))
				.Where(b => b != null).FirstOrDefault();
		}

		public void Add(Type modelType, IModelBinder binder)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));
			Precondition.Require(binder, () => Error.ArgumentNull("binder"));

            _mappings.Add(modelType, binder);
        }

		public void Add(ModelBinderProvider provider)
		{
			Precondition.Require(provider, () => Error.ArgumentNull("provider"));
			_providers.Add(provider);
		}

        public void Clear()
        {
            _mappings.Clear();
			_providers.Clear();
        }

        public IModelBinder GetBinder(Type modelType)
        {
            return GetBinder(modelType, true);
        }

        public IModelBinder GetBinder(Type modelType, bool fallbackToDefault)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));

			IModelBinder binder = GetAttributedBinder(modelType) 
				?? GetMappedBinder(modelType) 
				?? GetProvidedBinder(modelType);

			return binder ?? ((fallbackToDefault) ? DefaultBinder : null);
        }

        public void Remove(Type modelType)
        {
            _mappings.Remove(modelType);
        }

		public void Remove(ModelBinderProvider provider)
		{
			_providers.Remove(provider);
		}
        #endregion
    }
}
