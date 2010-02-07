using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web;

namespace Radischevo.Wahha.Web.Mvc
{
    public class WebFormViewEngine : VirtualPathViewEngine
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
		protected override bool FileExists(ControllerContext context, string virtualPath)
		{
			try
			{
				object viewInstance = BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(object));
				return (viewInstance != null);
			}
			catch (HttpException he)
			{
				if (he is HttpParseException)
				{
					throw;
				}
				if (he.GetHttpCode() == (int)HttpStatusCode.NotFound)
				{
					if (!base.FileExists(context, virtualPath))
						return false;
				}
				throw;
			}
		}

        protected override ViewEngineResult CreateView(ControllerContext context, string viewName)
        {
            string path = base.GetViewPath(context, viewName);
            if (String.IsNullOrEmpty(path))
                return null;

            return new ViewEngineResult(new WebFormView(path), this);
        }
        #endregion
    }
}
