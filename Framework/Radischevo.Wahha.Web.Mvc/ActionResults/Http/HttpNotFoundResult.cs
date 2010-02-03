using System;
using System.Net;

namespace Radischevo.Wahha.Web.Mvc
{
    public class HttpNotFoundResult : HttpStatusResult
    {
        public HttpNotFoundResult(bool throwError)
            : base(HttpStatusCode.NotFound, "Not Found", throwError)
        {   }
    }
}
