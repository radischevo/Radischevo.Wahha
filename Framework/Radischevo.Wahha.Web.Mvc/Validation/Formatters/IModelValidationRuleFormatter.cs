using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public interface IModelValidationRuleFormatter
    {
        #region Instance Methods
        string Format(string formName, 
            IEnumerable<ClientModelValidationRule> rules);
        #endregion
    }
}
