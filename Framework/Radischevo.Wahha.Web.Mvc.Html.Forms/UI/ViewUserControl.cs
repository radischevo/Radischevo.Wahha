using System;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;

using Radischevo.Wahha.Data.Caching;

using ViewError = Radischevo.Wahha.Web.Mvc.Html.Forms.Error;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
	[FileLevelControlBuilder(typeof(ViewUserControlBuilder))]
    public class ViewUserControl : UserControl,
        IViewDataContainer
    {
        #region Nested Types
        private sealed class ControlContainerPage : ViewPage
        {
            #region Constructors
            internal ControlContainerPage(ViewUserControl control)
            {
                Controls.Add(control);
            }
            #endregion
        }
        #endregion

        #region Instance Fields
        private ViewDataDictionary _viewData;
        private string _viewDataKey;
        private string _subDataKey;
		private TextWriter _output;
        #endregion

        #region Constructors
        protected ViewUserControl()
        {
            this.EnableViewState = false;
        }
        #endregion

        #region Instance Properties
        private ViewPage ViewPage
        {
            get
            {
                ViewPage page = (Page as ViewPage);
                if (page != null)
                    return page;

                throw ViewError.ViewControlRequiresViewPage();
            }
        }

        public object Model
        {
            get
            {
                return ViewData.Model;
            }
        }

        public ViewDataDictionary ViewData
        {
            get
            {
                EnsureViewData();
                return _viewData;
            }
            set
            {
                SetViewData(value);
            }
        }

        public string ViewDataKey
        {
            get
            {
                return (_viewDataKey == null) ?
                    String.Empty : _viewDataKey;
            }
            set
            {
                _viewDataKey = value;
            }
        }

        public string SubDataKey
        {
            get
            {
                return _subDataKey;
            }
            set
            {
                _subDataKey = value;
            }
        }

        public ViewContext ViewContext
        {
            get
            {
                return ViewPage.ViewContext;
            }
        }

		public TextWriter Output
		{
			get
			{
				return _output;
			}
			set
			{
				_output = value;
			}
		}

        public HtmlTextWriter Writer
        {
            get
            {
                return ViewPage.Writer;
            }
        }

        public HtmlHelper Html
        {
            get
            {
                return ViewPage.Html;
            }
        }

        public UrlHelper Url
        {
            get
            {
                return ViewPage.Url;
            }
        }

        public AjaxHelper Ajax
        {
            get
            {
                return ViewPage.Ajax;
            }
        }

        public new CacheProvider Cache
        {
            get
            {
                return ViewPage.Cache;
            }
        }

        public HttpParameters HttpParameters
        {
            get
            {
                return ViewPage.HttpParameters;
            }
        }
        #endregion

        #region Instance Methods
        protected void EnsureViewData()
        {
            if (_viewData != null)
                return;

            IViewDataContainer vdc = GetViewDataContainer(this);
            if (vdc == null)
                throw ViewError.ControlRequiresViewDataProvider();

            ViewDataDictionary vd = vdc.ViewData;
            if (!String.IsNullOrEmpty(_subDataKey))
            {
                ViewDataDictionary subViewData;
                vdc.ViewData.SubDataItems.TryGetValue(_subDataKey, out subViewData);
                vd = new ViewDataDictionary(subViewData);
                vd.Model = subViewData.Model;
            }

            if (!String.IsNullOrEmpty(ViewDataKey))
            {
                ViewDataDictionary vdPart = new ViewDataDictionary();
                vdPart.Model = vd[ViewDataKey];
                vd = vdPart;
            }
            SetViewData(vd);
        }

        public virtual void RenderView(ViewContext context)
        {
            context.Context.Response.Cache.SetExpires(DateTime.Now);
            ControlContainerPage page = new ControlContainerPage(this);
			page.Output = _output;

			string contentType = context.Context.Response.ContentType;
			page.RenderView(context);

			context.Context.Response.ContentType = contentType;
        }

        protected virtual void SetViewData(ViewDataDictionary viewData)
        {
            _viewData = viewData;
        }
        #endregion

        #region Static Methods
        private static IViewDataContainer GetViewDataContainer(Control control)
        {
            while (control != null)
            {
                control = control.Parent;
                IViewDataContainer vdc = control as IViewDataContainer;
                if (vdc != null)
                    return vdc;
            }
            return null;
        }
        #endregion
    }

    public class ViewUserControl<TModel> : ViewUserControl 
        where TModel : class
    {
        #region Instance Fields
        private ViewDataDictionary<TModel> _viewData;
        #endregion

        #region Constructors
        public ViewUserControl()
        {
        }
        #endregion

        #region Instance Properties
        public new ViewDataDictionary<TModel> ViewData
        {
            get
            {
                EnsureViewData();
                return _viewData;
            }
            set
            {
                SetViewData(value);
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

        #region Instance Methods
        protected override void SetViewData(ViewDataDictionary viewData)
        {
            _viewData = new ViewDataDictionary<TModel>(viewData);
            base.SetViewData(_viewData);
        }
        #endregion
    }
}
