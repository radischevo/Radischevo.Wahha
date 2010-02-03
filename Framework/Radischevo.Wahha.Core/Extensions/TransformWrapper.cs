using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Core
{
    internal sealed class TransformWrapper : IValueSet
    {
        #region Instance Fields
        private IValueSet _collection;
		private IDictionary<string, string> _map;
        private Func<string, string> _keyTransformer;
        #endregion

        #region Constructors
        public TransformWrapper(IValueSet collection,
            Func<string, string> keyTransformer)
        {
            Precondition.Require(collection, Error.ArgumentNull("collection"));
            Precondition.Require(keyTransformer, Error.ArgumentNull("keySelector"));

            _collection = collection;
            _keyTransformer = keyTransformer;
        }
        #endregion

        #region Instance Properties
		private IDictionary<string, string> Map
		{
			get
			{
				if (_map == null)
					_map = _collection.Keys.ToDictionary(k => _keyTransformer(k),
						k => k, StringComparer.OrdinalIgnoreCase);

				return _map;
			}
		}

        public object this[string key]
        {
            get
            {
                return GetValue<object>(key);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
				return Map.Keys;
            }
        }
        #endregion

        #region Instance Methods
        public TValue GetValue<TValue>(string key)
        {
            return GetValue<TValue>(key, default(TValue));
        }

        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
			string mappedKey;
			if(Map.TryGetValue(key, out mappedKey))
				return _collection.GetValue<TValue>(mappedKey, defaultValue);

			return defaultValue;
        }
        #endregion
    }
}
