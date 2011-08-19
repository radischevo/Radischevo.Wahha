using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Radischevo.Wahha.Core;

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

        private static bool AllowPropertyUpdate(PropertyDescriptor property)
        {
            if (property.IsReadOnly && !CanUpdateReadonlyTypedReference(property.PropertyType))
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
			return GetTypeDescriptor(context)
				.GetProperties().Cast<PropertyDescriptor>()
				.Where(p => AllowPropertyUpdate(p))
				.OrderBy(p => {
					PropertyBindingOrderAttribute attribute = p.Attributes
						.OfType<PropertyBindingOrderAttribute>()
						.FirstOrDefault();

					return (attribute == null) ? int.MaxValue : attribute.Order;
				});
        }

		private BindingContext CreateComplexModelBindingContext(BindingContext context, object result)
		{
			BindAttribute bind = (BindAttribute)TypeDescriptor.GetAttributes(context.ModelType)[typeof(BindAttribute)];
			BindingContext inner = new BindingContext(context, context.ModelType,
				context.ModelName, context.ValueProvider, context.Errors) {
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
				BindingContext inner = new BindingContext(context, property.PropertyType,
					propertyKey, context.ValueProvider, context.Errors) {
						Model = property.GetValue(context.Model)
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
            return binder.Bind(context);
        }

        protected virtual bool OnPropertyUpdating(BindingContext context,
            PropertyDescriptor property, object value)
        {
            string propertyKey = CreateSubMemberName(context.ModelName, property.Name);
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