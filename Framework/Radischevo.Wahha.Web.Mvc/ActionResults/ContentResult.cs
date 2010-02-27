using System;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ContentResult : ActionResult
    {
        #region Instance Fields
        private string _content;
        private Encoding _contentEncoding;
        private string _contentType;
        #endregion

        #region Instance Properties
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
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

        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            HttpResponseBase response = context.Context.Response;

            if (!String.IsNullOrEmpty(_contentType))
                response.ContentType = _contentType;
            
            if (_contentEncoding != null)
                response.ContentEncoding = _contentEncoding;
            
            if (_content != null)
                response.Write(_content);
        }
        #endregion
    }
}
