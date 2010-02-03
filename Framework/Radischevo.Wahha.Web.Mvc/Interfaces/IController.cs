using System;

using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IController
    {
        void Execute(RequestContext context);
    }
}
