using System;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Encapsulates the result of an action method and is used to perform a 
    /// framework level operation on the action method's behalf.
    /// </summary>
    public abstract class ActionResult
    {
        #region Constructors
        protected ActionResult()
        {   }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Enables processing of the result of an action method by a 
        /// custom type that inherits from <see cref="T:Radischevo.Wahha.Web.Mvc.ActionResult"/>.
        /// </summary>
        /// <param name="context"></param>
        public abstract void Execute(ControllerContext context);
        #endregion
    }
}
