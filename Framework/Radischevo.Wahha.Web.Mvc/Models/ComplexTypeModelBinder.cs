using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Validation;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ComplexTypeModelBinder : ModelBinderBase
    {
        #region Constructors
        public ComplexTypeModelBinder()
			: base()
        {
		}
        #endregion

        #region Static Methods
		protected static bool AreMembersValid(BindingContext context, string modelName)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			modelName = modelName ?? context.ModelName;

			return !context.Errors.Where(k => k.Member.StartsWith(modelName,
				StringComparison.InvariantCultureIgnoreCase))
				.Any(k => k.Member.Length != modelName.Length);
		}

		private static bool CanUpdateReadonlyTypedReference(Type type)
        {
            if (type.IsValueType)
                return false;

            if (type.IsArray)
                return false;

            if (type == typeof(string))
                return false;

            return true;
        }

        private static bool AllowPropertyUpdate(PropertyDescriptor property, Predicate<string> filter)
        {
            if (property.IsReadOnly && !CanUpdateReadonlyTypedReference(property.PropertyType))
                return false;

            if (!filter(property.Name))
                return false;

			return !property.Attributes.OfType<SkipBindingAttribute>().Any();
        }
        #endregion

        #region Instance Methods
        protected virtual ICustomTypeDescriptor GetTypeDescriptor(BindingContext context)
        {
            AssociatedMetadataTypeTypeDescriptionProvider provider =
                new AssociatedMetadataTypeTypeDescriptionProvider(context.ModelType);

            return provider.GetTypeDescriptor(context.ModelType, context.Model);
        }

        protected virtual IEnumerable<PropertyDescriptor> GetModelProperties(BindingContext context)
        {
            PropertyDescriptorCollection properties = GetTypeDescriptor(context).GetProperties();
            Predicate<string> propertyFilter = context.AllowMemberUpdate;

            return properties.Cast<PropertyDescriptor>().Where(p => AllowPropertyUpdate(p, propertyFilter));
        }

        protected virtual ModelMetadata GetModelMetadata(BindingContext context)
        {
            return context.Metadata;
        }
        
		private BindingContext CreateComplexModelBindingContext(BindingContext context, object result)
		{
			BindAttribute bind = (BindAttribute)TypeDescriptor.GetAttributes(context.ModelType)[typeof(BindAttribute)];
			Predicate<string> propertyFilter = (bind != null) ? (Predicate<string>)(propertyName =>
				bind.IsUpdateAllowed(propertyName) && context.AllowMemberUpdate(propertyName)) : context.AllowMemberUpdate;

			BindingContext inner = new BindingContext(context, context.ModelType,
				context.ModelName, context.ValueProvider, propertyFilter,
				context.Errors) {
					Model = result
				};
			return inner;
		}

        protected virtual bool OnModelUpdating(BindingContext context)
        {
            return true;
        }

        protected virtual void OnModelUpdated(BindingContext context)
        {
			ModelValidationContext validation = new ModelValidationContext(context, context.Metadata, context.Model);
			foreach (ModelValidationResult result in context.Validator.Validate(validation))
			{
				string memberKey = CreateSubMemberName(context.ModelName, result.Member);
				if(AreMembersValid(context, memberKey))
					context.Errors.Add(memberKey, result.Message);
			}
        }

        protected void BindProperties(BindingContext context)
        {
            foreach (PropertyDescriptor property in GetModelProperties(context))
                BindProperty(context, property);
        }

        protected void BindProperty(BindingContext context, PropertyDescriptor property)
        {
            string propertyKey = CreateSubMemberName(context.ModelName, property.Name);
			// В случае с типом значения нужно задать значение по умолчанию, 
			// иначе частичная инициализация объекта не удастся.
			object value = property.PropertyType.CreateInstance();

			if (context.Contains(propertyKey))
			{
				ModelMetadata propertyMetadata = context.Metadata.GetPropertyMetadata(property.Name);
				ModelValidator propertyValidator = context.Validator.GetPropertyValidator(property.Name);

				BindingContext inner = new BindingContext(context, property.PropertyType,
					propertyKey, context.ValueProvider, null, context.Errors) {
						Model = property.GetValue(context.Model),
						Metadata = propertyMetadata, Validator = propertyValidator
					};

				value = GetPropertyValue(inner, property);
			}

            if (OnPropertyUpdating(context, property, value))
            {
                SetProperty(context, property, value);
                OnPropertyUpdated(context, property, value);
            }
        }

		protected virtual IModelBinder GetPropertyBinder(PropertyDescriptor property)
		{
			ModelBinderAttribute attribute = property.Attributes
				.OfType<ModelBinderAttribute>().FirstOrDefault();

			if (attribute == null)
				return Binders.GetBinder(property.PropertyType);

			return attribute.GetBinder();
		}

        protected virtual object GetPropertyValue(BindingContext context, 
            PropertyDescriptor property)
        {
			IModelBinder binder = GetPropertyBinder(property);
            object value = binder.Bind(context);

            if ((context.Metadata.ConvertEmptyStringToNull) 
                && Object.Equals(value, String.Empty))
                return null;

            return value;
        }

        protected virtual bool OnPropertyUpdating(BindingContext context,
            PropertyDescriptor property, object value)
        {
            string propertyKey = CreateSubMemberName(context.ModelName, property.Name);
            ModelValidator propertyValidator = context.Validator.GetPropertyValidator(property.Name);
			ModelValidationContext validation = new ModelValidationContext(context, 
				context.Metadata, context.Model, value);

            foreach (ModelValidationResult result in propertyValidator.Validate(validation))
                context.Errors.Add(propertyKey, new ValidationError(result.Message, value, null));
            
            return VerifyValueUsability(context, propertyKey, property.PropertyType, value);
        }

        protected void SetProperty(BindingContext context, PropertyDescriptor property, object value)
        {
            string propertyKey = CreateSubMemberName(context.ModelName, property.Name);
            if (property.IsReadOnly)
                return;

            try
            {
                property.SetValue(context.Model, value);
            }
            catch (Exception ex)
            {
                context.Errors.Add(propertyKey, new ValidationError(ex.Message, value, ex));
            }
        }

        protected virtual void OnPropertyUpdated(BindingContext context, 
            PropertyDescriptor property, object value)
        {
        }

		protected override object ExecuteBind(BindingContext context)
		{
			object result = CreateModelInstance(context);
			if (result == null)
				return null;

			BindingContext inner = CreateComplexModelBindingContext(context, result);

			if (OnModelUpdating(inner))
			{
				BindProperties(inner);
				OnModelUpdated(inner);
			}
			return result;
		}
        #endregion
    }
}