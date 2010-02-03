using System;
using System.ComponentModel.DataAnnotations;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class RequiredFieldValidationRule : 
        DataAnnotationsValidationRule<RequiredAttribute>
    {
        #region Constructors
        public RequiredFieldValidationRule(
            DataAnnotationsModelValidator validator, 
            RequiredAttribute attribute)
            : base(validator, attribute)
        {
            ValidationType = "required";
            ErrorMessage = attribute.ErrorMessage;
        }
        #endregion
    }
}