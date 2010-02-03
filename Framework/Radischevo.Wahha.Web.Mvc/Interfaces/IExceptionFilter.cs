using System;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IExceptionFilter
    {
        void OnException(ExceptionContext context);
    }
}
