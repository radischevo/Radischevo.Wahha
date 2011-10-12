using System;
using System.Net;

namespace Radischevo.Wahha.Web.Mvc
{
    public class HttpForbiddenResult : HttpStatusResult
	{
		#region Constructors
		public HttpForbiddenResult()
			: this(false)
		{
		}

		public HttpForbiddenResult(bool throwError)
			: base(HttpStatusCode.Forbidden, "Forbidden", throwError)
		{
		}
		#endregion		
    }
}
