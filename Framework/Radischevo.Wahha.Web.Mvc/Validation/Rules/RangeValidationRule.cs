using System;
using System.ComponentModel.DataAnnotations;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class RangeValidationRule : DataAnnotationsValidationRule<RangeAttribute>
    {
        #region Constructors
        public RangeValidationRule(
            DataAnnotationsModelValidator validator, 
            RangeAttribute attribute)
            : base(validator, attribute)
        {
            ValidationType = "range";
            ErrorMessage = attribute.ErrorMessage;
            Parameters["minimum"] = attribute.Minimum;
            Parameters["maximum"] = attribute.Maximum;
        }
        #endregion
    }
}
