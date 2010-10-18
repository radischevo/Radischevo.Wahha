using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Radischevo.Wahha.Core
{
    public static class ValueSetExtensions
    {
        #region Extension Methods
		/// <summary>
		/// Gets the typed value with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="key">The key to find.</param>
		public static TValue GetValue<TValue>(this IValueSet values, string name)
		{
			return values.GetValue<TValue>(name, default(TValue), CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Gets the typed value with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="key">The key to find.</param>
		/// <param name="defaultValue">The default value of the variable.</param>
		public static TValue GetValue<TValue>(this IValueSet values, string name, TValue defaultValue)
		{
			return values.GetValue<TValue>(name, defaultValue, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Gets the typed value with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="key">The key to find.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
		public static TValue GetValue<TValue>(this IValueSet values, string name, IFormatProvider provider)
		{
			return values.GetValue<TValue>(name, default(TValue), provider);
		}
		
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
			if (keys == null || keys.Length < 1)
				return ContainsAny(values);

			foreach (string key in keys)
				if (values.Keys.Contains(key, comparer))
					return true;

			return false;
		}
        #endregion
    }
}
