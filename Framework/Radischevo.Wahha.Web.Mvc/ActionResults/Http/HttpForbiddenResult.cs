using System;
using System.Net;

namespace Radischevo.Wahha.Web.Mvc
{
    public class HttpForbiddenResult : HttpStatusResult
    {
        public HttpForbiddenResult(bool throwError)
            : base(HttpStatusCode.Forbidden, "Forbidden", throwError)
        {   }
    }
}
