using System;
using System.Runtime.Serialization;
using System.Web;

namespace Radischevo.Wahha.Web.Mvc
{
    public class RequestValidationException : HttpException
    {
        #region Constructors
        public RequestValidationException() 
            : base()
        {
        }

        public RequestValidationException(string message)
            : base(message)
        {
        }

        public RequestValidationException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        private RequestValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}
