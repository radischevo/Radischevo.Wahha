using System;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IResultFilter
    {
        void OnResultExecuting(ResultExecutionContext context);
        void OnResultExecuted(ResultExecutedContext context);
    }
}
