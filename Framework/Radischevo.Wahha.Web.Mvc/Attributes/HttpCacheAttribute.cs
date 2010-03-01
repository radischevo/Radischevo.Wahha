using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
        Inherited = true, AllowMultiple = false)]
    public class HttpCacheAttribute : ActionFilterAttribute
    {
        #region Instance Fields
        private HttpCacheability _cacheability;
        private HttpCacheRevalidation _revalidation;
        private int _duration;
        #endregion

        #region Constructors
        public HttpCacheAttribute()
        {
            _cacheability = HttpCacheability.Public;
            _revalidation = HttpCacheRevalidation.None;
        }

        public HttpCacheAttribute(HttpCacheability cacheability)
        {
            _cacheability = cacheability;
            _revalidation = HttpCacheRevalidation.None;
        }

        public HttpCacheAttribute(HttpCacheability cacheability,
            HttpCacheRevalidation revalidation)
        {
            _cacheability = cacheability;
            _revalidation = revalidation;
        }
        #endregion

        #region Instance Properties
        public HttpCacheability Cacheability
        {
            get
            {
                return _cacheability;
            }
            set
            {
                _cacheability = value;
            }
        }

        public HttpCacheRevalidation Revalidation
        {
            get
            {
                return _revalidation;
            }
            set
            {
                _revalidation = value;
            }
        }

        public int Duration
        {
            get
            {
                return _duration;
            }
            set 
            {
                _duration = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void OnExecuting(ActionExecutionContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
			HttpResponseBase response = context.HttpContext.Response;

            response.Cache.SetCacheability(_cacheability);
            response.Cache.SetRevalidation(_revalidation);
            response.Cache.SetExpires(DateTime.Now.AddSeconds(_duration));
            response.Cache.SetMaxAge(TimeSpan.FromSeconds(_duration));
            response.Cache.SetETagFromFileDependencies();
        }
        #endregion
    }
}