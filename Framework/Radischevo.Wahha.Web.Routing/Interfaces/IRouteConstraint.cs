using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    public interface IRouteConstraint
    {
        bool Match(HttpContextBase context, Route route, ValueDictionary values, RouteDirection direction);
    }
}
