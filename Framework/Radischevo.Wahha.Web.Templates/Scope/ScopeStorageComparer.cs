using System.Collections.Generic;
using System;

namespace Radischevo.Wahha.Web.Templates.Scope {
    /// <summary>
    /// Custom comparer for the context dictionaries
    /// The comparer treats strings as a special case, performing case insesitive comparison. 
    /// This guaratees that we remain consistent throughout the chain of contexts since PageData dictionary 
    /// behaves in this manner.
    /// </summary>
    internal class ScopeStorageComparer : IEqualityComparer<string> {
        private readonly IEqualityComparer<string> _defaultComparer = StringComparer.OrdinalIgnoreCase;
        private static IEqualityComparer<string> _instance;

        public static IEqualityComparer<string> Instance {
            get {
                if (_instance == null) {
                    _instance = new ScopeStorageComparer();
                }
                return _instance;
            }
        }

        private ScopeStorageComparer() { }

        public new bool Equals(string x, string y) {
            return _defaultComparer.Equals(x, y);
        }

        public int GetHashCode(string obj) {
            return _defaultComparer.GetHashCode(obj);
        }
    }
}
