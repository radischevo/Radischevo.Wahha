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
            : this(String.Empty, null)
        {
        }

		public ViewResult(string viewName)
			: this(viewName, null)
		{
		}

		public ViewResult(IView view)
			: this(view, null)
		{
		}

		public ViewResult(string viewName, ViewDataDictionary viewData)
			: base()
		{
			ViewName = viewName;
			ViewData = viewData;
		}

		public ViewResult(IView view, ViewDataDictionary viewData)
			: base()
		{
			View = view;
			ViewData = viewData;
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
            Precondition.Require(result, () => Error.ViewNotFound(base.ViewName));

            return result;
        }
        #endregion
    }
}
