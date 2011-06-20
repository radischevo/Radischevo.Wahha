using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Web.Templates.Scope {
    public class ScopeStorageDictionary : IDictionary<string, object> {
        private readonly IDictionary<string, object> _baseScope;
        private readonly IDictionary<string, object> _backingStore;
        private static readonly StateStorageKeyValueComparer _keyValueComparer = new StateStorageKeyValueComparer();

        public ScopeStorageDictionary()
            : this(null) {
        }

        public ScopeStorageDictionary(IDictionary<string, object> baseScope)
            : this(baseScope, new Dictionary<string, object>(ScopeStorageComparer.Instance)) {
        }

        /// <param name="backingStore">
        /// The dictionary to use as a storage. Since the dictionary would be used as-is, we expect the implementer to 
        /// use the same key-value comparison logic as we do here.
        /// </param>
        internal ScopeStorageDictionary(IDictionary<string, object> baseScope, 
			IDictionary<string, object> backingStore) {
            _baseScope = baseScope;
            _backingStore = backingStore;
        }

        public object this[string key] {
            get {
                object value;
                TryGetValue(key, out value);
                return value;
            }
            set {
                _backingStore[key] = value;
            }
        }

        protected IDictionary<string, object> BackingStore {
            get {
                return _backingStore;
            }
        }

        protected IDictionary<string, object> BaseScope {
            get {
                return _baseScope;
            }
        }

        public virtual void SetValue(string key, object value) {
            _backingStore[key] = value;
        }

        public virtual bool TryGetValue(string key, out object value) {
            return _backingStore.TryGetValue(key, out value) || (_baseScope != null && _baseScope.TryGetValue(key, out value));
        }

        public virtual bool Remove(string key) {
            return _backingStore.Remove(key);
        }

        public virtual IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            return GetItems().GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public virtual void Add(string key, object value) {
            _backingStore.Add(key, value);
        }

        public virtual bool ContainsKey(string key) {
            return _backingStore.ContainsKey(key) || (_baseScope != null && _baseScope.ContainsKey(key));
        }

        public virtual ICollection<string> Keys {
            get {
                return GetItems().Select(item => item.Key).ToList();
            }
        }

        public virtual ICollection<object> Values {
            get {
                return GetItems().Select(item => item.Value).ToList();
            }
        }

        public virtual void Add(KeyValuePair<string, object> item) {
            _backingStore.Add(item);
        }

        public virtual void Clear() {
            _backingStore.Clear();
        }

        public virtual bool Contains(KeyValuePair<string, object> item) {
            return _backingStore.Contains(item) || (_baseScope != null && _baseScope.Contains(item));
        }

        public virtual void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
            GetItems().ToList().CopyTo(array, arrayIndex);
        }

        public virtual int Count {
            get {
                return GetItems().Count();
            }
        }

        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        public virtual bool Remove(KeyValuePair<string, object> item) {
            return _backingStore.Remove(item);
        }

        protected virtual IEnumerable<KeyValuePair<string, object>> GetItems() {
            if (_baseScope == null) {
                return _backingStore;
            }
            return Enumerable.Concat(_backingStore, _baseScope).Distinct(_keyValueComparer);
        }

        private class StateStorageKeyValueComparer : IEqualityComparer<KeyValuePair<string, object>> {
            private IEqualityComparer<string> _stateStorageComparer = ScopeStorageComparer.Instance;

            public bool Equals(KeyValuePair<string, object> x, KeyValuePair<string, object> y) {
                return _stateStorageComparer.Equals(x.Key, y.Key);
            }

            public int GetHashCode(KeyValuePair<string, object> obj) {
                return _stateStorageComparer.GetHashCode(obj.Key);
            }
        }
    }
}
