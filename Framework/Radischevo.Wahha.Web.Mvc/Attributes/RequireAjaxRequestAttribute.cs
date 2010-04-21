using System;
using System.Net;
using System.Reflection;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Adds an action filter that checks 
    /// if the current request is performed through 
    /// the XmlHttpRequest object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class RequireAjaxRequestAttribute : ActionSelectorAttribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.RequireAjaxRequestAttribute"/> 
        /// class.
        /// </summary>
        public RequireAjaxRequestAttribute()
            : base()
        {
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Checks the request for XmlHttpRequest headers.
        /// </summary>
        /// <param name="context">The context of the current request.</param>
        /// <param name="actionMethod">The method, currently being executed.</param>
        public override bool IsValid(ControllerContext context, MethodInfo actionMethod)
        {
            return context.Context.Request.IsAjaxRequest;
        }
        #endregion
    }
}
