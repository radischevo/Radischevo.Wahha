using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Core
{
    public static class ValueSetExtensions
    {
        #region Extension Methods
        public static IValueSet Subset(this IValueSet values, Func<string, bool> keySelector)
        {
            return new SubsetWrapper(values, keySelector);
        }

        public static IValueSet Transform(this IValueSet values, Func<string, string> keyTransformer)
        {
            return new TransformWrapper(values, keyTransformer);
        }

		public static bool ContainsAll(this IValueSet values, params string[] keys)
		{
			return ContainsAll(values, StringComparer.OrdinalIgnoreCase, keys);
		}

		public static bool ContainsAll(this IValueSet values, 
			IEqualityComparer<string> comparer, params string[] keys)
		{
			foreach (string key in keys)
				if (!values.Keys.Contains(key, comparer))
					return false;

			return true;
		}

		public static bool ContainsAny(this IValueSet values)
		{
			return values.Keys.Any();
		}

		public static bool ContainsAny(this IValueSet values, params string[] keys)
		{
			return ContainsAny(values, StringComparer.OrdinalIgnoreCase, keys);
		}

		public static bool ContainsAny(this IValueSet values, 
			IEqualityComparer<string> comparer, params string[] keys)
		{
			foreach (string key in keys)
				if (values.Keys.Contains(key, comparer))
					return true;

			return false;
		}
        #endregion
    }
}
