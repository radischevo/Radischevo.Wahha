using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class ModelMetadataProviderCollection
    {
        #region Instance Fields
        private ModelMetadataProvider _default;
        private Dictionary<Type, ModelMetadataProvider> _collection;
        #endregion

        #region Constructors
        public ModelMetadataProviderCollection()
        {
            _collection = new Dictionary<Type, ModelMetadataProvider>();
        }
        #endregion

        #region Instance Properties
        public ModelMetadataProvider this[Type modelType]
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

        public ModelMetadataProvider Default
        {
            get
            {
                if (_default == null)
                    _default = new EmptyModelMetadataProvider();

                return _default;
            }
            set
            {
                _default = value;
            }
        }
        #endregion

        #region Instance Methods
        public void Add(Type modelType, ModelMetadataProvider provider)
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

        public ModelMetadataProvider GetProvider(Type modelType)
        {
            return GetProvider(modelType, true);
        }

        public ModelMetadataProvider GetProvider(Type modelType, bool fallbackToDefault)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));

            if(_collection.ContainsKey(modelType)) 
                return _collection[modelType] ?? Default;
            
            return (fallbackToDefault) ? Default : null;
        }

        public void Remove(Type modelType)
        {
            _collection.Remove(modelType);
        }
        #endregion
    }
}
