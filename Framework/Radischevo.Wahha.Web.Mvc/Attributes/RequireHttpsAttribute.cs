using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
		Inherited = true, AllowMultiple = false)]
	public class RequireHttpsAttribute : FilterAttribute, IAuthorizationFilter
	{
		#region Constructors
		public RequireHttpsAttribute()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public void OnAuthorization(AuthorizationContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			if (!context.HttpContext.Request.IsSecureConnection)
				HandleNonSecureRequest(context);
		}

		protected virtual void HandleNonSecureRequest(AuthorizationContext context)
		{
			if (context.HttpContext.Request.HttpMethod != HttpMethod.Get)
				throw Error.SecureConnectionRequired();

			string url = "https://" + context.HttpContext.Request.Url.Host 
				+ context.HttpContext.Request.RawUrl;

			context.Cancel = true;
			context.Result = new RedirectResult(url);
		}
		#endregion
	}
}
