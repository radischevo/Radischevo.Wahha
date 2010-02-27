using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class FormValidationMetadata
    {
        #region Instance Fields
        private List<ClientModelValidationRule> _rules;
        #endregion

        #region Constructors
        public FormValidationMetadata()
            : this(null)
        {
        }

        public FormValidationMetadata(
            IEnumerable<ClientModelValidationRule> rules)
        {
            _rules = GetValidationRules(rules);
        }
        #endregion

        #region Instance Properties
        public IEnumerable<ClientModelValidationRule> Rules
        {
            get
            {
                return _rules;
            }
        }
        #endregion

        #region Static Methods
        private static List<ClientModelValidationRule> GetValidationRules(
            IEnumerable<ClientModelValidationRule> rules)
        {
            if (rules == null)
                return new List<ClientModelValidationRule>();

            return new List<ClientModelValidationRule>(rules);
        }
        #endregion

        #region Instance Methods
        public FormValidationMetadata Append(
            Func<ClientModelValidationRuleBuilder, ClientModelValidationRule> rule)
        {
            Precondition.Require(rule, () => Error.ArgumentNull("rule"));
            return Append(rule(new ClientModelValidationRuleBuilder()));
        }

        public FormValidationMetadata Append(ModelValidationRule rule)
        {
			if(rule.SupportsClientValidation)
				return Append(new ClientModelValidationRule(rule));

			return this;
        }

        public FormValidationMetadata Append(ClientModelValidationRule rule)
        {
            Precondition.Require(rule, () => Error.ArgumentNull("rule"));
            _rules.Add(rule);

            return this;
        }
        #endregion

        #region Serialization
        public string Apply(string selector)
        {
            return Apply(selector, null);
        }

        public string Apply(string selector, 
            IModelValidationRuleFormatter formatter)
        {
            Precondition.Defined(selector,
				() => Error.ArgumentNull("selector"));

            if (formatter == null)
                formatter = new JsonModelValidationRuleFormatter();

            return formatter.Format(selector, Rules);
        }
        #endregion
    }
}
