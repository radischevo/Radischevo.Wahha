using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Radischevo.Wahha.Core
{
	internal sealed class PrefixWrapper : IValueSet
	{
		#region Instance Fields
        private IValueSet _collection;
		private IDictionary<string, string> _map;
		private StringComparison _comparison;
		private string _prefix;
        #endregion

        #region Constructors
        public PrefixWrapper(IValueSet collection, string prefix, bool ignoreCase)
        {
			Precondition.Require(collection, () => Error.ArgumentNull("collection"));
			Precondition.Defined(prefix, () => Error.ArgumentNull("prefix"));

            _collection = collection;
            _prefix = prefix;
			_comparison = (ignoreCase)
				? StringComparison.OrdinalIgnoreCase 
				: StringComparison.Ordinal;
        }
        #endregion

        #region Instance Properties
		private IDictionary<string, string> Map
		{
			get
			{
				if (_map == null)
					_map = _collection.Keys
						.Where(a => a.StartsWith(_prefix, _comparison))
						.ToDictionary(
							a => a.Substring(_prefix.Length),
							k => k, StringComparer.OrdinalIgnoreCase
						);

				return _map;
			}
		}
		
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
                return Map.Keys;
            }
        }
        #endregion

        #region Instance Methods
        public TValue GetValue<TValue>(string key, TValue defaultValue, 
			IFormatProvider provider)
        {
            string mappedKey;
			if(Map.TryGetValue(key, out mappedKey))
				return _collection.GetValue<TValue>(mappedKey, defaultValue, provider);

			return defaultValue;
        }
        #endregion
	}
}

