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
        
        public void OnAuthorization(AuthorizationContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            if (Validate(context.Context))
            {
                HttpCachePolicyBase cachePolicy = context.Context.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidationHandler, null);
            }
            else
            {
                context.Cancel = true;
                if (String.IsNullOrEmpty(_redirectTo))
                    context.Result = DefaultResult;
                else
                    context.Result = new RedirectResult(_redirectTo);
            }
        }

        protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));

            bool isAuthorized = Validate(context);
            return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }
        #endregion
    }
}
