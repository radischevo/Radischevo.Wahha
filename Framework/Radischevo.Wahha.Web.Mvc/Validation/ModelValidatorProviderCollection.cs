using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public sealed class ModelValidatorProviderCollection
    {
        #region Instance Fields
        private ModelValidatorProvider _default;
        private Dictionary<Type, ModelValidatorProvider> _collection;
        #endregion

        #region Constructors
        public ModelValidatorProviderCollection()
        {
            _collection = new Dictionary<Type, ModelValidatorProvider>();
        }
        #endregion

        #region Instance Properties
        public ModelValidatorProvider this[Type modelType]
        {
            get
            {
                if (_collection.ContainsKey(modelType))
                    return _collection[modelType];

                return null;
            }
            set
            {
                _collection[modelType] = value;
            }
        }

        public ModelValidatorProvider Default
        {
            get
            {
                if (_default == null)
                    _default = new EmptyModelValidatorProvider();

                return _default;
            }
            set
            {
                _default = value;
            }
        }
        #endregion

        #region Instance Methods
		private bool TryGetProvider(Type modelType, out ModelValidatorProvider provider)
		{
			provider = null;
			do
			{
				if (_collection.TryGetValue(modelType, out provider))
					return true;

				modelType = modelType.BaseType;
			}
			while (modelType != null);
			return false;
		}

        public void Add(Type modelType, ModelValidatorProvider provider)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));
            _collection.Add(modelType, provider);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(Type type)
        {
            return _collection.ContainsKey(type);
        }

        public ModelValidatorProvider GetProvider(Type modelType)
        {
            return GetProvider(modelType, true);
        }

        public ModelValidatorProvider GetProvider(Type modelType, bool fallbackToDefault)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));

			ModelValidatorProvider provider;
            if (TryGetProvider(modelType, out provider))
                return provider ?? Default;

            return (fallbackToDefault) ? Default : null;
        }

        public void Remove(Type modelType)
        {
            _collection.Remove(modelType);
        }
        #endregion
    }
}
