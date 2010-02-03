using System;
using System.Web;

namespace Radischevo.Wahha.Web.Routing
{
    public abstract class RoutingHandlerBase : IRouteHandler
    {
        #region Constructors
        protected RoutingHandlerBase()
        {
        }
        #endregion

        #region Instance Methods
        private IHttpHandler GetHttpHandlerInternal(RequestContext context)
        {
            IHttpHandler handler = GetHttpHandler(context);
            IRoutableHttpHandler rh = (handler as IRoutableHttpHandler);
            if (rh != null)
                rh.RequestContext = context;

            return handler;
        }

        protected abstract IHttpHandler GetHttpHandler(RequestContext context);
        #endregion

        #region IRouteHandler Members
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext context)
        {
            return GetHttpHandlerInternal(context);    
        }
        #endregion
    }
}
