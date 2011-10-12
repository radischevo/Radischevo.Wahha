using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Base class used to send binary content to the response
    /// </summary>
    public abstract class BinaryResult : ActionResult
    {
        #region Constants
        private const string _defaultContentType = "application/octet-stream";
        #endregion

        #region Instance Fields
        private string _contentType;
        private string _fileName;
        #endregion

        #region Constructors
        protected BinaryResult()
        { }
        #endregion

        #region Instance Properties
        /// <summary>
        /// The content type to use for the response.
        /// </summary>
        public string ContentType
        {
            get
            {
                if (String.IsNullOrEmpty(_contentType))
                    _contentType = _defaultContentType;
                
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        /// <summary>
        /// If specified, sets the content-disposition header 
        /// so that a file download dialog appears in 
        /// the browser with the specified file name.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
			if (context.IsChild)
				throw Error.CannotExecuteResultInChildAction();

            if (!String.IsNullOrEmpty(_contentType))
                context.Context.Response.ContentType = ContentType;

            if (!String.IsNullOrEmpty(_fileName))
                context.Context.Response.AddHeader("content-disposition",
                    String.Concat("attachment; filename=", HttpUtility.UrlPathEncode(_fileName)));

			Write(context.Context);
        }

		protected abstract void Write(HttpContextBase context);
        #endregion
    }
}
