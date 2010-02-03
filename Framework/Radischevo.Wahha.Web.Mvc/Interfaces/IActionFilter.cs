using System;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IActionFilter
    {
        void OnExecuting(ActionExecutionContext context);
        void OnExecuted(ActionExecutedContext context);
    }
}
