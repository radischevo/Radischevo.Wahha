using System;
using System.Security.Permissions;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that contains methods 
    /// for setting cache-specific HTTP headers and for controlling the 
    /// ASP.NET page output cache.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpCachePolicyWrapper : HttpCachePolicyBase
    {
        #region Instance Fields
        private readonly HttpCachePolicy _policy;
        #endregion

        #region Constructors
        public HttpCachePolicyWrapper(HttpCachePolicy policy)
        {
            Precondition.Require(policy, Error.ArgumentNull("policy"));
            _policy = policy;
        }
        #endregion

        #region Instance Properties
        public override HttpCacheVaryByContentEncodings VaryByContentEncodings
        {
            get
            {
                return _policy.VaryByContentEncodings;
            }
        }

        public override HttpCacheVaryByHeaders VaryByHeaders
        {
            get
            {
                return _policy.VaryByHeaders;
            }
        }

        public override HttpCacheVaryByParams VaryByParams
        {
            get
            {
                return _policy.VaryByParams;
            }
        }
        #endregion

        #region Instance Methods
        public override void AddValidationCallback(HttpCacheValidateHandler handler, object data)
        {
            _policy.AddValidationCallback(handler, data);
        }

        public override void AppendCacheExtension(string extension)
        {
            _policy.AppendCacheExtension(extension);
        }

        public override void SetAllowResponseInBrowserHistory(bool allow)
        {
            _policy.SetAllowResponseInBrowserHistory(allow);
        }

        public override void SetCacheability(HttpCacheability cacheability)
        {
            _policy.SetCacheability(cacheability);
        }

        public override void SetCacheability(HttpCacheability cacheability, string field)
        {
            _policy.SetCacheability(cacheability, field);
        }

        public override void SetETag(string etag)
        {
            _policy.SetETag(etag);
        }

        public override void SetETagFromFileDependencies()
        {
            _policy.SetETagFromFileDependencies();
        }

        public override void SetExpires(DateTime date)
        {
            _policy.SetExpires(date);
        }

        public override void SetLastModified(DateTime date)
        {
            _policy.SetLastModified(date);
        }

        public override void SetLastModifiedFromFileDependencies()
        {
            _policy.SetLastModifiedFromFileDependencies();
        }

        public override void SetMaxAge(TimeSpan delta)
        {
            _policy.SetMaxAge(delta);
        }

        public override void SetNoServerCaching()
        {
            _policy.SetNoServerCaching();
        }

        public override void SetNoStore()
        {
            _policy.SetNoStore();
        }

        public override void SetNoTransforms()
        {
            _policy.SetNoTransforms();
        }

        public override void SetOmitVaryStar(bool omit)
        {
            _policy.SetOmitVaryStar(omit);
        }

        public override void SetProxyMaxAge(TimeSpan delta)
        {
            _policy.SetProxyMaxAge(delta);
        }

        public override void SetRevalidation(HttpCacheRevalidation revalidation)
        {
            _policy.SetRevalidation(revalidation);
        }

        public override void SetSlidingExpiration(bool slide)
        {
            _policy.SetSlidingExpiration(slide);
        }

        public override void SetValidUntilExpires(bool validUntilExpires)
        {
            _policy.SetValidUntilExpires(validUntilExpires);
        }

        public override void SetVaryByCustom(string custom)
        {
            _policy.SetVaryByCustom(custom);
        }
        #endregion
    }
}
