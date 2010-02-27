using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web
{
    /// <summary>
    /// Incapsulates the collection 
    /// of HTTP request variables
    /// </summary>
    public class HttpParameters
    {
        #region Nested Types
        private sealed class CollectionWrapper<TCollection> : IHttpValueSet
            where TCollection : NameObjectCollectionBase
        {
            #region Instance Fields
            private TCollection _collection;
            private HashSet<string> _keys;
            private Func<TCollection, string, object> _selector;
            #endregion

            #region Constructors
            public CollectionWrapper(TCollection collection, 
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
                    return GetValue<object>(key);
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
                if (!ContainsKey(key))
                    return defaultValue;

                object value = _selector(_collection, key);

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

			public IEnumerable<TValue> GetValues<TValue>(string key)
			{
				List<TValue> list = new List<TValue>();
				string keyValue = GetValue<string>(key);

				if (keyValue == null)
					return list;

				string[] values = keyValue.Split(new char[] { ',' },
					StringSplitOptions.RemoveEmptyEntries);

				foreach (string sv in values)
				{
					object value = sv;
					if (typeof(TValue) == typeof(bool))
					{
						switch (sv.ToLower())
						{
							case "on":
							case "yes":
								value = true;
								break;
							case "off":
							default:
								value = false;
								break;
						}
					}
					list.Add(Converter.ChangeType<TValue>(value, default(TValue)));
				}
				return list;
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
        #endregion

        #region Instance Fields
        private CollectionWrapper<NameValueCollection> _form;
        private CollectionWrapper<NameValueCollection> _queryString;
        private CollectionWrapper<NameValueCollection> _headers;
        private CollectionWrapper<HttpCookieCollection> _cookies;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="RequestParameters"/> class
        /// </summary>
        /// <param name="request">The current HTTP request</param>
        public HttpParameters(HttpRequest request)
        {
            Precondition.Require(request, () => Error.ArgumentNull("request"));

            _queryString = new CollectionWrapper<NameValueCollection>(
                request.QueryString, (col, key) => col[key]);
            _headers = new CollectionWrapper<NameValueCollection>(
                request.Headers, (col, key) => col[key]);
            _form = new CollectionWrapper<NameValueCollection>(
                request.Form, (col, key) => col[key]);
            _cookies = new CollectionWrapper<HttpCookieCollection>(
                request.Cookies, (col, key) => { 
                    HttpCookie c = col[key]; return (c == null) ? null : c.Value; 
                });
        }
        #endregion

        #region Instance Properties
        public object this[string key]
        {
            get
            {
                return GetValue<object>(key);
            }
        }

        public IHttpValueSet QueryString
        {
            get
            {
                return _queryString;
            }
        }

        public IHttpValueSet Form
        {
            get
            {
                return _form;
            }
        }

        public IHttpValueSet Headers
        {
            get
            {
                return _headers;
            }
        }

        public IHttpValueSet Cookies
        {
            get
            {
                return _cookies;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets the typed list of 
        /// values from the specified 
        /// query string or form variable
        /// </summary>
        /// <typeparam name="TValue">The type of the list entry</typeparam>
        /// <param name="key">The name of a variable</param>
        public IEnumerable<TValue> GetValues<TValue>(string key)
        {
			if (_form.ContainsKey(key))
				return _form.GetValues<TValue>(key);

			if (_queryString.ContainsKey(key))
				return _queryString.GetValues<TValue>(key);

			if (_cookies.ContainsKey(key))
				return _cookies.GetValues<TValue>(key);

			if (_headers.ContainsKey(key))
				return _headers.GetValues<TValue>(key);

			return Enumerable.Empty<TValue>();
        }

        /// <summary>
        /// Gets the typed value of the 
        /// specified query string, form or 
        /// header variable
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="key">The name of the variable</param>
        public TValue GetValue<TValue>(string key)
        {
            return GetValue<TValue>(key, default(TValue));
        }

        /// <summary>
        /// Gets the typed value of the 
        /// specified query string, form or 
        /// header variable
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="key">The name of the variable</param>
        /// <param name="defaultValue">The default value of the variable</param>
        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            if (_form.ContainsKey(key))
                return _form.GetValue<TValue>(key, defaultValue);

            if (_queryString.ContainsKey(key))
                return _queryString.GetValue<TValue>(key, defaultValue);

            if (_cookies.ContainsKey(key))
                return _cookies.GetValue<TValue>(key, defaultValue);

            if (_headers.ContainsKey(key))
                return _headers.GetValue<TValue>(key, defaultValue);

            return defaultValue;
        }
        #endregion
    }

    /// <summary>
    /// Provides extension methods for the 
    /// <see cref="System.Web.HttpRequest"/> class.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Gets the combined HTTP value collection 
        /// for the current <see cref="System.Web.HttpRequest"/>.
        /// </summary>
        /// <param name="request">The request instance to extend.</param>
        public static HttpParameters Parameters(this HttpRequest request)
        {
            return new HttpParameters(request);
        }
    }
}
