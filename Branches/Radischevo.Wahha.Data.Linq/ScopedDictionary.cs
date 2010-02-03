using System;
using System.Collections.Generic;

namespace Jeltofiol.Wahha.Data.Linq
{
    public class ScopedDictionary<TKey, TValue>
    {
        #region Instance Fields
        private ScopedDictionary<TKey, TValue> _previous;
        private Dictionary<TKey, TValue> _map;
        #endregion

        #region Constructors
        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous)
        {
            _previous = previous;
            _map = new Dictionary<TKey, TValue>();
        }

        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous, 
            IEnumerable<KeyValuePair<TKey, TValue>> pairs)
            : this(previous)
        {
            foreach (KeyValuePair<TKey, TValue> p in pairs)
                _map.Add(p.Key, p.Value);
        }
        #endregion

        #region Instance Methods
        public void Add(TKey key, TValue value)
        {
            _map.Add(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            for (ScopedDictionary<TKey, TValue> scope = this; 
                scope != null; scope = scope._previous)
                if (scope._map.TryGetValue(key, out value))
                    return true;
            
            value = default(TValue);
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            for (ScopedDictionary<TKey, TValue> scope = this; 
                scope != null; scope = scope._previous)
                if (scope._map.ContainsKey(key))
                    return true;
            
            return false;
        }
        #endregion
    }
}
