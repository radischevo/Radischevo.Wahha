using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class ModelValidationRule
    {
        #region Instance Fields
		private ModelValidator _validator;
        private IDictionary<string, object> _parameters;
        private string _validationType;
        private string _errorMessage;
        private string _member;
        #endregion

        #region Constructors
        public ModelValidationRule(ModelValidator validator)
        {
            Precondition.Require(validator, () => Error.ArgumentNull("validator"));

			_validator = validator;
            _member = validator.PropertyName;

            _parameters = new Dictionary<string, object>(
                StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Instance Properties
        public IDictionary<string, object> Parameters
        {
            get
            {
                return _parameters;
            }
        }

		public ModelValidator Validator
		{
			get
			{
				return _validator;
			}
		}

        public string ValidationType
        {
            get
            {
                return _validationType ?? String.Empty;
            }
            set
            {
                _validationType = value;
            }
        }

        public string Member
        {
            get
            {
                return _member;
            }
            set
            {
                _member = value;
            }
        }

        public virtual string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

		public virtual bool SupportsClientValidation
		{
			get
			{
				return true;
			}
		}
        #endregion

        #region Instance Methods
        public virtual ModelValidationResult Validate(ModelValidationContext context)
        {
            return null;
        }
        #endregion
    }
}
