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
			foreach (string key in keys)
				if (!values.Keys.Contains(key))
					return false;

			return true;
		}

		public static bool ContainsAny(this IValueSet values, params string[] keys)
		{
			foreach (string key in keys)
				if (values.Keys.Contains(key))
					return true;

			return false;
		}
        #endregion
    }
}
