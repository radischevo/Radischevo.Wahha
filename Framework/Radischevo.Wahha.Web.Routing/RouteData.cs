using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Represents the dictionary storing 
    /// the route parameter values
    /// </summary>
    public class RouteData
    {
        #region Instance Fields
        private ValueDictionary _values;
        private ValueDictionary _tokens;
        private RouteBase _route;
        private IRouteHandler _handler;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of 
        /// the <see cref="RouteData"/> class
        /// </summary>
        public RouteData()
        {
            _values = new ValueDictionary();
            _tokens = new ValueDictionary();
        }

        /// <summary>
        /// Initializes a new instance of 
        /// the <see cref="RouteData"/> class
        /// </summary>
        /// <param name="route">The currently executed route</param>
        /// <param name="handler">The <see cref="IRouteHandler"/> used 
        /// to handle the request</param>
        public RouteData(RouteBase route, 
            IRouteHandler handler)
        {
            _values = new ValueDictionary();
            _tokens = new ValueDictionary();
            _route = route;
            _handler = handler;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the route parameter 
        /// values dictionary
        /// </summary>
        public ValueDictionary Values
        {
            get
            {
                return _values;
            }
        }

        public ValueDictionary Tokens
        {
            get
            {
                return _tokens;
            }
        }

        /// <summary>
        /// Gets or sets the route 
        /// which is executed for current request
        /// </summary>
        public RouteBase Route
        {
            get
            {
                return _route;
            }
            set
            {
                _route = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRouteHandler"/> 
        /// which handles the current request
        /// </summary>
        public IRouteHandler Handler
        {
            get
            {
                return _handler;
            }
            set
            {
                _handler = value;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets the value of the route parameter 
        /// with the specified name
        /// </summary>
        /// <param name="key">The name of the 
        /// parameter to find</param>
        public object GetValue(string key)
        {
            if (_values.ContainsKey(key))
                return _values[key];

            return null;
        }

        /// <summary>
        /// Gets the typed value of the route parameter 
        /// with the specified name
        /// </summary>
        /// <typeparam name="TValue">The type of the 
        /// parameter value</typeparam>
        /// <param name="key">The name of the 
        /// parameter to find</param>
        public TValue GetValue<TValue>(string key)
        {
            return _values.GetValue<TValue>(key);
        }

        /// <summary>
        /// Gets the typed value of the route parameter 
        /// with the specified name.
        /// </summary>
        /// <typeparam name="TValue">The type of the 
        /// parameter value.</typeparam>
        /// <param name="key">The name of the 
        /// parameter to find.</param>
        /// <param name="defaultValue">The value to return if no parameter 
        /// with the specified key exists.</param>
        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            return _values.GetValue<TValue>(key, defaultValue);
        }

        /// <summary>
        /// Gets the typed value of the route parameter 
        /// with the specified name. If no parameter with this 
        /// name found, an exception will be thrown.
        /// </summary>
        /// <typeparam name="TValue">The type of the 
        /// parameter value</typeparam>
        /// <param name="key">The name of the 
        /// parameter to find</param>
        public TValue GetRequiredValue<TValue>(string key)
        {
            if (_values.ContainsKey(key))
                return Converter.ChangeType<TValue>(_values[key]);

            throw Error.RequiredValueNotFound(key);
        }
        #endregion
    }
}
