using System;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
        Inherited = true, AllowMultiple = false)]
    public sealed class ValidateRequestTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region Constants
        internal const string ValidationFieldName = "__digest";
        #endregion

        #region Instance Fields
        private string _value;
        private int _timeout;
        #endregion

        #region Constructors
        public ValidateRequestTokenAttribute()
            : this(null)
        {   }

        public ValidateRequestTokenAttribute(string value)
            : base()
        {
            _value = value;
        }
        #endregion

        #region Instance Properties
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }
        #endregion

        #region IAuthorizationFilter Members
        public void OnAuthorization(AuthorizationContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            string value = context.Context.Request.Form.GetValue<string>(ValidationFieldName);

            if (String.IsNullOrEmpty(value))
                throw Error.RequestValidationError();

            RequestValidationToken current = RequestValidationToken.Create(context.Context);
            RequestValidationToken token = RequestValidationToken.Create(value);

            if(!token.IsValid(current))
                throw Error.RequestValidationError();

            if (_timeout > 0 && (DateTime.Now - token.CreationDate).TotalMinutes > _timeout)
                throw Error.RequestValidationError();    
        }
        #endregion
    }
}
