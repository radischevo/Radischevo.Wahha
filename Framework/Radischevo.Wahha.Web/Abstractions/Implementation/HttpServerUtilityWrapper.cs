using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that provides 
    /// helper methods for processing Web requests.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpServerUtilityWrapper : HttpServerUtilityBase
    {
        #region Instance Fields
        private readonly HttpServerUtility _server;
        #endregion

        #region Constructors
        public HttpServerUtilityWrapper(HttpServerUtility server)
        {
            Precondition.Require(server, Error.ArgumentNull("server"));
            _server = server;
        }
        #endregion

        #region Instance Properties
        public override string MachineName
        {
            get
            {
                return _server.MachineName;
            }
        }

        public override int ScriptTimeout
        {
            get
            {
                return _server.ScriptTimeout;
            }
            set
            {
                _server.ScriptTimeout = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void ClearError()
        {
            _server.ClearError();
        }

        public override object CreateObject(string progID)
        {
            return _server.CreateObject(progID);
        }

        public override object CreateObject(Type type)
        {
            return _server.CreateObject(type);
        }

        public override object CreateObjectFromClsid(string clsid)
        {
            return _server.CreateObjectFromClsid(clsid);
        }

        public override void Execute(string path)
        {
            _server.Execute(path);
        }

        public override void Execute(string path, bool preserveForm)
        {
            _server.Execute(path, preserveForm);
        }

        public override void Execute(string path, TextWriter writer)
        {
            _server.Execute(path, writer);
        }

        public override void Execute(string path, TextWriter writer, 
            bool preserveForm)
        {
            _server.Execute(path, writer, preserveForm);
        }

        public override void Execute(IHttpHandler handler, 
            TextWriter writer, bool preserveForm)
        {
            _server.Execute(handler, writer, preserveForm);
        }

        public override Exception GetLastError()
        {
            return _server.GetLastError();
        }

        public override string HtmlDecode(string s)
        {
            return _server.HtmlDecode(s);
        }

        public override void HtmlDecode(string s, TextWriter output)
        {
            _server.HtmlDecode(s, output);
        }

        public override string HtmlEncode(string s)
        {
            return _server.HtmlEncode(s);
        }

        public override void HtmlEncode(string s, TextWriter output)
        {
            _server.HtmlEncode(s, output);
        }

        public override string MapPath(string path)
        {
            return _server.MapPath(path);
        }

        public override void Transfer(string path)
        {
            _server.Transfer(path);
        }

        public override void Transfer(string path, bool preserveForm)
        {
            _server.Transfer(path, preserveForm);
        }

        public override void Transfer(IHttpHandler handler, 
            bool preserveForm)
        {
            _server.Transfer(handler, preserveForm);
        }

        public override void TransferRequest(string path)
        {
            _server.TransferRequest(path);
        }

        public override void TransferRequest(string path, bool preserveForm)
        {
            _server.TransferRequest(path, preserveForm);
        }

        public override void TransferRequest(string path, bool preserveForm, 
            string method, NameValueCollection headers)
        {
            _server.TransferRequest(path, preserveForm, method, headers);
        }

        public override string UrlDecode(string s)
        {
            return _server.UrlDecode(s);
        }

        public override void UrlDecode(string s, TextWriter output)
        {
            _server.UrlDecode(s, output);
        }

        public override string UrlEncode(string s)
        {
            return _server.UrlEncode(s);
        }

        public override void UrlEncode(string s, TextWriter output)
        {
            _server.UrlEncode(s, output);
        }

        public override string UrlPathEncode(string s)
        {
            return _server.UrlPathEncode(s);
        }

        public override byte[] UrlTokenDecode(string input)
        {
            return HttpServerUtility.UrlTokenDecode(input);
        }

        public override string UrlTokenEncode(byte[] input)
        {
            return HttpServerUtility.UrlTokenEncode(input);
        }
        #endregion
    }
}
