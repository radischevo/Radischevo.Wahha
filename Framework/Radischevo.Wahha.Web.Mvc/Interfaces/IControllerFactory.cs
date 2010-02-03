using System;
using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IControllerFactory
    {
        void Init(IValueSet settings);
        IController CreateController(RequestContext context, string controllerName);
        void ReleaseController(IController controller);
    }
}
