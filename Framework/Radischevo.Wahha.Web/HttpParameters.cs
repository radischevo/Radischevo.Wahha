using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web
{
    /// <summary>
    /// Incapsulates the collection 
    /// of HTTP request variables.
    /// </summary>
    public class HttpParameters : IHttpValueSet
    {
        #region Instance Fields
        private IHttpValueSet _form;
		private IHttpValueSet _queryString;
		private IHttpValueSet _headers;
		private IHttpValueSet _cookies;
        #endregion

        #region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="HttpParameters"/> class
		/// </summary>
		/// <param name="request">The current HTTP request</param>
		public HttpParameters(HttpRequest request)
		{
			Precondition.Require(request, () => Error.ArgumentNull("request"));

			_queryString = request.QueryString.AsValueSet();
			_headers = request.Headers.AsValueSet();
			_form = request.Form.AsValueSet();
			_cookies = request.Cookies.AsValueSet();
		}
        #endregion

        #region Instance Properties
        public object this[string key]
        {
            get
            {
                return GetValue<object>(key, null, CultureInfo.CurrentCulture);
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
		/// Determines whether the collection contains the specified key.
		/// </summary>
		/// <param name="key">The key to find.</param>
		public bool ContainsKey(string key)
		{
			return (_form.ContainsKey(key) ||
				_queryString.ContainsKey(key) ||
				_cookies.ContainsKey(key) ||
				_headers.ContainsKey(key));				
		}

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
        /// <param name="defaultValue">The default value of the variable</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
        public TValue GetValue<TValue>(string key, TValue defaultValue, 
			IFormatProvider provider)
        {
            if (_form.ContainsKey(key))
                return _form.GetValue<TValue>(key, defaultValue, provider);

            if (_queryString.ContainsKey(key))
                return _queryString.GetValue<TValue>(key, defaultValue, provider);

            if (_cookies.ContainsKey(key))
                return _cookies.GetValue<TValue>(key, defaultValue, provider);

            if (_headers.ContainsKey(key))
                return _headers.GetValue<TValue>(key, defaultValue, provider);

            return defaultValue;
        }
        #endregion

		#region IValueSet Members
		IEnumerable<string> IValueSet.Keys
		{
			get
			{
				return Headers.Keys.Concat(Cookies.Keys)
					.Concat(QueryString.Keys)
					.Concat(Form.Keys).Distinct();
			}
		}
		#endregion
	}
}
