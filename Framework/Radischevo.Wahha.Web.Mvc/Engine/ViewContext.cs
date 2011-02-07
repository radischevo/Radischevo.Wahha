using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Encapsulates information related to rendering a view.
    /// </summary>
    public class ViewContext : ControllerContext
    {
        #region Instance Fields
        private TempDataDictionary _tempData;
        private ViewDataDictionary _viewData;
        private IView _view;
        private ValidationErrorCollection _errors;
        #endregion

        #region Constructors
        public ViewContext(ControllerContext context, IView view,
            ViewDataDictionary viewData, TempDataDictionary tempData)
            : this(ViewContext.GetControllerContext(context).Context, 
            GetControllerContext(context).RouteData, 
            GetControllerContext(context).Controller, view, 
            viewData, tempData)
        {
        }

        public ViewContext(RequestContext context, ControllerBase controller, 
            IView view, ViewDataDictionary viewData, TempDataDictionary tempData)
            : this(GetRequestContext(context).Context, GetRequestContext(context).RouteData, 
            controller, view, viewData, tempData)
        {
        }

        public ViewContext(HttpContextBase context, RouteData routeData, 
            ControllerBase controller, IView view, ViewDataDictionary viewData, 
            TempDataDictionary tempData)
            : base(context, routeData, controller)
        {
            _view = view;
            _viewData = viewData;
            _tempData = tempData;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets data associated with this request 
        /// which only lives for one request.
        /// </summary>
        public TempDataDictionary TempData
        {
            get
            {
                return _tempData;
            }
        }

        /// <summary>
        /// Gets the view data supplied to the view.
        /// </summary>
        public ViewDataDictionary ViewData
        {
            get
            {
                return _viewData;
            }
            set
            {
                _viewData = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Radischevo.Wahha.Web.Mvc.IView"/> 
        /// to render.
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

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection"/> 
        /// containing any validation errors that occurred while processing the request.
        /// </summary>
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
    }
}
