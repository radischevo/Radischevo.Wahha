using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IViewEngine
    {
        void Init(IValueSet settings);
        ViewEngineResult FindView(ControllerContext context, string viewName);
        void ReleaseView(ControllerContext context, IView view);
    }
}
