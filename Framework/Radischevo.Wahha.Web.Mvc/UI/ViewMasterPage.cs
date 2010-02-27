using System;
using System.Collections.Generic;
using System.Web.UI;

using Radischevo.Wahha.Core;
using Mvc = Radischevo.Wahha.Web.Mvc;

namespace Radischevo.Wahha.Web.Mvc.UI
{
    public class ViewMasterPage : MasterPage
    {
        #region Constructors
        public ViewMasterPage()
        {
        }
        #endregion

        #region Instance Properties
        internal ViewPage ViewPage
        {
            get
            {
                ViewPage page = (Page as ViewPage);
				Precondition.Require(page, 
					() => Mvc.Error.ViewMasterPageRequiresViewPage());
                
                return page;
            }
        }

        public AjaxHelper Ajax
        {
            get
            {
                return ViewPage.Ajax;
            }
        }

        public HtmlHelper Html
        {
            get
            {
                return ViewPage.Html;
            }
        }

        public TempDataDictionary TempData
        {
            get
            {
                return ViewPage.TempData;
            }
        }

        public UrlHelper Url
        {
            get
            {
                return ViewPage.Url;
            }
        }

        public ValidationHelper Validation
        {
            get
            {
                return ViewPage.Validation;
            }
        }

        public HttpParameters Parameters
        {
            get
            {
                return ViewPage.Parameters;
            }
        }

        public object Model
        {
            get
            {
                return ViewData.Model;
            }
        }

        public ViewContext ViewContext
        {
            get
            {
                return ViewPage.ViewContext;
            }
        }

        public ViewDataDictionary ViewData
        {
            get
            {
                return ViewPage.ViewData;
            }
        }

        public HtmlTextWriter Writer
        {
            get
            {
                return ViewPage.Writer;
            }
        }
        #endregion
    }

    public class ViewMasterPage<TModel> : ViewMasterPage 
        where TModel : class
    {
        #region Instance Fields
        private ViewDataDictionary<TModel> _viewData;
        #endregion

        #region Constructors
        public ViewMasterPage()
        {
        }
        #endregion

        #region Instance Properties
        public new ViewDataDictionary<TModel> ViewData
        {
            get
            {
                if(_viewData == null)
                    _viewData = new ViewDataDictionary<TModel>(ViewPage.ViewData);

                return _viewData;
            }
        }

        public new TModel Model
        {
            get
            {
                return ViewData.Model;
            }
        }
        #endregion
    }
}
