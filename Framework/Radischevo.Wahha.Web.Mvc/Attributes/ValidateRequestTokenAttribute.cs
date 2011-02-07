using System;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
        Inherited = true, AllowMultiple = false)]
    public sealed class ValidateRequestTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region Instance Fields
		private string _fieldName;
        private string _value;
        private int _timeout;
        #endregion

        #region Constructors
        public ValidateRequestTokenAttribute(string fieldName)
            : this(fieldName, null)
        {   }

        public ValidateRequestTokenAttribute(string fieldName, string value)
            : base()
        {
			Precondition.Defined(fieldName, () => Error.ArgumentNull("fieldName"));

			_fieldName = fieldName;
            _value = value;
        }
        #endregion

        #region Instance Properties
		public string FieldName
		{
			get
			{
				return _fieldName;
			}
			set
			{
				_fieldName = value;
			}
		}
		
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

        #region Instance Methods
        public void OnAuthorization(AuthorizationContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
			string value = context.HttpContext.Request.Parameters.Form.GetValue<string>(FieldName);

            if (String.IsNullOrEmpty(value))
                throw Error.RequestValidationError();

			RequestValidationToken current = RequestValidationToken.Create(context.HttpContext);
            RequestValidationToken token = RequestValidationToken.Create(value);

            if(!token.IsValid(current))
                throw Error.RequestValidationError();

            if (_timeout > 0 && (DateTime.Now - token.CreationDate).TotalMinutes > _timeout)
                throw Error.RequestValidationError();    
        }
        #endregion
    }
}
