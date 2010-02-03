using System;
using System.Web;

using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class MvcAsyncRouteHandler : MvcRouteHandler
	{
		#region Constructors
		public MvcAsyncRouteHandler()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		protected override IHttpHandler GetHttpHandler(RequestContext context)
		{
			return new MvcAsyncHandler(context);
		}
		#endregion
	}
}
