using System;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class JavaScriptResult : ActionResult
    {
        #region Instance Fields
        private string _script;
        private Encoding _contentEncoding;
        #endregion

		#region Constructors
		public JavaScriptResult()
			: this(null, null)
		{
		}

		public JavaScriptResult(string script)
			: this(script, null)
		{
		}

		public JavaScriptResult(string script, Encoding contentEncoding)
		{
			_script = script;
			_contentEncoding = contentEncoding;
		}
		#endregion

		#region Instance Properties
		public string Script
        {
            get
            {
                return _script;
            }
            set
            {
                _script = value;
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

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
			if (context.IsChild)
				throw Error.CannotExecuteResultInChildAction();

            HttpResponseBase response = context.Context.Response;
            response.ContentType = "application/x-javascript";
            
            if (_contentEncoding != null)
                response.ContentEncoding = _contentEncoding;
            
            if (!String.IsNullOrEmpty(_script))
                response.Write(_script);
        }
        #endregion
    }
}
