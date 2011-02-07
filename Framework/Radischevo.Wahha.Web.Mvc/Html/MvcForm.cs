using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Text;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public enum FormMethod : int
    {
        Get = 0,
        Post
    }

    public class MvcForm : IDisposable
    {
        #region Instance Fields
        private bool _disposed;
        private HttpResponseBase _response;
        private FormMethod _method;
        #endregion

        #region Constructors
        public MvcForm(HttpResponseBase response)
        {
            Precondition.Require(response, () => Error.ArgumentNull("response"));
            _response = response;
        }

        public MvcForm(HttpResponseBase response, FormMethod method)
        {
            Precondition.Require(response, () => Error.ArgumentNull("response"));
            _response = response;
            _method = method;
        }
        #endregion

        #region Instance Methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                HtmlElementBuilder builder = new HtmlElementBuilder("form");
                _response.Write(builder.ToString(HtmlElementRenderMode.EndTag));
            }
        }
        #endregion
    }
}
