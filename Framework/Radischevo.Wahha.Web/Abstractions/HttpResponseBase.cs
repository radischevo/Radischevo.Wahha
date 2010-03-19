using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Serves as the base class for classes that provides 
    /// HTTP-response information from an ASP.NET operation.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpResponseBase
    {
        #region Constructors
        /// <summary>
        /// Initializes the class for use by an inherited class 
        /// instance. This constructor can only be called by an inherited class.
        /// </summary>
        protected HttpResponseBase()
        {
        }
        #endregion

        #region Instance Properties
        public virtual bool Buffer
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

        public virtual bool BufferOutput
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

        public virtual HttpCachePolicyBase Cache
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string CacheControl
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

        public virtual string Charset
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

        public virtual Encoding ContentEncoding
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

        public virtual string ContentType
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

        public virtual HttpCookieCollection Cookies
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual int Expires
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

        public virtual DateTime ExpiresAbsolute
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

        public virtual Stream Filter
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

        public virtual Encoding HeaderEncoding
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

        public virtual NameValueCollection Headers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsClientConnected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsRequestBeingRedirected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual TextWriter Output
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

        public virtual Stream OutputStream
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string RedirectLocation
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

        public virtual string Status
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

        public virtual int StatusCode
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

        public virtual string StatusDescription
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

        public virtual int SubStatusCode
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

        public virtual bool SuppressContent
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

        public virtual bool TrySkipIisCustomErrors
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
        public virtual void AddCacheDependency(params CacheDependency[] dependencies)
        {
            throw new NotImplementedException();
        }

        public virtual void AddCacheItemDependencies(ArrayList cacheKeys)
        {
            throw new NotImplementedException();
        }

        public virtual void AddCacheItemDependencies(string[] cacheKeys)
        {
            throw new NotImplementedException();
        }

        public virtual void AddCacheItemDependency(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public virtual void AddFileDependencies(ArrayList filenames)
        {
            throw new NotImplementedException();
        }

        public virtual void AddFileDependencies(string[] filenames)
        {
            throw new NotImplementedException();
        }

        public virtual void AddFileDependency(string filename)
        {
            throw new NotImplementedException();
        }

        public virtual void AddHeader(string name, string value)
        {
            throw new NotImplementedException();
        }

        public virtual void AppendCookie(HttpCookie cookie)
        {
            throw new NotImplementedException();
        }

        public virtual void AppendHeader(string name, string value)
        {
            throw new NotImplementedException();
        }

        public virtual void AppendToLog(string param)
        {
            throw new NotImplementedException();
        }

        public virtual string ApplyAppPathModifier(string virtualPath)
        {
            throw new NotImplementedException();
        }

        public virtual void BinaryWrite(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual void ClearContent()
        {
            throw new NotImplementedException();
        }

        public virtual void ClearHeaders()
        {
            throw new NotImplementedException();
        }

        public virtual void Close()
        {
            throw new NotImplementedException();
        }

        public virtual void DisableKernelCache()
        {
            throw new NotImplementedException();
        }

        public virtual void End()
        {
            throw new NotImplementedException();
        }

        public virtual void Flush()
        {
            throw new NotImplementedException();
        }

        public virtual void Pics(string value)
        {
            throw new NotImplementedException();
        }

        public virtual void Redirect(string url)
        {
            throw new NotImplementedException();
        }

        public virtual void Redirect(string url, bool endResponse)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveOutputCacheItem(string path)
        {
            throw new NotImplementedException();
        }

        public virtual void SetCookie(HttpCookie cookie)
        {
            throw new NotImplementedException();
        }

        public virtual void TransmitFile(string filename)
        {
            throw new NotImplementedException();
        }

        public virtual void TransmitFile(string filename, long offset, long length)
        {
            throw new NotImplementedException();
        }

        public virtual void Write(char ch)
        {
            throw new NotImplementedException();
        }

        public virtual void Write(object obj)
        {
            throw new NotImplementedException();
        }

        public virtual void Write(string s)
        {
            throw new NotImplementedException();
        }

        public virtual void Write(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteFile(string filename)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteFile(string filename, bool readIntoMemory)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteFile(IntPtr fileHandle, long offset, long size)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteFile(string filename, long offset, long size)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteSubstitution(HttpResponseSubstitutionCallback callback)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
