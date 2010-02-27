using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class ClientModelValidationRuleBuilder
    {
        #region Constructors
        public ClientModelValidationRuleBuilder()
        {
        }
        #endregion

        #region Instance Methods
        public ClientModelValidationRule Required(string field, string errorMessage)
        {
            return new ClientModelValidationRule("required", field, errorMessage);
        }

        public ClientModelValidationRule Range<T>(string field, 
            T minimum, T maximum, string errorMessage)
        {
            return new ClientModelValidationRule("range", field, errorMessage,
                new { minimum = minimum, maximum = maximum });
        }

        public ClientModelValidationRule Regex(string field, string pattern,
            string errorMessage)
        {
            Precondition.Defined(pattern, () => Error.ArgumentNull("pattern"));

            return new ClientModelValidationRule("regex", field, errorMessage,
                new { pattern = pattern });
        }

        public ClientModelValidationRule Length(string field, int maximumLength,
            string errorMessage)
        {
            Precondition.Require(maximumLength > -1,
				() => Error.ArgumentOutOfRange("maximumLength"));

            return new ClientModelValidationRule("stringLength", field,
                errorMessage, new { minimumLength = 0, maximumLength = maximumLength });
        }

        public ClientModelValidationRule Remote(string field, string url,
            string errorMessage)
        {
            Precondition.Defined(url, () => Error.ArgumentNull("url"));

            return new ClientModelValidationRule("remote", field,
                errorMessage, new { url = url });
        }

        public ClientModelValidationRule Equal(string field, 
            string comparedField, string errorMessage)
        {
			Precondition.Defined(comparedField, () => Error.ArgumentNull("comparedField"));

            return new ClientModelValidationRule("equalTo", field, 
                errorMessage, new { selector = comparedField });
        }
        #endregion
    }
}
