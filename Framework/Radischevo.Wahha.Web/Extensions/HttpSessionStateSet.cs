using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web
{
	internal sealed class HttpSessionStateSet : IHttpValueSet
	{
		#region Instance Fields
		private HttpSessionStateBase _session;
		private HashSet<string> _keys;
		#endregion

		#region Constructors
		public HttpSessionStateSet(HttpSessionStateBase session)
		{
			_session = session;
		}
		#endregion

		#region Instance Properties
		private HashSet<string> Keys
		{
			get
			{
				if (_keys == null)
				{
					_keys = (_session == null)
						? new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
						: new HashSet<string>(_session.Keys.Cast<string>(),
							StringComparer.InvariantCultureIgnoreCase);
				}
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
				return _session.Count;
			}
		}
		#endregion

		#region Instance Methods
		public TValue GetValue<TValue>(string key, TValue defaultValue,
			IFormatProvider provider)
		{
			if (!ContainsKey(key))
				return defaultValue;

			object value = _session[key];
			return Converter.ChangeType<TValue>(value, defaultValue, provider);
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