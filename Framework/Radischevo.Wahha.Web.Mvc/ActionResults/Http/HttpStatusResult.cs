using System;
using System.Net;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class HttpStatusResult : EmptyResult
    {
        #region Instance Fields
        private int _statusCode;
        private string _message;
		private ValueDictionary _headers;
        #endregion

        #region Constructors
		public HttpStatusResult(HttpStatusCode statusCode, string message)
			: this(statusCode, message, null)
		{
		}

		public HttpStatusResult(int statusCode, string message)
			: this(statusCode, message, null)
		{
		}

		public HttpStatusResult(HttpStatusCode statusCode, string message, object headers)
			: this((int)statusCode, message, new ValueDictionary(headers))
		{
		}

		public HttpStatusResult(int statusCode, string message, object headers)
			: this(statusCode, message, new ValueDictionary(headers))
		{
		}

		public HttpStatusResult(HttpStatusCode statusCode, string message, IValueSet headers)
			: this((int)statusCode, message, headers)
		{
		}

        public HttpStatusResult(int statusCode, string message, IValueSet headers)
        {
			Precondition.Require(statusCode > 0,
				() => Error.ArgumentOutOfRange("statusCode"));

            _statusCode = statusCode;
            _message = message;
			_headers = new ValueDictionary(headers);
        }
        #endregion

		#region Instance Properties
		public int StatusCode
		{
			get
			{
				return _statusCode;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public ValueDictionary Headers
		{
			get
			{
				return _headers;
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

			response.Clear();
            response.StatusCode = StatusCode;
            response.StatusDescription = Message ?? String.Empty;
			response.TrySkipIisCustomErrors = true;

			foreach (string key in Headers.Keys)
				response.AppendHeader(key, Headers.GetValue<string>(key));
        }
        #endregion
    }
}
