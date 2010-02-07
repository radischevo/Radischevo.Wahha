using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Validation;

namespace Radischevo.Wahha.Web.Mvc
{
    public class DefaultModelBinder : IModelBinder
    {
        #region Constants
        private const char _valueDelimiter = '-';
        private static readonly Type _collectionType = typeof(ICollection<>);
        private static readonly Type _dictionaryType = typeof(IDictionary<,>);
        private static readonly Type _enumerableType = typeof(IEnumerable<>);
        #endregion

        #region Instance Fields
        private string _resourceClassKey;
        private ModelBinderCollection _binders;
        #endregion

        #region Constructors
        public DefaultModelBinder()
        { }
        #endregion

        #region Instance Properties
        public string ResourceClassKey
        {
            get
            {
                return _resourceClassKey ?? String.Empty;
            }
            set
            {
                _resourceClassKey = value;
            }
        }

        public ModelBinderCollection Binders
        {
            get
            {
                if (_binders == null)
                    _binders = Configuration.Configuration.Instance.Models.Binders;

                return _binders;
            }
        }
        #endregion

        #region Static Methods
        protected static int? GetItemIndex(string key, string memberName)
        {
            if (String.IsNullOrEmpty(key))
                return null;
            int result;

            string keyPrefix = String.Concat(memberName, _valueDelimiter);
            // на входе: product-items-1-id, product-items
            // на выходе - 1
            int index = key.IndexOf(keyPrefix, StringComparison.OrdinalIgnoreCase);
            if (index != 0) // ключ должен начинаться с memberName + delimiter
                return null;

            key = key.Substring(keyPrefix.Length); // отрезаем префикс, и...
            index = key.IndexOf(_valueDelimiter);
            if (index > -1)
                key = key.Substring(0, index); // и кусок после индекса

            if (int.TryParse(key, out result))
                return result;

            return null;
        }

        protected static string GetItemKey(string key, string memberName)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            string keyPrefix = String.Concat(memberName, _valueDelimiter);
            // на входе: product-items-1-id, product-items
            // на выходе - 1
            int index = key.IndexOf(keyPrefix, StringComparison.OrdinalIgnoreCase);
            if (index != 0) // ключ должен начинаться с memberName + delimiter
                return null;

            key = key.Substring(keyPrefix.Length); // отрезаем префикс, и...
            index = key.IndexOf(_valueDelimiter);
            if (index > -1)
                key = key.Substring(0, index); // и кусок после ключа

            return key;
        }

        protected static string CreateSubMemberName(string prefix, string propertyName)
        {
            if (String.IsNullOrEmpty(prefix))
                return propertyName;

            if (String.IsNullOrEmpty(propertyName))
                return prefix;

            return String.Concat(prefix, _valueDelimiter, propertyName);
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

            return true;
        }

		protected static bool AreMembersValid(BindingContext context, string modelName)
		{
			Precondition.Require(context, Error.ArgumentNull("context"));
			modelName = modelName ?? context.ModelName;
			
			return !context.Errors.Where(k => k.Key.StartsWith(modelName,
				StringComparison.InvariantCultureIgnoreCase))
				.Any(k => k.Key.Length != modelName.Length);
		}
        #endregion

        #region Instance Methods
        #region Internal Helper Methods
        private string GetResourceString(ControllerContext context, string resourceName)
        {
            if (!String.IsNullOrEmpty(ResourceClassKey) && 
                context != null && context.Context != null)
                return context.Context.GetGlobalResourceObject(ResourceClassKey, 
                    resourceName, CultureInfo.CurrentUICulture) as string;

            return null;
        }

        private bool VerifyValueUsability(BindingContext context, string elementKey, Type elementType, object value)
        {
            if (value == null && !elementType.IsNullable() && !context.Errors.Any(
				k => k.Key.Equals(elementKey, StringComparison.InvariantCultureIgnoreCase)))
			{
				string message = GetValueRequiredResource(context);
                context.Errors.Add(elementKey, new ValidationError(message, value, null));
             
                return false;
            }
            return true;
        }

