using System;
using Radischevo.Wahha.Web.Mvc;

namespace ConsoleTester
{
	public class StubController : Controller
	{
	}

	public class UserController : Controller
	{
	}

	public class BlogController : Controller
	{
		public class NestedController : Controller
		{
		}
	}

	public class TopicController : BlogController
	{
	}

	public class FuckingController<T> : Controller
	{
	}

	public class SimpleController : IController
	{
		#region IController Members
		public void Execute(Radischevo.Wahha.Web.Routing.RequestContext context)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
