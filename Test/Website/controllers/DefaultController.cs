using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Radischevo.Wahha.Web.Mvc;

namespace Radischevo.Wahha.Test.Website
{
	[ErrorHandler, HttpCompression]
	public class DefaultController : Controller
	{
		public ActionResult Index ()
		{
			ViewData ["Message"] = "Welcome to Wahha MVC on Mono!";
			return View ("default/index");
		}
		
		public ActionResult List ()
		{
			ViewData ["Message"] = "Sample item list here.";
			return View ("default/index");
		}
	}
}

