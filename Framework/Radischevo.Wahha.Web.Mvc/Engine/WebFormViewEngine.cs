using System;

using Radischevo.Wahha.Web.Mvc.UI;

namespace Radischevo.Wahha.Web.Mvc
{
    public class WebFormViewEngine : BuildManagerViewEngine
	{
		#region Instance Fields
		private IBuildManager _buildManager;
		#endregion

		#region Constructors
		public WebFormViewEngine() 
            : base("~/{0}.aspx", "~/{0}.ascx", 
                 "~/Views/{0}.aspx", "~/Views/{0}.ascx", 
                 "~/Views/{1}/{0}.aspx", "~/Views/{1}/{0}.ascx", 
                 "~/Views/Shared/{0}.aspx", "~/Views/Shared/{0}.ascx")
        {   }
        #endregion

		#region Instance Properties
		public IBuildManager BuildManager
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
