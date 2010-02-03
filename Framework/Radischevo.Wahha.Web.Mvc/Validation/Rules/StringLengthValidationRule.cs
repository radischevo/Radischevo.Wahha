using System;
using System.ComponentModel.DataAnnotations;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class StringLengthValidationRule : DataAnnotationsValidationRule<StringLengthAttribute>
    {
        #region Constructors
        public StringLengthValidationRule(
            DataAnnotationsModelValidator validator, 
            StringLengthAttribute attribute)
            : base(validator, attribute)
        {
            ValidationType = "stringLength";
            ErrorMessage = attribute.ErrorMessage;
            Parameters["minimumLength"] = 0;
            Parameters["maximumLength"] = attribute.MaximumLength;
        }
        #endregion
    }
}