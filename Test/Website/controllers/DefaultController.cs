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
		
		[HttpGet]
		public ActionResult Register ()
		{
			return View ("default/register", new RegistrationForm());
		}
		
		[HttpPost]
		public ActionResult Register ([Bind(Source = "Json,Form")] RegistrationForm form)
		{
			if (ModelState.IsValid())
				return Json (form);
			
			return View ("default/register", form);
		}
	}
}

