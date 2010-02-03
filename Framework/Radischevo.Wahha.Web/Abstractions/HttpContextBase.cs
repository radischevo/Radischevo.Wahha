using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Profile;

namespace Radischevo.Wahha.Web.Abstractions
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpContextBase : IServiceProvider
    {
        #region Constructors
        protected HttpContextBase()
        {
        }
        #endregion

        #region Instance Properties
        public virtual IEnumerable<Exception> AllErrors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpApplicationStateBase Application
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpApplication ApplicationInstance
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual Cache Cache
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IHttpHandler CurrentHandler
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual RequestNotification CurrentNotification
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Exception Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IHttpHandler Handler
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsCustomErrorEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsDebuggingEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsPostNotification
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IDictionary Items
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IHttpHandler PreviousHandler
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual ProfileBase Profile
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpRequestBase Request
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpResponseBase Response
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpServerUtilityBase Server
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpSessionStateBase Session
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool SkipAuthorization
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual DateTime Timestamp
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual TraceContext Trace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IPrincipal User
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Instance Methods
        public virtual void AddError(Exception errorInfo)
        {
            throw new NotImplementedException();
        }

        public virtual void ClearError()
        {
            throw new NotImplementedException();
        }

        public virtual object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            throw new NotImplementedException();
        }

        public virtual object GetGlobalResourceObject(string classKey, 
            string resourceKey, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public virtual object GetLocalResourceObject(string virtualPath, string resourceKey)
        {
            throw new NotImplementedException();
        }

        public virtual object GetLocalResourceObject(string virtualPath, 
            string resourceKey, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public virtual object GetSection(string sectionName)
        {
            throw new NotImplementedException();
        }

        public virtual object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public virtual void RewritePath(string path)
        {
            throw new NotImplementedException();
        }

        public virtual void RewritePath(string path, bool rebaseClientPath)
        {
            throw new NotImplementedException();
        }

        public virtual void RewritePath(string filePath, string pathInfo, string queryString)
        {
            throw new NotImplementedException();
        }

        public virtual void RewritePath(string filePath, string pathInfo, 
            string queryString, bool setClientFilePath)
        {
            throw new NotImplementedException();
        }

        public virtual HttpContext Unwrap()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
