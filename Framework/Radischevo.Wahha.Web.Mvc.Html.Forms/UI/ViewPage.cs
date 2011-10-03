using System;
using System.IO;
using System.Web;
using System.Web.UI;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;
using Radischevo.Wahha.Web.Abstractions;

using ViewError = Radischevo.Wahha.Web.Mvc.Html.Forms.Error;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
    [FileLevelControlBuilder(typeof(ViewPageBuilder))]
    public class ViewPage : Page,
        IViewDataContainer
    {
        #region Instance Fields
        private ViewDataDictionary _viewData;
        private ViewContext _viewContext;
        private HtmlTextWriter _writer;
        private HtmlHelper _html;
        private UrlHelper _url;
        private AjaxHelper _ajax;
        private HttpParameters _httpParameters;
		private TextWriter _output;
        #endregion

        #region Constructors
        public ViewPage() : base()
        {
            base.EnableViewState = false;
            base.UnregisterRequiresControlState(this);
        }
        #endregion

        #region Instance Properties
        public ViewDataDictionary ViewData
        {
            get
            {
                if (_viewData == null)
                    SetViewData(new ViewDataDictionary());
                
                return _viewData;
            }
            set
            {
                SetViewData(value);
            }
        }

        public TempDataDictionary TempData
        {
            get
            {
                return ViewContext.TempData;
            }
        }

        public HttpParameters HttpParameters
        {
            get
            {
                return _httpParameters;
            }
            set
            {
                _httpParameters = value;
            }
        }

        public HtmlTextWriter Writer
        {
            get
            {
                return _writer;
            }
        }

        public ViewContext ViewContext
        {
            get
            {
                return _viewContext;
            }
            set
            {
                _viewContext = value;
            }
        }

        /// <summary>
        /// Returns an <see cref="T:Radischevo.Wahha.Web.Mvc.UI.HtmlHelper"/> containing 
        /// methods useful for rendering HTML elements.
        /// </summary>
        public HtmlHelper Html
        {
            get
            {
                return _html;
            }
            set
            {
                _html = value;
            }
        }

		/// <summary>
		/// Gets or sets the <see cref="System.IO.TextWriter"/>, 
		/// where the page will be rendered into.
		/// </summary>
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

        /// <summary>
        /// Returns an <see cref="T:Radischevo.Wahha.Web.Mvc.UrlHelper"/> containing 
        /// methods useful for resolving URLs and routes.
        /// </summary>
        public UrlHelper Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        /// <summary>
        /// Returns an <see cref="T:Radischevo.Wahha.Web.Mvc.UI.AjaxHelper"/> 
        /// containing methods useful for AJAX scenarios.
        /// </summary>
        public AjaxHelper Ajax
        {
            get
            {
                return _ajax;
            }
            set
            {
                _ajax = value;
            }
        }

        public new CacheProvider Cache
        {
            get
            {
                return ViewContext.Controller.Cache;
            }
        }

        /// <summary>
        /// Convenience property used to access the Model 
        /// property of the <see cref="T:Radischevo.Wahha.Web.Mvc.ViewDataDictionary"/>
        /// </summary>
        public object Model
        {
            get
            {
                return ViewData.Model;
            }
        }
        #endregion

        #region Instance Methods
        private void SetContentType(HttpContextBase context)
        {
            context.Request.Browser
                .Capabilities["preferredRenderingMime"] = context.Response.ContentType;
        }

        protected virtual void SetViewData(ViewDataDictionary viewData)
        {
            _viewData = viewData;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            _writer = writer;
            
            try
            {
                base.Render(writer);
            }
            finally
            {
                _writer = null;
            }
        }

        /// <summary>
        /// Instantiates and initializes the Ajax, Html, and Url properties.
        /// </summary>
        public virtual void InitHelpers(ViewContext context)
        {
			Precondition.Require(context, 
				() => ViewError.ArgumentNull("context"));

            SetContentType(context.Context);
            _httpParameters = context.Context.Request.Parameters;
            _html = new HtmlHelper(context);
            _url = new UrlHelper(context);
            _ajax = new AjaxHelper(context);
        }

        /// <summary>
        /// Renders the view page to the response.
        /// </summary>
        public virtual void RenderView(ViewContext context)
        {
            _viewContext = context;
            InitHelpers(context);
            ID = Guid.NewGuid().ToString();

			context.Context.Server.Execute(HttpHandlerWrapper.Wrap(this), _output, true);
        }
        #endregion
    }

    public class ViewPage<TModel> : ViewPage
        where TModel : class
    {
        #region Instance Fields
        private ViewDataDictionary<TModel> _viewData;
        private HtmlHelper<TModel> _html;
        #endregion

        #region Constructors
        public ViewPage() : base()
        {
        }
        #endregion

        #region Instance Properties
        public new ViewDataDictionary<TModel> ViewData
        {
            get
            {
                if (_viewData == null)
                    SetViewData(new ViewDataDictionary<TModel>());
                
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

        /// <summary>
        /// Returns an <see cref="T:Radischevo.Wahha.Web.Mvc.UI.HtmlHelper"/> containing 
        /// methods useful for rendering HTML elements.
        /// </summary>
        public new HtmlHelper<TModel> Html
        {
            get
            {
                return _html;
            }
            set
            {
                _html = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void InitHelpers(ViewContext context)
        {
            base.InitHelpers(context);
            base.Html = _html = new HtmlHelper<TModel>(context);
        }

        protected override void SetViewData(ViewDataDictionary viewData)
        {
            _viewData = new ViewDataDictionary<TModel>(viewData);
            base.SetViewData(_viewData);
        }
        #endregion
    }
}
