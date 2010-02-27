using System;
using System.IO;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.UI;

namespace Radischevo.Wahha.Web.Mvc
{
    public class WebFormView : IView
    {
        #region Instance Fields
        private IBuildManager _buildManager;
        private string _viewPath;
        #endregion

        #region Constructors
        public WebFormView(string viewPath)
        {
            Precondition.Defined(viewPath, () => Error.ArgumentNull("viewPath"));
            _viewPath = viewPath;
        }
        #endregion

        #region Instance Properties
        internal IBuildManager BuildManager
        {
            get
            {
                if (_buildManager == null)
                    _buildManager = new BuildManagerWrapper();

                return _buildManager;
            }
            set
            {
                _buildManager = value;
            }
        }

        public string ViewPath
        {
            get
            {
                return _viewPath;
            }
        }
        #endregion

        #region Instance Methods
        protected virtual void Render(ViewContext context, TextWriter writer)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            
            object instance = BuildManager.CreateInstanceFromVirtualPath(_viewPath, typeof(object));
            if (instance == null)
                throw Error.CouldNotCreateView(_viewPath);
            
            ViewPage page = (instance as ViewPage);
            ViewUserControl control = (instance as ViewUserControl);

            if (page != null)
            {
                page.ViewData = context.ViewData;
				page.Output = writer;
                page.RenderView(context);
            }
            else if (control != null)
            {
                control.ViewData = context.ViewData;
				control.Output = writer;
                control.RenderView(context);
            }
            else
                throw Error.WrongViewBase(instance.GetType());
        }
        #endregion

        #region IView Members
        void IView.Render(ViewContext context, TextWriter writer)
        {
            Render(context, writer);
        }
        #endregion
    }
}
