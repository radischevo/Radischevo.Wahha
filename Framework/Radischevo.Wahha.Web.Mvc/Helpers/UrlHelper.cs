using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Class containing convenience 
    /// methods for URL generation.
    /// </summary>
    public class UrlHelper : IHideObjectMembers
    {
        #region Instance Fields
        private ViewContext _context;
        private RouteCollection _routes;
        #endregion

        #region Constructors
        public UrlHelper(ViewContext context) : this(context, RouteTable.Routes)
        {   }

        public UrlHelper(ViewContext context, RouteCollection routes)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Require(routes, () => Error.ArgumentNull("routes"));

            _context = context;
            _routes = routes;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/>.
        /// </summary>
        public ViewContext Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Gets the collection of routes.
        /// </summary>
        public RouteCollection Routes
        {
            get
            {
                return _routes;
            }
        }
        #endregion

        #region Static Methods
        private static bool TryGetFileVersion(HttpContextBase context,
            string virtualPath, out long version)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            version = -1;

            if (!Uri.IsWellFormedUriString(virtualPath, UriKind.Relative))
                return false;

            string absolutePath = context.Server.MapPath(virtualPath);

            if (!File.Exists(absolutePath))
                return false;

            try
            {
                version = File.GetLastWriteTime(absolutePath).ToFileTimeUtc();
            }
            catch (UnauthorizedAccessException) // if the file could not be read
            {
                return false;
            }
            return true;
        }

        private static string GenerateUrl(RequestContext context, string key, 
            string controller, string action, RouteCollection routes, ValueDictionary values)
        {
            if (!String.IsNullOrEmpty(controller))
                values["controller"] = controller;

            if (!String.IsNullOrEmpty(action))
                values["action"] = action;

            VirtualPathData virtualPath = routes.GetVirtualPath(context, key, values);

            if (virtualPath == null)
                throw Error.MatchingRouteCouldNotBeLocated();

            return virtualPath.VirtualPath;
        }
        #endregion

        #region Instance Methods
        private string GenerateUrl(string key, string controller, string action, ValueDictionary values)
        {
            return GenerateUrl(_context, key, controller, action, _routes, values);
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="routeKey">The name of the route, if any.</param>
        public string Route(string routeKey)
        {
            return Route(routeKey, null, null, null);
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="values">The route values.</param>
        public string Route(object values)
        {
            return Route(null, null, null, new ValueDictionary(values));
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="routeKey">The name of the route, if any.</param>
        /// <param name="values">The route values.</param>
        public string Route(string routeKey, object values)
        {
            return Route(routeKey, null, null, new ValueDictionary(values));
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="routeKey">The name of the route, if any.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action">The name of the action.</param>
        public string Route(string routeKey, string controller, string action)
        {
            return Route(routeKey, controller, action, null);
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action">The name of the action.</param>
        public string Route(string controller, string action)
        {
            return Route(null, controller, action, null);
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action">The name of the action.</param>
        /// <param name="values">The route values.</param>
        public string Route(string controller, string action, object values)
        {
            return Route(null, controller, action, new ValueDictionary(values));
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="routeKey">The name of the route, if any.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action">The name of the action.</param>
        /// <param name="values">The route values.</param>
        public string Route(string routeKey, 
            string controller, string action, object values)
        {
            return Route(routeKey, controller, action, new ValueDictionary(values));
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="routeKey">The name of the route, if any.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action">The name of the action.</param>
        /// <param name="values">The route values.</param>
        public virtual string Route(string routeKey, string controller, string action, 
            ValueDictionary values)
        {
            if (values == null)
                values = new ValueDictionary();

            return HttpUtility.HtmlAttributeEncode(GenerateUrl(routeKey, controller, action, values));
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="action">The action with parameters.</param>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        public string Route<TController>(Expression<Action<TController>> action)
            where TController : IController
        {
            return Route<TController>(null, action);
        }

        /// <summary>
        /// Convenience method used to generate a link 
        /// using Routing to determine the virtual path.
        /// </summary>
        /// <param name="routeKey">The name of the route, if any.</param>
        /// <param name="action">The action with parameters.</param>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        public virtual string Route<TController>(string routeKey, 
            Expression<Action<TController>> action)
            where TController : IController
        {
			Precondition.Require(action, () => Error.ArgumentNull("action"));

            MethodCallExpression mexp = (action.Body as MethodCallExpression);
            if (mexp == null)
                throw Error.ExpressionMustBeAMethodCall("action");

            if (mexp.Object != action.Parameters[0])
                throw Error.MethodCallMustTargetLambdaArgument("action");

            string actionName = ActionMethodSelector.GetNameOrAlias(mexp.Method);
            string controllerName = typeof(TController).Name;

            ValueDictionary rvd = LinqHelper.ExtractArgumentsToDictionary(mexp);
            rvd = (rvd != null) ? rvd : new ValueDictionary();
            
            return HttpUtility.HtmlAttributeEncode(GenerateUrl(routeKey, controllerName, actionName, rvd));
        }

        /// <summary>
        /// Gets a resource URL for the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the resource.</param>
        public virtual string Resource(string virtualPath)
        {
            return Resource(virtualPath, false);
        }

        /// <summary>
        /// Gets a resource URL for the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the resource.</param>
        /// <param name="includeVersion">If true, the resource version 
        /// will be included in the URL.</param>
        public virtual string Resource(string virtualPath, bool includeVersion)
        {
            HttpContextBase context = _context.Context;
            long fileVersion;

            if (virtualPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
            {
                virtualPath = virtualPath.Remove(0, 2);
                string appPath = context.Request.ApplicationPath;

                if (String.IsNullOrEmpty(appPath))
                    appPath = "/";

                if (!appPath.EndsWith("/"))
                    appPath = String.Concat(appPath, "/");

                virtualPath = String.Concat(appPath, virtualPath);
            }

            if (includeVersion && TryGetFileVersion(Context.Context, virtualPath, out fileVersion))
                virtualPath += ((virtualPath.IndexOf('?') > -1) ? "&v=" : "?v=")
                    + Convert.ToString(fileVersion, CultureInfo.InvariantCulture);

            return virtualPath;
        }

        /// <summary>
        /// Gets the DNS host name of the current application 
        /// with the specified level of domain hierarchy.
        /// </summary>
        /// <param name="domainLevel">The target level of domain hierarchy.</param>
        /// <remarks>If the <paramref name="domainLevel"/> equals to zero, 
        /// the current domain is returned.</remarks>
        public string HostName(int domainLevel)
        {
            Uri uri = _context.Context.Request.Url;
            string currentDomain = uri.Host;

            if (domainLevel < 1)
                return currentDomain;
            
            if (uri.HostNameType != UriHostNameType.Dns)
                return currentDomain;

            string[] subDomains = currentDomain.Split(new char[] { '.' }, 
                StringSplitOptions.RemoveEmptyEntries);

            if (subDomains.Length <= domainLevel)
                return currentDomain;

            int startIndex = subDomains.Length - domainLevel;
            int count = subDomains.Length - startIndex;

            return String.Join(".", subDomains, startIndex, count);
        }

        /// <summary>
        /// Converts the specified virtual path to 
        /// an absolute URL.
        /// </summary>
        /// <param name="virtualPath">The virtual path to convert.</param>
        public string MakeAbsolute(string virtualPath)
        {
            return MakeAbsolute(virtualPath, 0, false);
        }

        /// <summary>
        /// Converts the specified virtual path to 
        /// an absolute URL.
        /// </summary>
        /// <param name="virtualPath">The virtual path to convert.</param>
        /// <param name="includeVersion">If true, the resource version 
        /// will be included in the URL.</param>
        public string MakeAbsolute(string virtualPath, bool includeVersion)
        {
            return MakeAbsolute(virtualPath, 0, includeVersion);
        }

        /// <summary>
        /// Converts the specified virtual path to 
        /// an absolute URL.
        /// </summary>
        /// <param name="virtualPath">The virtual path to convert.</param>
        /// <param name="domainLevel">The target level of domain hierarchy.</param>
        /// <remarks>If the <paramref name="domainLevel"/> equals to zero, 
        /// the current domain is returned.</remarks>
        public string MakeAbsolute(string virtualPath, int domainLevel)
        {
            return MakeAbsolute(virtualPath, domainLevel, false);
        }

        /// <summary>
        /// Converts the specified virtual path to 
        /// an absolute URL.
        /// </summary>
        /// <param name="virtualPath">The virtual path to convert.</param>
        /// <param name="domainLevel">The target level of domain hierarchy.</param>
        /// <param name="includeVersion">If true, the resource version 
        /// will be included in the URL.</param>
        /// <remarks>If the <paramref name="domainLevel"/> equals to zero, 
        /// the current domain is returned.</remarks>
        public virtual string MakeAbsolute(string virtualPath, int domainLevel, bool includeVersion)
        {
            if (virtualPath.Contains(Uri.SchemeDelimiter))
                return virtualPath;

            Uri uri = _context.Context.Request.Url;

            string absolutePath = (virtualPath.StartsWith("~/",
                StringComparison.OrdinalIgnoreCase)) ? HostName(domainLevel) +
                    Resource(virtualPath, includeVersion) : Resource(virtualPath, includeVersion);

            return String.Format("{0}{1}{2}", uri.Scheme,
                Uri.SchemeDelimiter, absolutePath);
        }
        #endregion
    }
}
