using System;
using System.IO.Compression;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Enables the output compression 
    /// using standard gzip/deflate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class HttpCompressionAttribute : ActionFilterAttribute
    {
        #region Instance Fields
        private const string ACCEPT_ENCODING_HEADER = "Accept-Encoding";
        private const string CONTENT_ENCODING_HEADER = "Content-Encoding";
        private const string GZIP = "gzip";
        private const string DEFLATE = "deflate";
        private const string COMPRESS_ENABLED_KEY = "Radischevo.Wahha.HttpCompressionEnabled";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes an instance of the 
        /// <see cref="T:Radischevo.Wahha.Web.Mvc.HttpCompressionAttribute"/> class
        /// </summary>
        public HttpCompressionAttribute()
            : base()
        {
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Checks the request headers to see if the specified
        /// encoding is accepted by the client.
        /// </summary>
        private static bool IsEncodingAccepted(HttpContextBase context, string encoding)
        {
            string encodings = context.Request.Headers.GetValue<string>(ACCEPT_ENCODING_HEADER);
            return (encodings != null && encodings.Contains(encoding, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Adds the specified encoding to the response headers.
        /// </summary>
        /// <param name="context">The context of the current controller action</param>
        /// <param name="encoding"></param>
        private static void SetEncoding(HttpContextBase context, string encoding)
        {
            context.Response.AppendHeader(CONTENT_ENCODING_HEADER, encoding);
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Enables the output compression 
        /// using gzip/deflate
        /// </summary>
        /// <param name="ctx">The context of the current controller action</param>
        public virtual void OnExecuted(ActionExecutedContext ctx)
        {
            HttpContextBase context = ctx.Context;
            if (!context.Items.Contains(COMPRESS_ENABLED_KEY))
            {
                if (IsEncodingAccepted(context, DEFLATE))
                {
                    context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress);
                    SetEncoding(context, DEFLATE);
                }
                else if (IsEncodingAccepted(context, GZIP))
                {
                    context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                    SetEncoding(context, GZIP);
                }
                context.Items.Add(COMPRESS_ENABLED_KEY, true);
            }
        }
        #endregion
    }
}
