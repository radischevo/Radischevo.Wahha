using System;
using System.ComponentModel.DataAnnotations;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class DataAnnotationsValidationRule : ModelValidationRule
    {
        #region Instance Fields
        private ValidationAttribute _attribute;
        #endregion

        #region Constructors
        public DataAnnotationsValidationRule(
            DataAnnotationsModelValidator validator, 
            ValidationAttribute attribute)
            : base(validator)
        {
            _attribute = attribute;            
        }
        #endregion

        #region Instance Properties
        public ValidationAttribute Attribute
        {
            get
            {
                return _attribute;
            }
        }
        #endregion

        #region Instance Methods
		public override ModelValidationResult Validate(ModelValidationContext context)
        {
            if (!Attribute.IsValid(context.Value))
                return new ModelValidationResult { Message = Attribute.FormatErrorMessage(Member) };

            return null;
        }
        #endregion
    }

    public class DataAnnotationsValidationRule<TAttribute> : DataAnnotationsValidationRule
        where TAttribute : ValidationAttribute
    {
        #region Constructors
        public DataAnnotationsValidationRule(
            DataAnnotationsModelValidator validator, 
            ValidationAttribute attribute)
            : base(validator, attribute)
        {
        }
        #endregion

        #region Instance Properties
        public new TAttribute Attribute
        {
            get
            {
                return (TAttribute)base.Attribute;
            }
        }
        #endregion
    }
}
