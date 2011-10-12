using System;
using System.IO;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class StaticFileResult : BinaryResult
    {
        #region Instance Fields
        private string _path;
        private Encoding _contentEncoding;
        #endregion

        #region Constructors
        public StaticFileResult(string path)
            : this(path, null, null, null)
        {   }

        public StaticFileResult(string path, string fileName)
            : this(path, fileName, null, null)
        {   }

        public StaticFileResult(string path, string fileName, 
            string contentType)
            : this(path, fileName, contentType, null)
        { }

        public StaticFileResult(string path,
            string fileName, string contentType, 
            Encoding contentEncoding) 
			: base()
        {
            Precondition.Defined(path, () => Error.ArgumentNull("path"));

            _path = path;
            _contentEncoding = contentEncoding;

            base.ContentType = contentType;
            base.FileName = fileName;
        }
        #endregion

        #region Instance Properties
        public string Path
        {
            get
            {
                return _path;
            }
        }

        public Encoding ContentEncoding
        {
            get
            {
                return _contentEncoding;
            }
            set
            {
                _contentEncoding = value;
            }
        }
        #endregion

        #region Static Methods
        private static string ResolvePath(HttpContextBase context, string virtualUrl)
        {
			if (!virtualUrl.StartsWith("~/", StringComparison.Ordinal))
				return virtualUrl;
   
			virtualUrl = virtualUrl.Remove(0, 2);
            string appPath = context.Request.ApplicationPath;

            if (String.IsNullOrEmpty(appPath))
                appPath = "/";

            if (!appPath.EndsWith("/"))
                appPath = String.Concat(appPath, "/");

            return String.Concat(appPath, virtualUrl);
        }
        #endregion

        #region Instance Methods
        protected override void Write(HttpContextBase context)
        {
            if (_contentEncoding != null)
                context.Response.ContentEncoding = _contentEncoding;

            string filePath = context.Server.MapPath(
				ResolvePath(context, _path));

            FileInfo fi = new FileInfo(filePath);

            if (fi.Exists)
            {
                context.Response.AddHeader("Last-Modified",
                    fi.LastWriteTimeUtc.ToString("r"));
                context.Response.AddHeader("Content-Length",
                    fi.Length.ToString());
            }
            context.Response.TransmitFile(filePath);
        }
        #endregion
    }
}
