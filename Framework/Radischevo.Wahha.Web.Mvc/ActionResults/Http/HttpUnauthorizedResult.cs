﻿using System;
using System.Net;

namespace Radischevo.Wahha.Web.Mvc
{
    public class HttpUnauthorizedResult : HttpStatusResult
    {
        public HttpUnauthorizedResult()
            : base(HttpStatusCode.Unauthorized, "Not Authorized", false)
        {   }
    }
}
