using System;
using System.Web;

namespace Radischevo.Wahha.Web.Routing
{
    public class StopRoutingHandler : IRouteHandler
    {
        #region Constructors
        public StopRoutingHandler()
        {
        }
        #endregion

        #region Instance Methods
        protected virtual IHttpHandler GetHttpHandler(RequestContext context)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region IRouteHandler Members
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext context)
        {
            return GetHttpHandler(context);
        }
        #endregion
    }
}
