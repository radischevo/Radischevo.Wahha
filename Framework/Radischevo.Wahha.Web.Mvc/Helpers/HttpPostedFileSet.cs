using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	internal class HttpPostedFileSet : IValueSet
	{
		#region Instance Fields
		private HttpFileCollectionBase _collection;
		private HashSet<string> _keys;
		#endregion

		#region Constructors
		public HttpPostedFileSet(HttpFileCollectionBase collection)
		{
			Precondition.Require(collection, 
				Error.ArgumentNull("collection"));

			_collection = collection;
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
				return _collection[key];
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
		public TValue GetValue<TValue>(string key)
		{
			return GetValue<TValue>(key, default(TValue));
		}

		public TValue GetValue<TValue>(string key, TValue defaultValue)
		{
			Type type = typeof(TValue);

			if (!type.Equals(typeof(HttpPostedFileBase)))
				throw Error.HttpPostedFileSetTypeLimitations(type);

			object value = _collection[key];
			if (value == null)
				return defaultValue;

			return (TValue)value;
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
