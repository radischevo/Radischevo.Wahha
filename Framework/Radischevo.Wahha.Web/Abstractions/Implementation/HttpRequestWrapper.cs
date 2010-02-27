using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that enables ASP.NET to read 
    /// the HTTP values that are sent by a client during a Web request.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpRequestWrapper : HttpRequestBase
    {
        #region Instance Fields
        private readonly HttpRequest _request;
        private HttpParameters _parameters;
        #endregion

        #region Constructors
        public HttpRequestWrapper(HttpRequest request)
        {
            Precondition.Require(request, () => Error.ArgumentNull("request"));
            _request = request;
        }
        #endregion

        #region Instance Properties
        public override IEnumerable<string> AcceptTypes
        {
            get
            {
                return _request.AcceptTypes;
            }
        }

        public override string AnonymousID
        {
            get
            {
                return _request.AnonymousID;
            }
        }

        public override string ApplicationPath
        {
            get
            {
                return _request.ApplicationPath;
            }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                return _request.AppRelativeCurrentExecutionFilePath;
            }
        }

        public override HttpBrowserCapabilitiesBase Browser
        {
            get
            {
                return new HttpBrowserCapabilitiesWrapper(_request.Browser);
            }
        }

        public override HttpClientCertificate ClientCertificate
        {
            get
            {
                return _request.ClientCertificate;
            }
        }

        public override Encoding ContentEncoding
        {
            get
            {
                return _request.ContentEncoding;
            }
            set
            {
                _request.ContentEncoding = value;
            }
        }

        public override int ContentLength
        {
            get
            {
                return _request.ContentLength;
            }
        }

        public override string ContentType
        {
            get
            {
                return _request.ContentType;
            }
            set
            {
                _request.ContentType = value;
            }
        }

        public override HttpCookieCollection Cookies
        {
            get
            {
                return _request.Cookies;
            }
        }

        public override string CurrentExecutionFilePath
        {
            get
            {
                return _request.CurrentExecutionFilePath;
            }
        }

        public override string FilePath
        {
            get
            {
                return _request.FilePath;
            }
        }

        public override HttpFileCollectionBase Files
        {
            get
            {
                return new HttpFileCollectionWrapper(_request.Files);
            }
        }

        public override Stream Filter
        {
            get
            {
                return _request.Filter;
            }
            set
            {
                _request.Filter = value;
            }
        }

        public override IValueSet Form
        {
            get
            {
                return Parameters.Form;
            }
        }

        public override IValueSet Headers
        {
            get
            {
                return Parameters.Headers;
            }
        }

        public override HttpMethod HttpMethod
        {
            get
            {
                return (HttpMethod)Enum.Parse(typeof(HttpMethod), 
                    _request.HttpMethod, true);
            }
        }

        public override Stream InputStream
        {
            get
            {
                return _request.InputStream;
            }
        }

        public override bool IsAuthenticated
        {
            get
            {
                return _request.IsAuthenticated;
            }
        }

        public override bool IsLocal
        {
            get
            {
                return _request.IsLocal;
            }
        }

        public override bool IsSecureConnection
        {
            get
            {
                return _request.IsSecureConnection;
            }
        }

        public override object this[string key]
        {
            get
            {
                return _request[key];
            }
        }

        public override WindowsIdentity LogonUserIdentity
        {
            get
            {
                return _request.LogonUserIdentity;
            }
        }

        public override HttpParameters Parameters
        {
            get
            {
                if(_parameters == null)
                    _parameters = new HttpParameters(_request);

                return _parameters;
            }
        }

        public override string Path
        {
            get
            {
                return _request.Path;
            }
        }

        public override string PathInfo
        {
            get
            {
                return _request.PathInfo;
            }
        }

        public override string PhysicalApplicationPath
        {
            get
            {
                return _request.PhysicalApplicationPath;
            }
        }

        public override string PhysicalPath
        {
            get
            {
                return _request.PhysicalPath;
            }
        }

        public override IValueSet QueryString
        {
            get
            {
                return Parameters.QueryString;
            }
        }

        public override string RawUrl
        {
            get
            {
                return _request.RawUrl;
            }
        }

        public override string RequestType
        {
            get
            {
                return _request.RequestType;
            }
            set
            {
                _request.RequestType = value;
            }
        }

        public override NameValueCollection ServerVariables
        {
            get
            {
                return _request.ServerVariables;
            }
        }

        public override int TotalBytes
        {
            get
            {
                return _request.TotalBytes;
            }
        }

        public override Uri Url
        {
            get
            {
                return _request.Url;
            }
        }

        public override Uri UrlReferrer
        {
            get
            {
                return _request.UrlReferrer;
            }
        }

        public override string UserAgent
        {
            get
            {
                return _request.UserAgent;
            }
        }

        public override string UserHostAddress
        {
            get
            {
                return _request.UserHostAddress;
            }
        }

        public override string UserHostName
        {
            get
            {
                return _request.UserHostName;
            }
        }

        public override IEnumerable<string> UserLanguages
        {
            get
            {
                return _request.UserLanguages;
            }
        }
        #endregion

        #region Instance Methods
        public override byte[] BinaryRead(int count)
        {
            return _request.BinaryRead(count);
        }

        public override int[] MapImageCoordinates(string imageFieldName)
        {
            return _request.MapImageCoordinates(imageFieldName);
        }

        public override string MapPath(string virtualPath)
        {
            return _request.MapPath(virtualPath);
        }

        public override string MapPath(string virtualPath, 
            string baseVirtualDir, bool allowCrossAppMapping)
        {
            return _request.MapPath(virtualPath, baseVirtualDir, 
                allowCrossAppMapping);
        }

        public override void SaveAs(string filename, bool includeHeaders)
        {
            _request.SaveAs(filename, includeHeaders);
        }

        public override void ValidateInput()
        {
            _request.ValidateInput();
        }
        #endregion
    }
}
