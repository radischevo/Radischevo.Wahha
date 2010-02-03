using System;
using System.IO.Compression;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web
{
    public class HttpCompressionModule : IHttpModule
    {
        #region Static Fields
        private const string ACCEPT_ENCODING_HEADER = "Accept-Encoding";
        private const string CONTENT_ENCODING_HEADER = "Content-Encoding";
        private const string GZIP_HEADER_VALUE = "gzip";
        private const string DEFLATE_HEADER_VALUE = "deflate";
        private const string COMPRESS_ENABLED_KEY = "Radischevo.Wahha.HttpCompressionEnabled";
        #endregion

        #region Static Methods
        /// <summary>
        /// Checks the request headers to see if the specified
        /// encoding is accepted by the client.
        /// </summary>
        private static bool IsEncodingAccepted(HttpContext context, string encoding)
        {
            return context.Request.Headers[ACCEPT_ENCODING_HEADER] != null &&
                context.Request.Headers[ACCEPT_ENCODING_HEADER].Contains(encoding);
        }

        /// <summary>
        /// Adds the specified encoding to the response headers.
        /// </summary>
        /// <param name="encoding"></param>
        private static void SetEncoding(HttpContext context, string encoding)
        {
            context.Response.AppendHeader(CONTENT_ENCODING_HEADER, encoding);
        }
        #endregion

        #region Instance Methods
        private void RegisterCompressFilter(HttpContext context)
        {
            if (context.Handler is DefaultHttpHandler)
                return;

            HttpResponse response = context.Response;

            if (!context.Items.Contains(COMPRESS_ENABLED_KEY))
            {
                if (IsEncodingAccepted(context, GZIP_HEADER_VALUE))
                {
                    SetEncoding(context, GZIP_HEADER_VALUE);
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                else if (IsEncodingAccepted(context, DEFLATE_HEADER_VALUE))
                {
                    SetEncoding(context, DEFLATE_HEADER_VALUE);
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
                context.Items.Add(COMPRESS_ENABLED_KEY, true);
            }
        }
        #endregion

        #region Event Handlers
        private void OnPostAcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            RegisterCompressFilter(app.Context);
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            app.PostAcquireRequestState -= new EventHandler(OnPostAcquireRequestState);
            app.EndRequest -= new EventHandler(OnEndRequest);
        }
        #endregion

        #region IHttpModule Members
        void IHttpModule.Init(HttpApplication context)
        {
            context.PostAcquireRequestState += new EventHandler(OnPostAcquireRequestState);
            context.EndRequest += new EventHandler(OnEndRequest);
        }

        void IHttpModule.Dispose()
        { }
        #endregion
    }
}
