using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Templates.Scope {
    public class StaticScopeStorageProvider : IScopeStorageProvider {
        private static readonly IDictionary<string, object> _defaultContext =
            new ScopeStorageDictionary(null, new Dictionary<string, object>(ScopeStorageComparer.Instance));
        private IDictionary<string, object> _currentContext;

        public IDictionary<string, object> CurrentScope {
            get {
                return _currentContext ?? _defaultContext;
            }
            set {
                _currentContext = value;
            }
        }

        public IDictionary<string, object> GlobalScope {
            get {
                return _defaultContext;
            }
        }
    }
}
