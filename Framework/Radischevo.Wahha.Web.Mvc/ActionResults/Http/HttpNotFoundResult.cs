using System;
using System.Net;

namespace Radischevo.Wahha.Web.Mvc
{
    public class HttpNotFoundResult : HttpStatusResult
	{
		#region Constructors
		public HttpNotFoundResult()
			: this(false)
		{
		}

		public HttpNotFoundResult(bool throwError)
            : base(HttpStatusCode.NotFound, "Not Found", throwError)
        {
		}
		#endregion
	}
}
