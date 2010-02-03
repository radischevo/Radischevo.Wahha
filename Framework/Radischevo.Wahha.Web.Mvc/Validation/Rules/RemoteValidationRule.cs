using System;
using System.ComponentModel.DataAnnotations;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class RemoteValidationRule :
        DataAnnotationsValidationRule<RemoteValidationAttribute>
    {
        #region Constructors
        public RemoteValidationRule(
            DataAnnotationsModelValidator validator,
            RemoteValidationAttribute attribute)
            : base(validator, attribute)
        {
            ValidationType = "remote";
            ErrorMessage = attribute.ErrorMessage;
            Parameters["url"] = attribute.Url;
        }
        #endregion
    }
}