using System;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Base class used to supply the model to the view 
    /// and then render the view to the response.
    /// </summary>
    public abstract class ViewResultBase : ActionResult
    {
        #region Instance Fields
        private TempDataDictionary _tempData;
        private ViewDataDictionary _viewData;
        private ValidationErrorCollection _errors;
        private ViewEngineCollection _viewEngines;
        private string _viewName;
        private IView _view;
        #endregion

        #region Constructors
        protected ViewResultBase()
        {
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets or sets the <see cref="T:Radischevo.Wahha.Web.Mvc.TempDataDictionary"/> 
        /// for this result.
        /// </summary>
        public TempDataDictionary TempData
        {
            get
            {
                if (_tempData == null)
                    _tempData = new TempDataDictionary();

                return _tempData;
            }
            set
            {
                _tempData = value;
            }
        }

        /// <summary>
        /// Gets or sets the view data 
        /// <see cref="T:Radischevo.Wahha.Web.Mvc.ViewDataDictionary"/> 
        /// for this result.
        /// </summary>
        public ViewDataDictionary ViewData
        {
            get
            {
                if (_viewData == null)
                    return new ViewDataDictionary();

                return _viewData;
            }
            set
            {
                _viewData = value;
            }
        }

        /// <summary>
        /// Gets the view engines (
        /// <see cref="T:Radischevo.Wahha.Web.Mvc.ViewEngineCollection"/>) 
        /// associated with this result.
        /// </summary>
        public ViewEngineCollection ViewEngines
        {
            get
            {
                if (_viewEngines == null)
                    _viewEngines = Configurations
                        .Configuration.Instance.Views.ViewEngines;

                return _viewEngines;
            }
        }

        /// <summary>
        /// The name of the view to be rendered.
        /// </summary>
        public string ViewName
        {
            get
            {
                return _viewName ?? String.Empty;
            }
            set
            {
                _viewName = value;
            }
        }

        /// <summary>
        ///  Gets or sets the <see cref="T:Radischevo.Wahha.Web.Mvc.IView"/> 
        ///  that is rendered to the response.
        /// </summary>
        public IView View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
            }
        }

        public ValidationErrorCollection Errors
        {
            get
            {
                if (_errors == null)
                    _errors = new ValidationErrorCollection();

                return _errors;
            }
            set
            {
                _errors = value;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// When called by the action invoker, renders the view to the response.
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            if (String.IsNullOrEmpty(ViewName))
                ViewName = context.RouteData.GetRequiredValue<string>("action");

            ViewEngineResult result = null;
            if (View == null)
            {
                result = FindView(context);
                View = result.View;
            }

            ViewContext vc = new ViewContext(context, View, ViewData, TempData);
            vc.Errors = Errors;

            try
            {
                View.Render(vc, context.Context.Response.Output);
            }
            finally
            {
                if(result != null)
                    result.Engine.ReleaseView(context, View);
            }
        }

        /// <summary>
        /// When overridden, returns the <see cref="T:Radischevo.Wahha.Web.Mvc.ViewEngineResult"/> 
        /// used to render the view.
        /// </summary>
        /// <param name="context"></param>
        protected abstract ViewEngineResult FindView(ControllerContext context);
        #endregion
    }
}
