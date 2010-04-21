using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireAuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region Instance Fields
        private string _redirectTo;
		private string _parameterName;
        #endregion

        #region Constructors
        public RequireAuthorizationAttribute() 
            : base()
        {   }
        #endregion

        #region Instance Properties
        public string RedirectTo
        {
            get
            {
                return _redirectTo;
            }
            set
            {
                _redirectTo = value;
            }
        }

		public string ParameterName
		{
			get
			{
				return _parameterName;
			}
			set
			{
				_parameterName = value;
			}
		}

        protected virtual ActionResult DefaultResult
        {
            get
            {
                return new HttpUnauthorizedResult();
            }
        }
        #endregion

        #region Instance Methods
        private void CacheValidationHandler(HttpContext context, 
            object data, ref HttpValidationStatus status)
        {
            status = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        protected virtual bool Validate(HttpContextBase context)
        {
            if(context.User == null || context.User.Identity == null)
                return false;
            
            return context.User.Identity.IsAuthenticated;
        }

		protected virtual string CreateRedirectionUrl(AuthorizationContext context)
		{
			if (String.IsNullOrEmpty(_redirectTo))
				return null;

			string url = _redirectTo;
			string query = String.Empty;

			if (!String.IsNullOrEmpty(_parameterName))
				query = String.Concat(
					(_redirectTo.IndexOf('?') > -1) ? "&" : "?",
					_parameterName, "=",
					Uri.EscapeUriString(context.HttpContext.Request.Url.PathAndQuery)
				);
			
			return url + query;
		}

		protected virtual bool RedirectAllowed(AuthorizationContext context, string url)
		{
			if (String.IsNullOrEmpty(url))
				return false;

			if (context.Context.IsChild)
				return false;

			if (context.HttpContext.Request.IsAjaxRequest)
				return false;

			return true;
		}

        public void OnAuthorization(AuthorizationContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
			if (Validate(context.HttpContext))
            {
				HttpCachePolicyBase cachePolicy = context.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidationHandler, null);
            }
            else
            {
                context.Cancel = true;
				string url = CreateRedirectionUrl(context);

				if (RedirectAllowed(context, url))
					context.Result = new RedirectResult(url);
                else
					context.Result = DefaultResult;
            }
        }

        protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));

            bool isAuthorized = Validate(context);
            return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }
        #endregion
    }
}
