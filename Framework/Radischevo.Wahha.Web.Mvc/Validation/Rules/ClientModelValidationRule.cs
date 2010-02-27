using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class ClientModelValidationRule
    {
        #region Instance Fields
        private string _field;
        private string _validationType;
        private string _errorMessage;
        private IDictionary<string, object> _parameters;
        #endregion

        #region Constructors
        public ClientModelValidationRule(string type,
            string field, string errorMessage)
            : this(type, field, errorMessage, 
                (IDictionary<string, object>)null)
        {
        }

        public ClientModelValidationRule(string type,
            string field, string errorMessage, object parameters)
            : this(type, field, errorMessage, 
                new ValueDictionary(parameters))
        {
        }

        public ClientModelValidationRule(string type, 
            string field, string errorMessage, 
            IDictionary<string, object> parameters) 
        {
            _validationType = type;
            _field = field;
            _errorMessage = errorMessage;

            CopyParameters(parameters);
        }

        public ClientModelValidationRule(ModelValidationRule rule)
        {
            Precondition.Require(rule, () => Error.ArgumentNull("rule"));

			_field = rule.Member;
            _errorMessage = rule.ErrorMessage;
            _validationType = rule.ValidationType;

            CopyParameters(rule.Parameters);
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

        public string Field
        {
            get
            {
                return _field ?? String.Empty;
            }
            set
            {
                _field = value;
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
        #endregion

        #region Instance Methods
        private void CopyParameters(IDictionary<string, object> source)
        {
            _parameters = new Dictionary<string, object>(
                StringComparer.InvariantCultureIgnoreCase);

            if (source == null)
                return;
            
            foreach (KeyValuePair<string, object> pair in source)
                _parameters.Add(pair);
        }
        #endregion
    }
}
