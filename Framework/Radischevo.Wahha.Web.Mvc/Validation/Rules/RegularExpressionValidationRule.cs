using System;
using System.ComponentModel.DataAnnotations;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class RegularExpressionValidationRule : 
        DataAnnotationsValidationRule<RegularExpressionAttribute>
    {
        #region Constructors
        public RegularExpressionValidationRule(
            DataAnnotationsModelValidator validator, 
            RegularExpressionAttribute attribute)
            : base(validator, attribute)
        {
            ValidationType = "regularExpression";
            ErrorMessage = attribute.ErrorMessage;
            Parameters["pattern"] = attribute.Pattern;
        }
        #endregion
    }
}