using System;
using System.Collections.Generic;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ArrayModelBinder<T> : ModelBinderBase
	{
		#region Instance Fields
		private IModelBinder _elementBinder;
		#endregion

		#region Constructors
		public ArrayModelBinder()
			: base()
		{
		}
		#endregion

		#region Instance Properties
		public bool IsSimpleType
		{
			get
			{
				return typeof(T).IsSimple();
			}
		}

		public bool IsNullableType
		{
			get
			{
				return typeof(T).IsNullable();
			}
		}

		public IModelBinder ElementBinder
		{
			get
			{
				if (_elementBinder == null)
					_elementBinder = Binders.GetBinder(typeof(T));

				return _elementBinder;
			}
		}
		#endregion

		#region Static Methods
		protected static int? GetItemIndex(string key, string memberName)
		{
			if (String.IsNullOrEmpty(key))
				return null;
			int result;

			string keyPrefix = String.Concat(memberName, ValueDelimiter);
			// на входе: product-items-1-id, product-items
			// на выходе - 1
			int index = key.IndexOf(keyPrefix, StringComparison.OrdinalIgnoreCase);
			if (index != 0) // ключ должен начинаться с memberName + delimiter
				return null;

			key = key.Substring(keyPrefix.Length); // отрезаем префикс, и...
			index = key.IndexOf(ValueDelimiter);
			if (index > -1)
				key = key.Substring(0, index); // и кусок после индекса

			if (int.TryParse(key, out result))
				return result;

			return null;
		}
		#endregion

		#region Instance Methods
		private T ValidateElement(BindingContext context, string elementName, object value)
		{
			if (VerifyValueUsability(context, elementName, typeof(T), value))
				return Converter.ChangeType<T>(value);

			return default(T);
		}

		protected ICollection<T> ReadCollection(BindingContext context)
		{
			Type elementType = typeof(T);

			SortedDictionary<int, T> convertedValues = new SortedDictionary<int, T>();
			ValueProviderResult result = context.GetValue();
			object value = (result == null) ? null : result.GetValue<string>();

			if (value != null && IsSimpleType)
			{
				string[] split = ((string)value).Split(new char[] { ',' },
					StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < split.Length; ++i)
				{
					string elementKey = CreateSubMemberName(context.ModelName, i.ToString());

					ValueDictionary bindingData = new ValueDictionary();
					bindingData.Add(elementKey, split[i]);

					BindingContext inner = new BindingContext(context, elementType,
						elementKey, new DictionaryValueProvider(bindingData), context.ModelState);

					value = ElementBinder.Bind(inner);
					convertedValues[i] = ValidateElement(context, elementKey, value);
				}
			}
			else
			{
				foreach (string kvp in context.ValueProvider.Keys)
				{
					int? index = GetItemIndex(kvp, context.ModelName);
					if (!index.HasValue)
						continue;

					if (convertedValues.ContainsKey(index.Value))
						continue;

					string elementKey = CreateSubMemberName(context.ModelName, index.ToString());
					BindingContext inner = new BindingContext(context, elementType,
						elementKey, context.ValueProvider, context.ModelState);

					value = ElementBinder.Bind(inner);
					convertedValues[index.Value] = ValidateElement(context, elementKey, value);
				}
			}
			return convertedValues.Values;
		}

		protected override object ExecuteBind(BindingContext context)
		{
			ICollection<T> convertedValues = ReadCollection(context);

			T[] array = new T[convertedValues.Count];
			if (convertedValues.Count == 0)
				return array;

			CollectionHelpers.CopyArray<T>(array, convertedValues);
			return array;
		}
		#endregion
	}
}
