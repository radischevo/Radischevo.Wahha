using System.Collections.Generic;
using System;

namespace Radischevo.Wahha.Web.Templates.Scope {
    public static class ScopeStorage {
        private static IScopeStorageProvider _stateStorageProvider;
        private static readonly IScopeStorageProvider _defaultStorageProvider = new StaticScopeStorageProvider();

        public static IScopeStorageProvider CurrentProvider {
            get {
                return _stateStorageProvider ?? _defaultStorageProvider;
            }
            set {
                _stateStorageProvider = value;
            }
        }

        public static IDictionary<string, object> CurrentScope {
            get {
                return CurrentProvider.CurrentScope;
            }
        }

        public static IDictionary<string, object> GlobalScope {
            get {
                return CurrentProvider.GlobalScope;
            }
        }

        public static IDisposable CreateTransientScope(IDictionary<string, object> context) {
            var currentContext = CurrentScope;
            CurrentProvider.CurrentScope = context;
            return new DisposableAction(() => CurrentProvider.CurrentScope = currentContext); // Return an IDisposable that pops the item back off
        }

        public static IDisposable CreateTransientScope() {
            return CreateTransientScope(new ScopeStorageDictionary(CurrentScope));
        }
    }
}
