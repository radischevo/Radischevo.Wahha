using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    public class HttpMethodConstraint : IRouteConstraint
    {
        #region Instance Fields
        private HttpMethod _methods;
        #endregion

        #region Constructors
        public HttpMethodConstraint(HttpMethod methods)
        {
            _methods = methods;
        }
        #endregion

        #region Instance Methods
        protected virtual bool Match(HttpContextBase context, Route route, 
            ValueDictionary values, RouteDirection direction)
        {
            if (direction == RouteDirection.UrlGeneration)
                return true;

            Precondition.Require(context, () => Error.ArgumentNull("context"));
            return ((context.Request.HttpMethod & _methods) > 0);
        }
        #endregion

        #region IRouteConstraint Members
        bool IRouteConstraint.Match(HttpContextBase context, Route route,
            ValueDictionary values, RouteDirection direction)
        {
            return Match(context, route, values, direction);
        }
        #endregion
    }
}
