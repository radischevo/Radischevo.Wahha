using System;
using System.IO;
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
    public sealed class HttpCompressionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        #region Constants
        private const string ACCEPT_ENCODING_HEADER = "Accept-Encoding";
        private const string CONTENT_ENCODING_HEADER = "Content-Encoding";
        private const string GZIP = "gzip";
        private const string DEFLATE = "deflate";
        private const string COMPRESS_ENABLED_KEY = "Radischevo.Wahha.HttpCompressionEnabled";
        #endregion

		#region Instance Fields
		private Stream _original;
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
		private bool CanAddCompressionFilter(ActionContext context)
		{
			if (context.Context.IsChild)
				return false;

			return (!context.HttpContext.Items.Contains(COMPRESS_ENABLED_KEY));
		}

		private void AddResponseFilter(HttpResponseBase response, 
			Func<Stream, Stream> factory)
		{
			_original = response.Filter;
			response.Filter = factory(response.Filter);
		}

		private void TryRemoveHeaders(HttpContextBase context)
		{
			try
			{
				context.Response.Headers[CONTENT_ENCODING_HEADER] = String.Empty;
			}
			catch (PlatformNotSupportedException)
			{
				// we get it if the application is executed under IIS < 7.
				// There's nothing we can do here.
			}
		}

		/// <summary>
        /// Enables the output compression 
        /// using gzip/deflate
        /// </summary>
        /// <param name="ctx">The context of the current controller action</param>
        public override void OnExecuted(ActionExecutedContext ctx)
        {
			HttpContextBase context = ctx.HttpContext;
            if (CanAddCompressionFilter(ctx))
            {
                if (IsEncodingAccepted(context, DEFLATE))
                {
					AddResponseFilter(context.Response, filter => 
						new DeflateStream(filter, CompressionMode.Compress));

                    SetEncoding(context, DEFLATE);
                }
                else if (IsEncodingAccepted(context, GZIP))
                {
					AddResponseFilter(context.Response, filter =>
						new GZipStream(filter, CompressionMode.Compress));
                    SetEncoding(context, GZIP);
                }
                context.Items.Add(COMPRESS_ENABLED_KEY, true);
				context.Response.AppendHeader("Vary", "Content-Encoding");
            }
        }
        
		public void OnException(ExceptionContext ctx)
		{
			HttpContextBase context = ctx.Context;

			// We should remove any response filters since
			// we'll get invalid response if an exception is thrown.
			if (context.Items.Contains(COMPRESS_ENABLED_KEY))
			{
				TryRemoveHeaders(context);
				context.Response.Filter = _original;
			}
		}
		#endregion
	}
}
