﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Radischevo.Wahha.Core
{
    internal sealed class SubsetWrapper : IValueSet
    {
        #region Instance Fields
        private IValueSet _collection;
        private Func<string, bool> _keySelector;
        #endregion

        #region Constructors
        public SubsetWrapper(IValueSet collection, 
            Func<string, bool> keySelector)
        {
			Precondition.Require(collection, () => Error.ArgumentNull("collection"));
			Precondition.Require(keySelector, () => Error.ArgumentNull("keySelector"));

            _collection = collection;
            _keySelector = keySelector;
        }
        #endregion

        #region Instance Properties
        public object this[string key]
        {
            get 
            {
                return GetValue<object>(key, null, CultureInfo.CurrentCulture);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _collection.Keys
                    .Where(k => _keySelector(k));
            }
        }
        #endregion

        #region Instance Methods
        public TValue GetValue<TValue>(string key, TValue defaultValue, 
			IFormatProvider provider)
        {
            if (Keys.Contains(key, StringComparer.OrdinalIgnoreCase))
                return _collection.GetValue<TValue>(key, defaultValue, provider);

            return defaultValue;
        }
        #endregion
    }
}
