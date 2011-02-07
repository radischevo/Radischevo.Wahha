using System;
using System.IO;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
    public class WebFormView : BuildManagerCompiledView
    {
        #region Constructors
		public WebFormView(string viewPath, IBuildManager buildManager)
			: this(viewPath, buildManager, new DefaultViewActivator())
		{
		}

        public WebFormView(string viewPath, 
			IBuildManager buildManager, IViewActivator activator)
			: base(viewPath, buildManager, activator)
        {
        }
        #endregion

        #region Instance Methods
		protected override void RenderView(ViewContext context, TextWriter writer, object instance)
		{
			ViewPage page = (instance as ViewPage);
			ViewUserControl control = (instance as ViewUserControl);

			if (page != null)
				RenderViewPage(context, writer, page);
			else if (control != null)
				RenderViewUserControl(context, writer, control);
			else
				throw Error.WrongViewBase(instance.GetType());
		}

		private void RenderViewPage(ViewContext context, 
			TextWriter writer, ViewPage page)
		{
			page.ViewData = context.ViewData;
			page.Output = writer;
			page.RenderView(context);
		}

		private void RenderViewUserControl(ViewContext context, 
			TextWriter writer, ViewUserControl control)
		{
			control.ViewData = context.ViewData;
			control.Output = writer;
			control.RenderView(context);
		}
        #endregion
    }
}
