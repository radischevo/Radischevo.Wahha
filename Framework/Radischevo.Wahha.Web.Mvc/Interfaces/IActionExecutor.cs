using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Defines the contract for an action invoker, used to 
    /// invoke an action in response to an http request.
    /// </summary>
    public interface IActionExecutor
    {
        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="values"></param>
        /// <returns>True if the action was found, otherwise false.</returns>
        bool InvokeAction(ControllerContext context, 
            string actionName, IDictionary<string, object> values);
    }
}
