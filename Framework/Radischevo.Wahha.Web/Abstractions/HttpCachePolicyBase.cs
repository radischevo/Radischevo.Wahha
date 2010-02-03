using System;
using System.Security.Permissions;
using System.Web;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Serves as the base class for classes that contain methods for 
    /// setting cache-specific HTTP headers and for controlling the 
    /// ASP.NET page output cache. 
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpCachePolicyBase
    {
        #region Constructors
        /// <summary>
        /// Initializes the class for use by an inherited class instance. 
        /// This constructor can only be called by an inherited class.
        /// </summary>
        protected HttpCachePolicyBase()
        {
        }
        #endregion

        #region Instance Properties
        public virtual HttpCacheVaryByContentEncodings VaryByContentEncodings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpCacheVaryByHeaders VaryByHeaders
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpCacheVaryByParams VaryByParams
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Instance Methods
        public virtual void AddValidationCallback(HttpCacheValidateHandler handler, object data)
        {
            throw new NotImplementedException();
        }

        public virtual void AppendCacheExtension(string extension)
        {
            throw new NotImplementedException();
        }

        public virtual void SetAllowResponseInBrowserHistory(bool allow)
        {
            throw new NotImplementedException();
        }

        public virtual void SetCacheability(HttpCacheability cacheability)
        {
            throw new NotImplementedException();
        }

        public virtual void SetCacheability(HttpCacheability cacheability, string field)
        {
            throw new NotImplementedException();
        }

        public virtual void SetETag(string etag)
        {
            throw new NotImplementedException();
        }

        public virtual void SetETagFromFileDependencies()
        {
            throw new NotImplementedException();
        }

        public virtual void SetExpires(DateTime date)
        {
            throw new NotImplementedException();
        }

        public virtual void SetLastModified(DateTime date)
        {
            throw new NotImplementedException();
        }

        public virtual void SetLastModifiedFromFileDependencies()
        {
            throw new NotImplementedException();
        }

        public virtual void SetMaxAge(TimeSpan delta)
        {
            throw new NotImplementedException();
        }

        public virtual void SetNoServerCaching()
        {
            throw new NotImplementedException();
        }

        public virtual void SetNoStore()
        {
            throw new NotImplementedException();
        }

        public virtual void SetNoTransforms()
        {
            throw new NotImplementedException();
        }

        public virtual void SetOmitVaryStar(bool omit)
        {
            throw new NotImplementedException();
        }

        public virtual void SetProxyMaxAge(TimeSpan delta)
        {
            throw new NotImplementedException();
        }

        public virtual void SetRevalidation(HttpCacheRevalidation revalidation)
        {
            throw new NotImplementedException();
        }

        public virtual void SetSlidingExpiration(bool slide)
        {
            throw new NotImplementedException();
        }

        public virtual void SetValidUntilExpires(bool validUntilExpires)
        {
            throw new NotImplementedException();
        }

        public virtual void SetVaryByCustom(string custom)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