        private string GetValueInvalidResource(ControllerContext context)
        {
            return GetResourceString(context, "PropertyValueInvalid") ??
                Resources.Resources.Error_BinderValueInvalid;
        }

        private string GetValueRequiredResource(ControllerContext context)
        {
            return GetResourceString(context, "PropertyValueRequired") ??
                Resources.Resources.Error_BinderValueRequired;
        }

        protected SortedDictionary<int, object> ReadCollection(BindingContext context, Type elementType)
        {
            bool isSimpleType = elementType.IsSimple();
            bool allowsNullValue = elementType.IsNullable();
            SortedDictionary<int, object> convertedValues = new SortedDictionary<int, object>();

            object value = context.Data.GetValue<string>(context.ModelName);
            IModelBinder binder = Binders.GetBinder(elementType);

            if (value != null && isSimpleType) // список простых значений
            {
                string[] split = ((string)value).Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < split.Length; ++i)
                {
                    string elementKey = CreateSubMemberName(context.ModelName, i.ToString());
                    
                    ValueDictionary bindingData = new ValueDictionary();
                    bindingData.Add(elementKey, split[i]);

                    BindingContext inner = new BindingContext(context, elementType,
                        elementKey, context.Source, bindingData, null, context.Errors);

                    value = binder.Bind(inner);

                    convertedValues[i] = (VerifyValueUsability(context,
                        elementKey, elementType, value)) ? value : null;
                }
            }
            else
            {
                foreach (KeyValuePair<string, object> kvp in context.Data)
                // итерируем по коллекции, выискивая нужные нам значения...
                {
                    int? index = GetItemIndex(kvp.Key, context.ModelName);
                    if (!index.HasValue)
                        continue;
                    
                    if (convertedValues.ContainsKey(index.Value))
                        continue;

                    string elementKey = CreateSubMemberName(context.ModelName, index.ToString());
                    BindingContext inner = new BindingContext(context, elementType,
                        elementKey, context.Source, context.Data, null, context.Errors);

                    value = binder.Bind(inner);
                    convertedValues[index.Value] = (VerifyValueUsability(context, 
                        elementKey, elementType, value)) ? value : null;
                }
            }
            return convertedValues;
        }

        protected virtual object CreateModelInstance(BindingContext context, Type modelType)
        {
            object model = context.Model;
            if (model == null)
            {
                if (context.ModelType.GetConstructor(Type.EmptyTypes) == null)
                {
                    context.Errors.Add(context.ModelName,
                        Error.MissingParameterlessConstructor(context.ModelType));
                    return null;
                }
                else
                {
                    model = Activator.CreateInstance(modelType);
                }
            }
            return model;
        }

        protected virtual ICustomTypeDescriptor GetTypeDescriptor(BindingContext context)
        {
            AssociatedMetadataTypeTypeDescriptionProvider provider =
                new AssociatedMetadataTypeTypeDescriptionProvider(context.ModelType);

            return provider.GetTypeDescriptor(context.ModelType, context.Model);
        }

        protected IEnumerable<PropertyDescriptor> GetModelProperties(BindingContext context)
        {
            PropertyDescriptorCollection properties = GetTypeDescriptor(context).GetProperties();
            Predicate<string> propertyFilter = context.AllowMemberUpdate;

            return properties.Cast<PropertyDescriptor>().Where(p => AllowPropertyUpdate(p, propertyFilter));
        }

        protected virtual ModelMetadata GetModelMetadata(BindingContext context)
        {
            return context.Metadata;
        }
        #endregion

        #region Well-Know Type Binding Methods
        public virtual object Bind(BindingContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));

            if (!String.IsNullOrEmpty(context.ModelName) && !context.Data.Any(k => 
                k.Key.StartsWith(context.ModelName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (context.FallbackToEmptyPrefix)
                    context.ModelName = String.Empty;
                else
                    return null;
            }

            object rawValue = null;
            context.TryGetValue(out rawValue);

            if (context.ModelType.IsSimple() || context.ModelType.IsEnum) // простой тип, тут пробуем устроить convert
                return BindSimpleObject(context, context.ModelType, rawValue, CultureInfo.CurrentCulture);

            if (context.ModelType.IsGenericType && // nullable - достаем аргумент и делаем конверт...
                context.ModelType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return BindSimpleObject(context, context.ModelType.GetGenericArguments()[0], 
                    rawValue, CultureInfo.CurrentCulture);

            return BindComplexObject(context);
        }

        protected object BindSimpleObject(BindingContext context,
            Type type, object value, CultureInfo culture)
        {
            if (type.IsInstanceOfType(value))
                return value;

            string valueAsString = (value as string);
            if (String.IsNullOrEmpty(valueAsString))
                return null;

            if (type.IsEnum)
                return BindEnum(context, valueAsString, type);

            if (type == typeof(Boolean)) // с bool разговор особый
            {
                switch (valueAsString.ToLower(CultureInfo.CurrentCulture))
                {
                    case "on":
                    case "yes":
                    case "true":
                        return true;
                    default:
                        return false;
                }
            }

            CultureInfo currentCulture = culture ?? CultureInfo.CurrentCulture;
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            bool canConvertFrom = converter.CanConvertFrom(value.GetType());
            if (!canConvertFrom)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
                if (!converter.CanConvertTo(type))
                    return null;
            }

            try
            {
                return (canConvertFrom) ? converter.ConvertFrom(null, currentCulture, value)
                    : converter.ConvertTo(null, currentCulture, value, type);
            }
            catch (Exception ex)
            {
                context.Errors.Add(context.ModelName, new ValidationError(ex.Message, value, ex));
                return null;
            }
        }

        protected object BindEnum(BindingContext context, string value, Type type)
        {
            try
            {
                return Enum.Parse(type, value, true);
            }
            catch (Exception ex)
            {
                context.Errors.Add(context.ModelName, new ValidationError(ex.Message, value, ex));
                return null;
            }
        }

        protected virtual object BindComplexObject(BindingContext context)
        {
            if (context.ModelType.IsArray)
                return BindArray(context, context.ModelType.GetElementType());

            Type dictionaryType = context.ModelType.GetGenericInterface(_dictionaryType);
            if (dictionaryType != null)
                return BindDictionary(context, dictionaryType);

            Type enumerableType = context.ModelType.GetGenericInterface(_enumerableType);
            if (enumerableType != null)
            {
                Type collectionType = context.ModelType.GetGenericInterface(_collectionType);
                if (collectionType != null)
                    return BindCollection(context, collectionType);
                
                return BindArray(context, enumerableType.GetGenericArguments()[0]);
            }
            return BindComplexElementalObject(context);
        }

        protected object BindArray(BindingContext context, Type elementType)
        {
            SortedDictionary<int, object> convertedValues =
                ReadCollection(context, elementType);

            Array array = Array.CreateInstance(elementType, convertedValues.Count);
            // если ничего не было найдено, на этом и закончим
            if (convertedValues.Count == 0)
                return array;

            CollectionHelpers.CopyArray(elementType, array, 
                convertedValues.Values);
            return array;
        }

        protected object BindDictionary(BindingContext context, Type dictionaryType)
        {
            Type[] ga = dictionaryType.GetGenericArguments();
            Type keyType = ga[0];
            Type valueType = ga[1];

            if (!keyType.IsSimple()) // мы можем конвертировать только простые типы ключа
                throw Error.UnsupportedDictionaryType(context.ModelType);

            Dictionary<object, object> convertedValues = new Dictionary<object, object>();
            IModelBinder binder = Binders.GetBinder(valueType);

            foreach (KeyValuePair<string, object> kvp in context.Data)
            {
                string formKey = GetItemKey(kvp.Key, context.ModelName);
                if (String.IsNullOrEmpty(formKey))
                    continue;

                object key = BindSimpleObject(context, keyType, formKey, CultureInfo.CurrentCulture);
                if (key == null || (!valueType.IsSimple() && convertedValues.ContainsKey(key)))
                    continue;

                string formValue = CreateSubMemberName(context.ModelName, formKey);
                BindingContext inner = new BindingContext(context, valueType,
                    formValue, context.Source,
                    context.Data, null, context.Errors);

                object value = binder.Bind(inner);
                convertedValues.Add(key, (VerifyValueUsability(context, 
                    formValue, valueType, value)) ? value : null);
            }

            object dictionary = CreateModelInstance(context, context.ModelType);

            if (dictionary == null)
                return null;

            if (convertedValues.Count == 0)
                return dictionary;

            CollectionHelpers.CopyDictionary(keyType, valueType, dictionary, convertedValues);
            return dictionary;
        }

        protected object BindCollection(BindingContext context, Type collectionType)
        {
            Type itemType = collectionType.GetGenericArguments()[0];
            SortedDictionary<int, object> convertedValues =
                ReadCollection(context, itemType);

            object collection = CreateModelInstance(context, context.ModelType);

            if (collection == null)
                return null;

            // если ничего не было найдено, на этом и закончим
            if (convertedValues.Count == 0)
                return collection;

            CollectionHelpers.CopyCollection(itemType, 
                collection, convertedValues.Values);
            return collection;
        }
        #endregion

        #region Custom Type Binding
        protected virtual object BindComplexElementalObject(BindingContext context)
        {
            object result = CreateModelInstance(context, context.ModelType);

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

		private BindingContext CreateComplexModelBindingContext(BindingContext context, object result)
		{
			BindAttribute bind = (BindAttribute)TypeDescriptor.GetAttributes(context.ModelType)[typeof(BindAttribute)];
			Predicate<string> propertyFilter = (bind != null) ? (Predicate<string>)(propertyName =>
				bind.IsUpdateAllowed(propertyName) && context.AllowMemberUpdate(propertyName)) : context.AllowMemberUpdate;

			BindingContext inner = new BindingContext(context, context.ModelType,
				context.ModelName, context.Source, context.Data, propertyFilter,
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

        protected virtual void BindProperty(BindingContext context, PropertyDescriptor property)
        {
            string propertyKey = CreateSubMemberName(context.ModelName, property.Name);
			object value = null;

			if (context.Data.Any(k => k.Key.StartsWith(propertyKey,
				StringComparison.InvariantCultureIgnoreCase)))
			{
				IModelBinder binder = Binders.GetBinder(property.PropertyType);
				ModelMetadata propertyMetadata = context.Metadata.GetPropertyMetadata(property.Name);
				ModelValidator propertyValidator = context.Validator.GetPropertyValidator(property.Name);

				BindingContext inner = new BindingContext(context, property.PropertyType,
					propertyKey, context.Source, context.Data, null, context.Errors) {
						Model = property.GetValue(context.Model),
						Metadata = propertyMetadata, Validator = propertyValidator
					};

				value = GetPropertyValue(inner, property, binder);
			}
            if (OnPropertyUpdating(context, property, value))
            {
                SetProperty(context, property, value);
                OnPropertyUpdated(context, property, value);
            }
        }

        protected virtual object GetPropertyValue(BindingContext context, 
            PropertyDescriptor property, IModelBinder binder)
        {
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

            bool isUsable = true;
            foreach (ModelValidationResult result in propertyValidator.Validate(validation))
            {
                context.Errors.Add(propertyKey, new ValidationError(result.Message, value, null));
                isUsable = false;
            }
            return isUsable && VerifyValueUsability(context, propertyKey, property.PropertyType, value);
        }

        protected virtual void SetProperty(BindingContext context, PropertyDescriptor property, object value)
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
        #endregion
        #endregion
    }
}
