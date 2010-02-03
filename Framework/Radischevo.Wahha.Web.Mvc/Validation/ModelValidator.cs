using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class ModelValidator
    {
        #region Instance Fields
        private ModelValidator _container;
        private string _propertyName;
        private Type _type;
        private IEnumerable<ModelValidator> _properties;
        #endregion

        #region Constructors
        public ModelValidator(Type modelType)
            : this(null, null, modelType)
        {
        }

        public ModelValidator(ModelValidator container, 
            string propertyName, Type modelType)
        {
            Precondition.Require(modelType, Error.ArgumentNull("modelType"));

            _container = container;
            _propertyName = propertyName;
            _type = modelType;
        }
        #endregion

        #region Instance Properties
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public ModelValidator Container
        {
            get
            {
                return _container;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }

        public IEnumerable<ModelValidator> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = CreateValidatorsForProperties();

                return _properties;
            }
        }
        #endregion

        #region Instance Methods
        protected IEnumerable<ModelValidator> CreateValidatorsForProperties()
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_type))
                yield return CreateValidatorForProperty(property);
        }

        protected virtual ModelValidator CreateValidatorForProperty(PropertyDescriptor property)
        {
            return new ModelValidator(this, property.Name, property.PropertyType);
        }

        public virtual IEnumerable<ModelValidationRule> GetValidationRules()
        {
            return Enumerable.Empty<ModelValidationRule>();
        }

        public virtual IEnumerable<ModelValidationResult> Validate(
            ControllerContext context, object container)
        {
            foreach (ModelValidationRule rule in GetValidationRules())
            {
                ModelValidationResult result = rule.Validate(context, container);
                if (result != null)
                    yield return result;
            }
        }

        public ModelValidator GetPropertyValidator(string propertyName)
        {
            if (Properties == null)
                return null;

            return Properties.FirstOrDefault(p => String.Equals(propertyName,
                p.PropertyName, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion
    }
}
