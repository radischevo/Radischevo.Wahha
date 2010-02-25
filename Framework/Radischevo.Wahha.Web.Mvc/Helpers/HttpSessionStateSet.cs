﻿using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	internal class HttpSessionStateSet : IValueSet
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
                return GetValue<object>(key);
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
        public TValue GetValue<TValue>(string key)
        {
            return GetValue<TValue>(key, default(TValue));
        }

        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            if (!ContainsKey(key))
                return defaultValue;

			object value = _session[key];

            if (typeof(TValue) == typeof(bool))
            {
                switch (value.ToString().ToLower())
                {
                    case "on":
                    case "yes":
                    case "true":
                        value = true;
                        break;
                    default:
                        value = false;
                        break;
                }
            }
            return Converter.ChangeType<TValue>(value, defaultValue);
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
