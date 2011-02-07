using System;
using System.Collections.Generic;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Represents a single route
    /// </summary>
    public class Route : RouteBase
    {
        #region Instance Fields
        private IRouteHandler _handler;
        private string _url;
        private ParsedRoute _parsedRoute;
        private ValueDictionary _defaults;
        private ValueDictionary _tokens;
        private List<IRouteConstraint> _constraints;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Route"/> class
        /// </summary>
        /// <param name="url">The pattern of the URL to route</param>
        /// <param name="handler">The <see cref="IRouteHandler"/>, which is used 
        /// to handle a request</param>
        public Route(string url, IRouteHandler handler)
            : this(url, null, null, null, handler)
        {   }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Route"/> class
        /// </summary>
        /// <param name="url">The pattern of the URL to route</param>
        /// <param name="defaults">Dictionary, containing default values of route 
        /// parameters</param>
        /// <param name="handler">The <see cref="IRouteHandler"/>, which is used 
        /// to handle a request</param>
        public Route(string url, ValueDictionary defaults, IRouteHandler handler)
            : this(url, defaults, null, null, handler)
        {   }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Route"/> class
        /// </summary>
        /// <param name="url">The pattern of the URL to route</param>
        /// <param name="defaults">Dictionary, containing default values of route 
        /// parameters</param>
        /// <param name="constraints">Collection of route constraints</param>
        /// <param name="handler">The <see cref="IRouteHandler"/>, which is used 
        /// to handle a request</param>
        public Route(string url, ValueDictionary defaults, 
            IEnumerable<IRouteConstraint> constraints, IRouteHandler handler)
            : this(url, defaults, constraints, null, handler)
        {   }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Route"/> class
        /// </summary>
        /// <param name="url">The pattern of the URL to route</param>
        /// <param name="defaults">Dictionary, containing default values of route 
        /// parameters</param>
        /// <param name="constraints">Collection of route constraints</param>
        /// <param name="tokens">Dictionary, containing additional route data</param>
        /// <param name="handler">The <see cref="IRouteHandler"/>, which is used 
        /// to handle a request</param>
        public Route(string url, ValueDictionary defaults,
            IEnumerable<IRouteConstraint> constraints, 
            ValueDictionary tokens, IRouteHandler handler)
        {
            _defaults = defaults;
            _constraints = (constraints == null) ? null :
                new List<IRouteConstraint>(constraints);
            _handler = handler;
            _tokens = tokens;
            Url = url;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets or sets the pattern of an 
        /// URL to be routed
        /// </summary>
        public string Url
        {
            get
            {
                return (_url == null) ? String.Empty : _url;
            }
            set
            {
                _parsedRoute = RouteParser.Parse(value);
                _url = value;
            }
        }

        /// <summary>
        /// Gets the default values 
        /// of route parameters
        /// </summary>
        public ValueDictionary Defaults
        {
            get
            {
                if (_defaults == null)
                    _defaults = new ValueDictionary();

                return _defaults;
            }
        }

        /// <summary>
        /// Gets the collection of data tokens
        /// </summary>
        public ValueDictionary Tokens
        {
            get
            {
                if (_tokens == null)
                    _tokens = new ValueDictionary();

                return _tokens;
            }
        }

        /// <summary>
        /// Gets the collection of route constraints
        /// </summary>
        public List<IRouteConstraint> Constraints
        {
            get
            {
                if (_constraints == null)
                    _constraints = new List<IRouteConstraint>();

                return _constraints;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRouteHandler"/>, which 
        /// is used for current request handling
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

        #region Static Methods
        private static string GetApplicationPath(HttpContextBase context)
        {
            if (context == null)
                return String.Empty;

            return context.Request.ApplicationPath.Define().TrimEnd('/');
        }

		private static string GetExecutionPath(HttpContextBase context)
		{
			if (context == null)
				return String.Empty;

			return (String.IsNullOrEmpty(context.Request.ApplicationPath)) ? 
				context.Request.CurrentExecutionFilePath : 
				context.Request.AppRelativeCurrentExecutionFilePath.Substring(1);
		}
        #endregion

        #region Instance Methods
        private bool MatchConstraints(HttpContextBase context, 
			ValueDictionary values, RouteDirection direction)
        {
            foreach (IRouteConstraint constraint in Constraints)
            {
                if (!MatchConstraint(context, constraint, values, direction))
                    return false;
            }
            return true;
        }

        protected virtual bool MatchConstraint(HttpContextBase context, 
            IRouteConstraint constraint, ValueDictionary values, 
			RouteDirection direction)
        {
            return constraint.Match(context, this, values, direction);
        }

        /// <summary>
        /// Gets the <see cref="VirtualPathData"/> for 
        /// the current instance.
        /// </summary>
        /// <param name="values">The <see cref="ValueDictionary"/> 
        /// containing the route parameter values.</param>
		/// <param name="variables">The <see cref="ValueDictionary"/> 
		/// containing the route variable values.</param>
        public override VirtualPathData GetVirtualPath(RequestContext context,
			ValueDictionary values, ValueDictionary variables)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            HttpContextBase httpContext = context.Context;
            HttpRequestBase request = httpContext.Request;

            BoundUrl url = _parsedRoute.Bind(context.RouteData.Values, values, variables, Defaults);
            if (url == null)
                return null;
            
            if (!MatchConstraints(httpContext, url.Values, RouteDirection.UrlGeneration))
                return null;

            string appRelativePath = url.Url;
            string applicationPath = GetApplicationPath(httpContext);

            string virtualPath = (_parsedRoute.IsAppRelative) ? 
                httpContext.Response.ApplyAppPathModifier(
                    String.Concat(applicationPath, appRelativePath)) : appRelativePath;

            string absolutePath = (_parsedRoute.IsRelative) ? 
                String.Concat(request.Url.Scheme, Uri.SchemeDelimiter, request.Url.Authority, virtualPath) : 
                (virtualPath.Contains(Uri.SchemeDelimiter)) ? virtualPath : 
                    String.Concat(request.Url.Scheme, Uri.SchemeDelimiter, virtualPath);

            VirtualPathData data = new VirtualPathData(this, absolutePath);

            if(_tokens != null)
                foreach (KeyValuePair<string, object> kvp in _tokens)
                    data.Tokens[kvp.Key] = kvp.Value;
            
            return data;
        }

        /// <summary>
        /// Gets the <see cref="RouteData"/> for 
        /// the current <see cref="HttpContextBase"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> 
        /// containing the incoming request.</param>
		/// <param name="variables">The <see cref="ValueDictionary"/> 
		/// containing the route variable values.</param>
        public override RouteData GetRouteData(HttpContextBase context, 
			ValueDictionary variables)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));

            string applicationPath = GetApplicationPath(context);
            string appRelativePath = String.Concat(GetExecutionPath(context), context.Request.PathInfo);

            string virtualPath = (_parsedRoute.IsAppRelative) ? appRelativePath :
                String.Concat(applicationPath, appRelativePath);
            string path = (_parsedRoute.IsRelative) ? virtualPath :
                String.Concat(context.Request.Url.Authority, virtualPath);

            ValueDictionary values = _parsedRoute.Match(path, variables, Defaults);
            
            if (values == null)
                return null;
            
            RouteData data = new RouteData(this, _handler);
            if (!MatchConstraints(context, values, RouteDirection.IncomingRequest))
                return null;
            
            foreach (KeyValuePair<string, object> kvp in values)
                data.Values.Add(kvp.Key, kvp.Value);
            
            if (_tokens != null)          
                foreach (KeyValuePair<string, object> kvp in _tokens)
                    data.Tokens[kvp.Key] = kvp.Value;
            
            return data;
        }
        #endregion
    }
}
