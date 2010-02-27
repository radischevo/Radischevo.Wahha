using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Validation;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public delegate DataAnnotationsValidationRule DataAnnotationsValidationRuleFactory(
        ModelValidator validator, ValidationAttribute attribute);

    public class DataAnnotationsModelValidator : ModelValidator
    {
        #region Static Fields
        private static ReaderWriterLock _lock = new ReaderWriterLock();
        private static Dictionary<Type, DataAnnotationsValidationRuleFactory> _adapters;
        private static DataAnnotationsValidationRuleFactory _defaultFactory;
        #endregion

        #region Instance Fields
        private List<ModelValidationRule> _validationRules;
        #endregion

        #region Constructors
        static DataAnnotationsModelValidator()
        {
            _adapters = new Dictionary<Type, DataAnnotationsValidationRuleFactory>();

            _adapters.Add(typeof(RangeAttribute), (validator, attr) => new RangeValidationRule(
                (DataAnnotationsModelValidator)validator, (RangeAttribute)attr));

            _adapters.Add(typeof(RegularExpressionAttribute), (validator, attr) => new RegularExpressionValidationRule(
                (DataAnnotationsModelValidator)validator, (RegularExpressionAttribute)attr));

            _adapters.Add(typeof(RequiredAttribute), (validator, attr) => new RequiredFieldValidationRule(
                (DataAnnotationsModelValidator)validator, (RequiredAttribute)attr));

            _adapters.Add(typeof(StringLengthAttribute), (validator, attr) => new StringLengthValidationRule(
                (DataAnnotationsModelValidator)validator, (StringLengthAttribute)attr));

            _adapters.Add(typeof(RemoteValidationAttribute), (validator, attr) => new RemoteValidationRule(
                (DataAnnotationsModelValidator)validator, (RemoteValidationAttribute)attr));

			_adapters.Add(typeof(DataTypeValidationAttribute), (validator, attr) => new DataTypeValidationRule(
				(DataAnnotationsModelValidator)validator, (DataTypeValidationAttribute)attr));

            _defaultFactory = (validator, attr) => new DataAnnotationsValidationRule(
                (DataAnnotationsModelValidator)validator, attr);
        }

        public DataAnnotationsModelValidator(Type modelType)
            : this(null, null, modelType)
        {
        }

        public DataAnnotationsModelValidator(
            DataAnnotationsModelValidator container, 
            string propertyName, Type modelType)
            : base(container, propertyName, modelType)
        {
            _validationRules = new List<ModelValidationRule>();
            CreateValidationRules(GetTypeDescriptor()
                .GetAttributes().OfType<ValidationAttribute>());
        }
        #endregion

        #region Static Methods
        public static void RegisterAdapter(Type attributeType, Type adapterType)
        {
            ValidateAttributeType(attributeType);
            ValidateAdapterType(adapterType);
            
            ConstructorInfo constructor = GetAdapterConstructor(attributeType, adapterType);

            _lock.AcquireWriterLock(Timeout.Infinite);

            try
            {
                _adapters[attributeType] = (validator, attribute) => (DataAnnotationsValidationRule)constructor
                    .Invoke(new object[] { validator, attribute });
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        public static void RegisterFactory(Type attributeType, DataAnnotationsValidationRuleFactory factory)
        {
            ValidateAttributeType(attributeType);
            ValidateFactory(factory);

            _lock.AcquireWriterLock(Timeout.Infinite);

            try
            {
                _adapters[attributeType] = factory;
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        private static ConstructorInfo GetAdapterConstructor(Type attributeType, Type adapterType)
        {
            Type validatorType = typeof(ModelValidator);

            ConstructorInfo constructor = adapterType.GetConstructor(new Type[] { validatorType, attributeType });
            if (constructor == null)
                throw Error.InvalidDataAnnotationsValidationRuleConstructor(adapterType, validatorType, attributeType);

            return constructor;
        }

        private static void ValidateAdapterType(Type adapterType)
        {
            Precondition.Require(adapterType, () => Error.ArgumentNull("adapterType"));

            if (!typeof(DataAnnotationsValidationRule).IsAssignableFrom(adapterType))
                throw Error.TypeMustDeriveFromType(adapterType, typeof(DataAnnotationsValidationRule));
        }

        private static void ValidateAttributeType(Type attributeType)
        {
            Precondition.Require(attributeType, () => Error.ArgumentNull("attributeType"));

            if (!typeof(ValidationAttribute).IsAssignableFrom(attributeType))
                throw Error.TypeMustDeriveFromType(attributeType, typeof(ValidationAttribute));
        }

        private static void ValidateFactory(DataAnnotationsValidationRuleFactory factory)
        {
            Precondition.Require(factory, () => Error.ArgumentNull("factory"));
        }
        #endregion

        #region Instance Methods
        protected virtual ICustomTypeDescriptor GetTypeDescriptor()
        {
            return new AssociatedMetadataTypeTypeDescriptionProvider(Type).GetTypeDescriptor(Type);
        }

        protected virtual void CreateValidationRules(
            IEnumerable<ValidationAttribute> attributes)
        {
            _lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (ValidationAttribute attribute in attributes)
                {
                    DataAnnotationsValidationRuleFactory factory;
                    if (!_adapters.TryGetValue(attribute.GetType(), out factory))
                        factory = _defaultFactory;

                    _validationRules.Add(factory(this, attribute));
                }
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }

        protected override ModelValidator CreateValidatorForProperty(PropertyDescriptor property)
        {
            DataAnnotationsModelValidator validator = new DataAnnotationsModelValidator(this, 
                property.Name, property.PropertyType);

            validator.CreateValidationRules(property.Attributes.OfType<ValidationAttribute>());
            return validator;
        }

        public override IEnumerable<ModelValidationRule> GetValidationRules()
        {
            return _validationRules;
        }
        #endregion
    }
}
