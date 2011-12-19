using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Mvc;

namespace Radischevo.Wahha.Test.Website
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes (RouteCollection routes)
		{
			routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");
			routes.MapRoute("default", "/{controller}/{action}/{id}", 
                new { controller = "Default", action = "Index", id = "" }
            );
		}

		protected void Application_Start ()
		{
			RegisterRoutes (RouteTable.Routes);		
		}
	}
}
