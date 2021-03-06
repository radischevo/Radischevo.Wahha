﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Caching;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that provides 
    /// HTTP-response information from an ASP.NET operation.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpResponseWrapper : HttpResponseBase
	{
		#region Static Fields
		private static IMethodInvoker _switchWriterMethod;
		#endregion

		#region Instance Fields
		private readonly HttpResponse _response;
        #endregion

        #region Constructors
		static HttpResponseWrapper()
		{
			string methodName = (Type.GetType("Mono.Runtime") == null) ? "SwitchWriter" : "SetTextWriter";
			MethodInfo method = typeof(HttpResponse).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			
			_switchWriterMethod = (method == null) ? null : method.CreateInvoker();
		}
		
        public HttpResponseWrapper(HttpResponse response)
        {
            Precondition.Require(response, () => Error.ArgumentNull("response"));
            _response = response;
        }
        #endregion

        #region Instance Properties
        public override bool Buffer
        {
            get
            {
                return _response.Buffer;
            }
            set
            {
                _response.Buffer = value;
            }
        }

        public override bool BufferOutput
        {
            get
            {
                return _response.BufferOutput;
            }
            set
            {
                _response.BufferOutput = value;
            }
        }

        public override HttpCachePolicyBase Cache
        {
            get
            {
                return new HttpCachePolicyWrapper(_response.Cache);
            }
        }

        public override string CacheControl
        {
            get
            {
                return _response.CacheControl;
            }
            set
            {
                _response.CacheControl = value;
            }
        }

        public override string Charset
        {
            get
            {
                return _response.Charset;
            }
            set
            {
                _response.Charset = value;
            }
        }

        public override Encoding ContentEncoding
        {
            get
            {
                return _response.ContentEncoding;
            }
            set
            {
                _response.ContentEncoding = value;
            }
        }

        public override string ContentType
        {
            get
            {
                return _response.ContentType;
            }
            set
            {
                _response.ContentType = value;
            }
        }

        public override HttpCookieCollection Cookies
        {
            get
            {
                return _response.Cookies;
            }
        }

        public override int Expires
        {
            get
            {
                return _response.Expires;
            }
            set
            {
                _response.Expires = value;
            }
        }

        public override DateTime ExpiresAbsolute
        {
            get
            {
                return _response.ExpiresAbsolute;
            }
            set
            {
                _response.ExpiresAbsolute = value;
            }
        }

        public override Stream Filter
        {
            get
            {
                return _response.Filter;
            }
            set
            {
                _response.Filter = value;
            }
        }

        public override Encoding HeaderEncoding
        {
            get
            {
                return _response.HeaderEncoding;
            }
            set
            {
                _response.HeaderEncoding = value;
            }
        }

        public override NameValueCollection Headers
        {
            get
            {
				return _response.Headers;
            }
        }

        public override bool IsClientConnected
        {
            get
            {
                return _response.IsClientConnected;
            }
        }

        public override bool IsRequestBeingRedirected
        {
            get
            {
                return _response.IsRequestBeingRedirected;
            }
        }

        public override TextWriter Output
        {
            get
            {
				return _response.Output;
            }
			set
			{
				Precondition.Require(value, () => Error.ArgumentNull("value"));
				
				if (_switchWriterMethod != null)
					_switchWriterMethod.Invoke(_response, value);
			}
        }

        public override Stream OutputStream
        {
            get
            {
                return _response.OutputStream;
            }
        }

        public override string RedirectLocation
        {
            get
            {
                return _response.RedirectLocation;
            }
            set
            {
                _response.RedirectLocation = value;
            }
        }

        public override string Status
        {
            get
            {
                return _response.Status;
            }
            set
            {
                _response.Status = value;
            }
        }

        public override int StatusCode
        {
            get
            {
                return _response.StatusCode;
            }
            set
            {
                _response.StatusCode = value;
            }
        }

        public override string StatusDescription
        {
            get
            {
                return _response.StatusDescription;
            }
            set
            {
                _response.StatusDescription = value;
            }
        }

        public override int SubStatusCode
        {
            get
            {
                return _response.SubStatusCode;
            }
            set
            {
                _response.SubStatusCode = value;
            }
        }

        public override bool SuppressContent
        {
            get
            {
                return _response.SuppressContent;
            }
            set
            {
                _response.SuppressContent = value;
            }
        }

        public override bool TrySkipIisCustomErrors
        {
            get
            {
                return _response.TrySkipIisCustomErrors;
            }
            set
            {
                _response.TrySkipIisCustomErrors = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void AddCacheDependency(params CacheDependency[] dependencies)
        {
            _response.AddCacheDependency(dependencies);
        }

        public override void AddCacheItemDependencies(ArrayList cacheKeys)
        {
            _response.AddCacheItemDependencies(cacheKeys);
        }

        public override void AddCacheItemDependencies(string[] cacheKeys)
        {
            _response.AddCacheItemDependencies(cacheKeys);
        }

        public override void AddCacheItemDependency(string cacheKey)
        {
            _response.AddCacheItemDependency(cacheKey);
        }

        public override void AddFileDependencies(string[] filenames)
        {
            _response.AddFileDependencies(filenames);
        }

        public override void AddFileDependencies(ArrayList filenames)
        {
            _response.AddFileDependencies(filenames);
        }

        public override void AddFileDependency(string filename)
        {
            _response.AddFileDependency(filename);
        }

        public override void AddHeader(string name, string value)
        {
            _response.AddHeader(name, value);
        }

        public override void AppendCookie(HttpCookie cookie)
        {
            _response.AppendCookie(cookie);
        }

        public override void AppendHeader(string name, string value)
        {
            _response.AppendHeader(name, value);
        }

        public override void AppendToLog(string param)
        {
            _response.AppendToLog(param);
        }

        public override string ApplyAppPathModifier(string virtualPath)
        {
            return _response.ApplyAppPathModifier(virtualPath);
        }

        public override void BinaryWrite(byte[] buffer)
        {
            _response.BinaryWrite(buffer);
        }

        public override void Clear()
        {
            _response.Clear();
        }

        public override void ClearContent()
        {
            _response.ClearContent();
        }

        public override void ClearHeaders()
        {
            _response.ClearHeaders();
        }

        public override void Close()
        {
            _response.Close();
        }

        public override void DisableKernelCache()
        {
            _response.DisableKernelCache();
        }

        public override void End()
        {
            _response.End();
        }

        public override void Flush()
        {
            _response.Flush();
        }

        public override void Pics(string value)
        {
            _response.Pics(value);
        }

        public override void Redirect(string url)
        {
            _response.Redirect(url);
        }

        public override void Redirect(string url, bool endResponse)
        {
            _response.Redirect(url, endResponse);
        }

        public override void RemoveOutputCacheItem(string path)
        {
            HttpResponse.RemoveOutputCacheItem(path);
        }

        public override void SetCookie(HttpCookie cookie)
        {
            _response.SetCookie(cookie);
        }

        public override void TransmitFile(string filename)
        {
            _response.TransmitFile(filename);
        }

        public override void TransmitFile(string filename, long offset, long length)
        {
            _response.TransmitFile(filename, offset, length);
        }

        public override void Write(char ch)
        {
            _response.Write(ch);
        }

        public override void Write(object obj)
        {
            _response.Write(obj);
        }

        public override void Write(string s)
        {
            _response.Write(s);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _response.Write(buffer, index, count);
        }

        public override void WriteFile(string filename)
        {
            _response.WriteFile(filename);
        }

        public override void WriteFile(string filename, bool readIntoMemory)
        {
            _response.WriteFile(filename, readIntoMemory);
        }

        public override void WriteFile(IntPtr fileHandle, long offset, long size)
        {
            _response.WriteFile(fileHandle, offset, size);
        }

        public override void WriteFile(string filename, long offset, long size)
        {
            _response.WriteFile(filename, offset, size);
        }

        public override void WriteSubstitution(HttpResponseSubstitutionCallback callback)
        {
            _response.WriteSubstitution(callback);
        }
        #endregion
    }
}
