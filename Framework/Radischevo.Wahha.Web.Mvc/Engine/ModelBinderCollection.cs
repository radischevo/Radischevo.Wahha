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
        private Dictionary<Type, IModelBinder> _collection;
        #endregion

        #region Constructors
        public ModelBinderCollection()
        {
            _collection = new Dictionary<Type, IModelBinder>();
        }
        #endregion

        #region Instance Properties
        public IModelBinder this[Type modelType]
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

        public IModelBinder DefaultBinder
        {
            get
            {
                if (_defaultBinder == null)
                    _defaultBinder = new DefaultModelBinder();

                return _defaultBinder;
            }
            set
            {
                _defaultBinder = value;
            }
        }
        #endregion

        #region Instance Methods
        public void Add(Type modelType, IModelBinder binder)
        {
            Precondition.Require(modelType, Error.ArgumentNull("modelType"));
            _collection.Add(modelType, binder);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(Type type)
        {
            return _collection.ContainsKey(type);
        }

        public IModelBinder GetBinder(Type modelType)
        {
            return GetBinder(modelType, true);
        }

        public IModelBinder GetBinder(Type modelType, bool fallbackToDefault)
        {
            Precondition.Require(modelType, Error.ArgumentNull("modelType"));

            if (_collection.ContainsKey(modelType))
                return _collection[modelType] ?? DefaultBinder;

            ModelBinderAttribute[] attrs = modelType.GetCustomAttributes<ModelBinderAttribute>(true).ToArray();
            switch (attrs.Length)
            {
                case 0:
                    return (fallbackToDefault) ? DefaultBinder : null;
                case 1:
                    return attrs[0].GetBinder();
                default:
                    throw Error.MultipleModelBinderAttributes(modelType);
            }
        }

        public void Remove(Type modelType)
        {
            _collection.Remove(modelType);
        }
        #endregion
    }
}
