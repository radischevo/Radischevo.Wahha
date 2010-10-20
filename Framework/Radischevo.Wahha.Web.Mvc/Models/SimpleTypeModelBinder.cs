using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class SimpleTypeModelBinder : ModelBinderBase
	{
		#region Constructors
		public SimpleTypeModelBinder()
			: base()
		{
		}
		#endregion

		#region Static Methods
		private static T Accumulate<T>(object accumulator, object value, Func<T, T, T> func)
		{
			if (accumulator == null)
				return (T)value;

			return func((T)accumulator, (T)value);
		}

		protected static object AccumulateBitmask(object accumulator, object value, Type type)
		{
			Type valueType = Enum.GetUnderlyingType(type);

			if (valueType.Equals(typeof(int)))
				return Accumulate<int>(accumulator, value, (a, b) => a | b);

			if (valueType.Equals(typeof(sbyte)))
				return Accumulate<sbyte>(accumulator, value, (a, b) => (sbyte)(a | b));

			if (valueType.Equals(typeof(byte)))
				return Accumulate<byte>(accumulator, value, (a, b) => (byte)(a | b));

			if (valueType.Equals(typeof(short)))
				return Accumulate<short>(accumulator, value, (a, b) => (short)(a | b));

			if (valueType.Equals(typeof(ushort)))
				return Accumulate<ushort>(accumulator, value, (a, b) => (ushort)(a | b));

			if (valueType.Equals(typeof(uint)))
				return Accumulate<uint>(accumulator, value, (a, b) => (uint)(a | b));

			if (valueType.Equals(typeof(long)))
				return Accumulate<long>(accumulator, value, (a, b) => (long)(a | b));

			if (valueType.Equals(typeof(ulong)))
				return Accumulate<ulong>(accumulator, value, (a, b) => (ulong)(a | b));

			if (valueType.Equals(typeof(char)))
				return Accumulate<char>(accumulator, value, (a, b) => (char)(a | b));

			return value;
		}
		#endregion

		#region Instance Methods
		private object ExtractValue(BindingContext context)
		{
			object value = null;
			context.TryGetValue(out value);

			return value;
		}

		protected override object ExecuteBind(BindingContext context)
		{
			if (context.ModelType.IsEnum)
				return BindEnum(context);

			return BindValue(context, CultureInfo.CurrentCulture);
		}

		protected override bool TryBindExactValue(BindingContext context, out object value)
		{
			// here we need to override the default behaviour.
			value = null;
			return false;
		}

		protected virtual object BindValue(BindingContext context, CultureInfo culture)
		{
			object value = ExtractValue(context);
			Type type = context.ModelType.MakeNonNullableType();
			CultureInfo currentCulture = culture ?? CultureInfo.CurrentCulture;

			if (type.IsInstanceOfType(value))
				return value;

			string stringValue = (value as string);
			if (String.IsNullOrEmpty(stringValue))
				return null;

			if (type == typeof(Boolean)) // с bool разговор особый
			{
				switch (stringValue.ToLower(currentCulture))
				{
					case "on":
					case "yes":
					case "true":
						return true;
					default:
						return false;
				}
			}

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

		protected object BindEnum(BindingContext context)
		{
			string stringValue = Convert.ToString(ExtractValue(context));
			Type type = context.ModelType.MakeNonNullableType();

			if (String.IsNullOrEmpty(stringValue))
				return null;

			bool canBeBitmask = type.GetCustomAttributes<FlagsAttribute>().Any();
			string[] values = stringValue.Split(new char[] { ',' },
				StringSplitOptions.RemoveEmptyEntries);

			if (canBeBitmask && values.Length > 1)
				return BindBitmask(context, values, type);

			try
			{
				return Enum.Parse(type, values[0], true);
			}
			catch (Exception ex)
			{
				context.Errors.Add(context.ModelName, new ValidationError(ex.Message, stringValue, ex));
				return null;
			}
		}

		protected object BindBitmask(BindingContext context, IEnumerable<string> values, Type type)
		{
			IEnumerable<string> names = values.Intersect(Enum.GetNames(type),
				StringComparer.OrdinalIgnoreCase);
			object value = null;
			object defaultValue = type.CreateInstance();

			foreach (string name in names)
				value = AccumulateBitmask(value, Enum.Parse(type, name, true), type);

			if (value == null)
				return defaultValue;

			return Converter.ChangeType(type, value, defaultValue, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}
