using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Configuration;
using Radischevo.Wahha.Core;
using System;

namespace Radischevo.Wahha.Web.Templates.Scope {
    internal class WebConfigScopeDictionary : IDictionary<string, object> {
        private readonly Link<Dictionary<string, object>> _items;

        public WebConfigScopeDictionary()
            : this(WebConfigurationManager.AppSettings) {
        }

        public WebConfigScopeDictionary(NameValueCollection appSettings) {
            _items = new Link<Dictionary<string, object>>(
                () => appSettings.AllKeys.ToDictionary(key => key, key => (object)appSettings[key], ScopeStorageComparer.Instance)
            );
        }

        public object this[string key] {
            get {
                object value;
                TryGetValue(key, out value);
                return value;
            }
            set {
                throw new NotSupportedException("WebPageResources.StateStorage_ScopeIsReadOnly");
            }
        }

        private IDictionary<string, object> Items {
            get {
                return _items.Value;
            }
        }

        public bool TryGetValue(string key, out object value) {
            return Items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(string key, object value) {
            throw new NotSupportedException("WebPageResources.StateStorage_ScopeIsReadOnly");
        }

        public bool ContainsKey(string key) {
            return Items.ContainsKey(key);
        }

        public ICollection<string> Keys {
            get {
                return Items.Keys;
            }
        }

        public bool Remove(string key) {
            throw new NotSupportedException("WebPageResources.StateStorage_ScopeIsReadOnly");
        }

        public ICollection<object> Values {
            get {
                return Items.Values;
            }
        }

        public void Add(KeyValuePair<string, object> item) {
            throw new NotSupportedException("WebPageResources.StateStorage_ScopeIsReadOnly");
        }

        public void Clear() {
            throw new NotSupportedException("WebPageResources.StateStorage_ScopeIsReadOnly");
        }

        public bool Contains(KeyValuePair<string, object> item) {
            return Items.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
            Items.CopyTo(array, arrayIndex);
        }

        public int Count {
            get {
                return Items.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return true;
            }
        }

        public bool Remove(KeyValuePair<string, object> item) {
            throw new NotSupportedException("WebPageResources.StateStorage_ScopeIsReadOnly");
        }
    }
}
