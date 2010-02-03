using System;
using System.Web;

using Radischevo.Wahha.Web.Mvc;
using Radischevo.Wahha.Web.Routing;

public class OutputMessageAttribute : FilterAttribute, IActionFilter
{
    private string _message;

    public OutputMessageAttribute()
    {
        _message = String.Empty;
    }

    public string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }

    public void OnExecuting(ActionExecutionContext context)
    {
        context.Context.Response.Write(_message);
    }

    #region IActionFilter Members
    void IActionFilter.OnExecuted(ActionExecutedContext context)
    {
    }
    #endregion
}
