using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Class used to render a view using an <see cref="T:Radischevo.Wahha.Web.Mvc.IView"/> 
    /// returned by a <see cref="T:Radischevo.Wahha.Web.Mvc.IViewEngine"/>.
    /// </summary>
    public class ViewResult : ViewResultBase
    {
        #region Constructors
        public ViewResult()
            : base()
        {
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Searches the registered view engines and returns the 
        /// <see cref="T:Radischevo.Wahha.Web.Mvc.ViewEngineResult"/> 
        /// used to render the view.
        /// </summary>
        /// <param name="context"></param>
        protected override ViewEngineResult FindView(ControllerContext context)
        {
            ViewEngineResult result = base.ViewEngines.FindView(context, base.ViewName);
            Precondition.Require(result, Error.ViewNotFound(base.ViewName));

            return result;
        }
        #endregion
    }
}
