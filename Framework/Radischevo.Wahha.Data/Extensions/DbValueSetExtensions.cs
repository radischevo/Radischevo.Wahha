using System;
using System.Collections.Generic;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public static class DbValueSetExtensions
	{
		#region Nested Types
		private sealed class DbValueSet : IDbValueSet
		{
			#region Instance Fields
			private IValueSet _collection;
			private HashSet<string> _accessedKeys;
			#endregion

			#region Constructors
			public DbValueSet(IValueSet collection)
			{
				Precondition.Require(collection, () => 
					Error.ArgumentNull("collection"));

				_collection = collection;
				_accessedKeys = new HashSet<string>();
			}
			#endregion

			#region Instance Properties
			public IEnumerable<string> AccessedKeys
			{
				get
				{
					return _accessedKeys;
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
					return _collection.Keys;
				}
			}
			#endregion

			#region Instance Methods
			private void MarkAccessedKey(string key)
			{
				if (!_accessedKeys.Contains(key))
					_accessedKeys.Add(key);
			}

			public TValue GetValue<TValue>(string key, TValue defaultValue, IFormatProvider provider)
			{
				MarkAccessedKey(key);
				return _collection.GetValue<TValue>(key, defaultValue, provider);
			}
			#endregion
		}
		#endregion

		#region Extension Methods
		public static IDbValueSet ToDbValueSet(this IValueSet values)
		{
			return new DbValueSet(values);
		}

		public static IDbValueSet Subset(this IDbValueSet values, Func<string, bool> keySelector)
		{
			return ToDbValueSet(ValueSet.Subset(values, keySelector));
		}

		public static IDbValueSet Transform(this IDbValueSet values, Func<string, string> keyTransformer)
		{
			return ToDbValueSet(ValueSet.Transform(values, keyTransformer));
		}
		#endregion
	}
}
