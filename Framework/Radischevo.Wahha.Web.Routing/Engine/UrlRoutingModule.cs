using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Implements an <see cref="IHttpModule"/> 
    /// for the URL routing
    /// </summary>
    public class UrlRoutingModule : IHttpModule
    {
        #region Nested Types
        private class RequestData
        {
            #region Instance Fields
            private IHttpHandler _handler;
            private string _originalPath;
            #endregion

            #region Constructors
            public RequestData(IHttpHandler handler, 
                string originalPath)
            {
                _handler = handler;
                _originalPath = originalPath;
            }
            #endregion

            #region Instance Properties
            public IHttpHandler Handler
            {
                get
                {
                    return _handler;
                }
            }

            public string OriginalPath
            {
                get
                {
                    return _originalPath;
                }
            }
            #endregion
        }
        #endregion

        #region Static Fields
        private const string _requestDataKey = "Radischevo.Wahha.Web.Routing.RequestData";
        #endregion

        #region Instance Fields
        private RouteCollection _routes;
        #endregion

        #region Constructors
        public UrlRoutingModule()
        {
        }
        #endregion

        #region Instance Properties
        public RouteCollection Routes
        {
            get
            {
                if (_routes == null)
                    _routes = RouteTable.Routes;
                
                return _routes;
            }
            set
            {
                _routes = value;
            }
        }
        #endregion

        #region Instance Methods
        protected virtual void Init(HttpApplication application)
        {
            application.PostResolveRequestCache += new EventHandler(OnPostResolveRequestCache);
            application.PostMapRequestHandler += new EventHandler(OnPostMapRequestHandler);
            EnsureRouteTableLoaded();
        }

        private void OnPostResolveRequestCache(object sender, EventArgs e)
        {
            HttpContextBase context = new HttpContextWrapper(((HttpApplication)sender).Context);
            PostResolveRequestCache(context);
        }

        private void OnPostMapRequestHandler(object sender, EventArgs e)
        {
            HttpContextBase context = new HttpContextWrapper(((HttpApplication)sender).Context);
            PostMapRequestHandler(context);
        }

        /// <summary>
        /// Finds the appropriate route for the specified 
        /// HTTP request and rewrites the execution path, 
        /// if necessary
        /// </summary>
        /// <param name="context">The current HTTP request context</param>
        public virtual void PostResolveRequestCache(HttpContextBase context)
        {
            RouteData data = Routes.GetRouteData(context);
            if (data == null)
                return;

            IRouteHandler handler = data.Handler;

            if (handler == null)
                throw Error.NoRouteHandlerFound();

            if (handler is StopRoutingHandler) // если надо тормознуть, тормозим
                return;

            RequestContext requestContext = new RequestContext(context, data);
            IHttpHandler httpHandler = handler.GetHttpHandler(requestContext);

            if (httpHandler == null)
                throw Error.NoHttpHandlerFound(handler.GetType());

            context.Items[_requestDataKey] =
                new RequestData(httpHandler, context.Request.Path);

            context.RewritePath("~/wahha.routing.axd");
        }

        /// <summary>
        /// Performs the cleanup after the 
        /// request had been routed
        /// </summary>
        /// <param name="context">The current HTTP request context</param>
        public virtual void PostMapRequestHandler(HttpContextBase context)
        {
            RequestData data = (RequestData)context.Items[_requestDataKey];
            if (data != null)
            {
                context.RewritePath(data.OriginalPath);
                context.Handler = data.Handler;
            }
        }

        protected virtual void EnsureRouteTableLoaded()
        {
			IRouteTableProvider provider = RouteTable.Provider;
			RouteTableProviderResult result = provider.GetRouteTable();

			RouteTable.Routes.Variables.Merge((IValueSet)result.Variables);
			RouteTable.Routes.AddRange(result.Routes);
        }

        protected virtual void Dispose()
        {   }

        void IHttpModule.Init(HttpApplication application)
        {
            Init(application);
        }

        void IHttpModule.Dispose()
        {
            Dispose();
        }
        #endregion
    }
}
