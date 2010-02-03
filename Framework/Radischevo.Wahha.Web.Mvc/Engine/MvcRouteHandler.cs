using System;
using System.Web;

using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    public class MvcRouteHandler : IRouteHandler
    {
        #region Constructors
        public MvcRouteHandler()
        {
        }
        #endregion

        #region Instance Methods
        protected virtual IHttpHandler GetHttpHandler(RequestContext context)
        {
            return new MvcHandler(context);
        }

        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext context)
        {
            return GetHttpHandler(context);
        }
        #endregion
    }
}
