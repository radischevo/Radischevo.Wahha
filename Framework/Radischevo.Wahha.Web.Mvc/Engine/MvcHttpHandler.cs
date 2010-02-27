using System;
using System.Web;
using System.Web.SessionState;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
	public class MvcHttpHandler
		: UrlRoutingHandler, IRequiresSessionState
	{
		#region Constructors
		public MvcHttpHandler()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		protected override void VerifyAndProcessRequest(IHttpHandler handler, HttpContextBase context)
		{
			Precondition.Require(handler, () => Error.ArgumentNull("handler"));
			Precondition.Require(context, () => Error.ArgumentNull("context"));

			handler.ProcessRequest(context.Unwrap());
		}
		#endregion
	}
}
