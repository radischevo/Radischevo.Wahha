using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Radischevo.Wahha.Core
{
    public static class ValueSet
	{
		#region Nested Types
		private sealed class EmptyValueSet : IValueSet
		{
			#region Static Fields
			private static readonly IEnumerable<string> _emptyKeys = new string[0];
			#endregion

			#region Constructors
			public EmptyValueSet()
			{
			}
			#endregion

			#region Instance Properties
			public object this[string key]
			{
				get
				{
					return null;
				}
			}

			public IEnumerable<string> Keys
			{
				get
				{
					return _emptyKeys;
				}
			}
			#endregion

			#region Instance Methods
			public TValue GetValue<TValue>(string key, 
				TValue defaultValue, IFormatProvider provider)
			{
				return defaultValue;
			}
			#endregion
		}
		#endregion

		#region Static Fields
		private static readonly IValueSet _empty = new EmptyValueSet();
		#endregion

		#region Static Properties
		public static IValueSet Empty
		{
			get
			{
				return _empty;
			}
		}
		#endregion

		#region Extension Methods
		/// <summary>
		/// Gets the typed value with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="key">The key to find.</param>
		public static TValue GetValue<TValue>(this IValueSet values, string name)
		{
			Precondition.Require(values, () => Error.ArgumentNull("values"));
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
			Precondition.Require(values, () => Error.ArgumentNull("values"));
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
			Precondition.Require(values, () => Error.ArgumentNull("values"));
			return values.GetValue<TValue>(name, default(TValue), provider);
		}
		
        public static IValueSet Subset(this IValueSet values, Func<string, bool> keySelector)
        {
			return new SubsetWrapper(values, keySelector);
        }
		
		public static IValueSet Subset(this IValueSet values, string prefix)
		{
			return Subset (values, prefix, true);
		}
		
		public static IValueSet Subset(this IValueSet values, string prefix, bool ignoreCase)
		{
			if (String.IsNullOrEmpty(prefix))
				return values;
			
			return new PrefixWrapper(values, prefix, ignoreCase);
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
			Precondition.Require(values, () => Error.ArgumentNull("values"));

			foreach (string key in keys)
				if (!values.Keys.Contains(key, comparer))
					return false;

			return true;
		}
		
		public static bool ContainsAny(this IValueSet values)
		{
			Precondition.Require(values, () => Error.ArgumentNull("values"));
			return values.Keys.Any();
		}
		
		public static bool ContainsAny(this IValueSet values, string key)
		{
			return ContainsAny(values, StringComparer.OrdinalIgnoreCase, key);
		}
		
		public static bool ContainsAny(this IValueSet values, 
			IEqualityComparer<string> comparer, string key)
		{
			Precondition.Require(values, () => Error.ArgumentNull("values"));
			return values.Keys.Contains(key, comparer);
		}
		
		public static bool ContainsAny(this IValueSet values, params string[] keys)
		{
			return ContainsAny(values, StringComparer.OrdinalIgnoreCase, keys);
		}

		public static bool ContainsAny(this IValueSet values, 
			IEqualityComparer<string> comparer, params string[] keys)
		{
			Precondition.Require(values, () => Error.ArgumentNull("values"));

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
