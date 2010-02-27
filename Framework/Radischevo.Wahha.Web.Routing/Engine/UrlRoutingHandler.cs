using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Provides an <see cref="IHttpHandler"/> 
    /// for the HTTP request routing
    /// </summary>
    public abstract class UrlRoutingHandler : IHttpHandler
    {
        #region Instance Fields
        private RouteCollection _routes;
        #endregion

        #region Constructors
        protected UrlRoutingHandler()
        {   }
        #endregion

        #region Instance Properties
        protected virtual bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public RouteCollection Routes
        {
            get
            {
                if (_routes == null)
                    _routes = RouteTable.Routes;

                return _routes;
            }
        }
        #endregion

        #region Instance Methods
        protected virtual void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextWrapper(context));
        }

        protected virtual void ProcessRequest(HttpContextBase context)
        {
            RouteData data = Routes.GetRouteData(context);
            Precondition.Require(data, () => Error.NoRouteMatched());

            IRouteHandler handler = data.Handler;
            Precondition.Require(handler, () => Error.NoRouteHandlerFound());

            RequestContext ctx = new RequestContext(context, data);
            IHttpHandler httpHandler = handler.GetHttpHandler(ctx);
            Precondition.Require(httpHandler, () => Error.NoHttpHandlerFound(handler.GetType()));

            VerifyAndProcessRequest(httpHandler, context);
        }

        protected abstract void VerifyAndProcessRequest(IHttpHandler handler, HttpContextBase context);
        #endregion

        #region IHttpHandler Members
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            ProcessRequest(context);
        }

        bool IHttpHandler.IsReusable
        {
            get
            {
                return IsReusable;
            }
        }
        #endregion
    }
}
