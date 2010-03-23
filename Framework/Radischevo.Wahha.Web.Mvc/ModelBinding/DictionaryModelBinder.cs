using System;
using System.Collections.Generic;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class DictionaryModelBinder<TKey, TValue> : SimpleTypeModelBinder
	{
		#region Instance Fields
		private IModelBinder _keyBinder;
		private IModelBinder _valueBinder;
		#endregion

		#region Constructors
		public DictionaryModelBinder()
			: base()
		{
		}
		#endregion

		#region Instance Properties
		public IModelBinder KeyBinder
		{
			get
			{
				if (_keyBinder == null)
					_keyBinder = Binders.GetBinder(typeof(TKey));

				return _keyBinder;
			}
		}

		public IModelBinder ValueBinder
		{
			get
			{
				if (_valueBinder == null)
					_valueBinder = Binders.GetBinder(typeof(TValue));

				return _valueBinder;
			}
		}
		#endregion

		#region Static Methods
		private static ValueDictionary CreateKeyBindingData(string keyName, object keyValue)
		{
			ValueDictionary values = new ValueDictionary();
			values[keyName] = keyValue;

			return values;
		}

		protected static string GetItemKey(string key, string memberName)
		{
			if (String.IsNullOrEmpty(key))
				return null;

			string keyPrefix = String.Concat(memberName, ValueDelimiter);
			// на входе: product-items-1-id, product-items
			// на выходе - 1
			int index = key.IndexOf(keyPrefix, StringComparison.OrdinalIgnoreCase);
			if (index != 0) // ключ должен начинаться с memberName + delimiter
				return null;

			key = key.Substring(keyPrefix.Length); // отрезаем префикс, и...
			index = key.IndexOf(ValueDelimiter);
			if (index > -1)
				key = key.Substring(0, index); // и кусок после ключа

			return key;
		}
		#endregion

		#region Instance Methods
		private TValue ValidateValue(BindingContext context, string elementName, object value)
		{
			if (VerifyValueUsability(context, elementName, typeof(TValue), value))
				return Converter.ChangeType<TValue>(value);

			return default(TValue);
		}

		protected override object ExecuteBind(BindingContext context)
		{
			Type keyType = typeof(TKey);
			Type valueType = typeof(TValue);

			if (!keyType.IsSimple())
				throw Error.UnsupportedDictionaryType(context.ModelType);

			Dictionary<TKey, TValue> convertedValues = new Dictionary<TKey, TValue>();

			foreach (KeyValuePair<string, object> kvp in context.Data)
			{
				string formKey = GetItemKey(kvp.Key, context.ModelName);
				if (String.IsNullOrEmpty(formKey))
					continue;

				string keyField = CreateSubMemberName(context.ModelName, "key");

				BindingContext keyContext = new BindingContext(context, keyType,
					keyField, ParameterSource.Form, CreateKeyBindingData(keyField, formKey),
					null, context.Errors);

				object boundKey = KeyBinder.Bind(keyContext);
				if (boundKey == null)
					continue;

				TKey key = Converter.ChangeType<TKey>(boundKey);
				if (!valueType.IsSimple() && convertedValues.ContainsKey(key))
					continue;

				string formValue = CreateSubMemberName(context.ModelName, formKey);
				BindingContext valueContext = new BindingContext(context, valueType,
					formValue, context.Source, context.Data, null, context.Errors);

				TValue value = ValidateValue(context, formValue, ValueBinder.Bind(valueContext));
				convertedValues.Add(key, value);
			}

			IDictionary<TKey, TValue> dictionary = (CreateModelInstance(context) as IDictionary<TKey, TValue>);
			if (dictionary == null)
				return null;

			if (convertedValues.Count == 0)
				return dictionary;

			CollectionHelpers.CopyDictionary<TKey, TValue>(dictionary, convertedValues);
			return dictionary;
		}
		#endregion
	}
}
