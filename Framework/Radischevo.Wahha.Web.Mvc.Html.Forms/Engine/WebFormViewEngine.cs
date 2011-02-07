using System;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
    public class WebFormViewEngine : BuildManagerViewEngine
	{
		#region Constructors
		public WebFormViewEngine() 
            : base("~/{0}.aspx", "~/{0}.ascx", 
                 "~/views/{0}.aspx", "~/views/{0}.ascx", 
                 "~/views/{1}/{0}.aspx", "~/views/{1}/{0}.ascx", 
                 "~/views/shared/{0}.aspx", "~/views/shared/{0}.ascx")
        {   }
        #endregion

		#region Instance Methods
		protected override bool IsValidCompiledType(ControllerContext context, string virtualPath, Type type)
		{
			if (!typeof(ViewPage).IsAssignableFrom(type))
				return typeof(ViewUserControl).IsAssignableFrom(type);
			
			return true;
		}

        protected override IView CreateView(ControllerContext context, string virtualPath)
        {
			return new WebFormView(virtualPath, BuildManager);
        }
        #endregion
    }
}
