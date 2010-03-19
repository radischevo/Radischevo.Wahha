using System;
using System.IO;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Renders an Action delegate and captures all output to a string.
    /// </summary>
    internal class BlockRenderer
    {
        #region Instance Fields
        private readonly HttpContextBase _context;
        #endregion

        #region Constructors
        public BlockRenderer(HttpContextBase context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            _context = context;
        }
        #endregion

        #region Instance Methods
        /// <summary>Renders the action and returns a string.</summary>
        /// <param name="renderer">The delegate to render.</param>
        internal string Capture(Action renderer)
        {
            HttpResponseBase response = _context.Response;
            Stream originalFilter = null;
            CaptureFilter filter;
            string html = "";

            if (renderer != null)
            {
                try
                {
					_context.Session.UpdateSessionId();

                    response.Flush();
                    originalFilter = response.Filter;
                    filter = new CaptureFilter(response.Filter);
                    
                    response.Filter = filter;
                    renderer();
                    response.Flush();

                    html = filter.GetContents(response.ContentEncoding);
                }
                finally
                {
                    if (originalFilter != null)
                        response.Filter = originalFilter;
                }
            }
            return html;
        }
        #endregion
    }
}
