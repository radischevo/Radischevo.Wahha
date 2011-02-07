using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web
{
	internal sealed class HttpCollectionWrapper<TCollection> : IHttpValueSet
		where TCollection : NameObjectCollectionBase
	{
		#region Instance Fields
		private TCollection _collection;
		private HashSet<string> _keys;
		private Func<TCollection, string, object> _selector;
		#endregion

		#region Constructors
		public HttpCollectionWrapper(TCollection collection,
			Func<TCollection, string, object> selector)
		{
			Precondition.Require(collection, () => Error.ArgumentNull("collection"));
			Precondition.Require(selector, () => Error.ArgumentNull("selector"));

			_collection = collection;
			_selector = selector;
		}
		#endregion

		#region Instance Properties
		private HashSet<string> Keys
		{
			get
			{
				if (_keys == null)
					_keys = new HashSet<string>(_collection.Keys.Cast<string>(),
						StringComparer.InvariantCultureIgnoreCase);

				return _keys;
			}
		}

		public object this[string key]
		{
			get
			{
				return GetValue<object>(key, null, CultureInfo.CurrentCulture);
			}
		}

		public int Count
		{
			get
			{
				return _collection.Count;
			}
		}
		#endregion

		#region Instance Methods
		public TValue GetValue<TValue>(string key, TValue defaultValue,
			IFormatProvider provider)
		{
			if (!ContainsKey(key))
				return defaultValue;

			object value = _selector(_collection, key);
			return Converter.ChangeType<TValue>(value, defaultValue);
		}

		public IEnumerable<TValue> GetValues<TValue>(string key)
		{
			return HttpValueCollectionConverter.Convert<TValue>(GetValue<string>(key, 
				null, CultureInfo.CurrentCulture));
		}

		public bool ContainsKey(string key)
		{
			return Keys.Contains(key);
		}

		public bool ContainsAll(params string[] keys)
		{
			foreach (string key in keys)
			{
				if (!ContainsKey(key))
					return false;
			}
			return true;
		}

		public bool ContainsAny(params string[] keys)
		{
			foreach (string key in keys)
			{
				if (ContainsKey(key))
					return true;
			}
			return false;
		}
		#endregion

		#region IValueSet Members
		IEnumerable<string> IValueSet.Keys
		{
			get
			{
				return Keys;
			}
		}
		#endregion
	}
}
