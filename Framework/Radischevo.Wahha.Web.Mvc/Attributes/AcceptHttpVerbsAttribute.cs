using System;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// When applied to an action method, specifies which HTTP verbs the method will respond to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AcceptHttpVerbsAttribute : ActionSelectorAttribute
    {
        #region Instance Fields
        private HttpMethod _verbs;
        #endregion

        #region Constructors
        public AcceptHttpVerbsAttribute(HttpMethod verbs)
        {
            _verbs = verbs;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the combination of HTTP verbs 
        /// the action method will respond to.
        /// </summary>
        public HttpMethod Verbs
        {
            get
            {
                return _verbs;
            }
        }
        #endregion

        #region Instance Methods
        public override bool IsValid(ControllerContext context, MethodInfo actionMethod)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            return ((_verbs & context.Context.Request.HttpMethod) > 0);
        }
        #endregion
    }
}
