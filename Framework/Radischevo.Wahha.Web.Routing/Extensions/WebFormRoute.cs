using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    public class WebFormRoute : Route
    {
        #region Constructors
        public WebFormRoute(string url, string virtualPath)
            : this(url, virtualPath, null, null, null)
        {
        }

        public WebFormRoute(string url,
            string virtualPath, ValueDictionary defaults)
            : this(url, virtualPath, defaults, null, null)
        {
        }

        public WebFormRoute(string url,
            string virtualPath, ValueDictionary defaults,
            IEnumerable<IRouteConstraint> constraints)
            : this(url, virtualPath, defaults, constraints, null)
        {
        }

        public WebFormRoute(string url,
            string virtualPath, ValueDictionary defaults,
            IEnumerable<IRouteConstraint> constraints,
            ValueDictionary tokens)
            : base(url, defaults, constraints, tokens,
                new WebFormRoutingHandler(virtualPath))
        {
        }
        #endregion
    }
}
