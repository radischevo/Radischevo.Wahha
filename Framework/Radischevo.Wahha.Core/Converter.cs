using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Radischevo.Wahha.Core
{
    /// <summary>
    /// Converts a base data type to another base data type.
    /// </summary>
    public static class Converter
    {
        #region Nested Types
        private static class ObjectToHashConverter
        {
            #region Static Fields
            private static readonly Dictionary<Type,
                Func<object, IDictionary<string, object>>> _cache =
                new Dictionary<Type, Func<object, IDictionary<string, object>>>();
            private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
            #endregion

            #region Static Methods
            internal static Func<object, IDictionary<string, object>> GetConverter(Type type)
            {
                _lock.EnterUpgradeableReadLock();

                try
                {
                    Func<object, IDictionary<string, object>> func;
                    if (!_cache.TryGetValue(type, out func))
                    {
                        _lock.EnterWriteLock();
                        try
                        {
                            if (!_cache.TryGetValue(type, out func))
                            {
                                func = CreateConverter(type);
                                _cache[type] = func;
                            }
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                    return func;
                }
                finally
                {
                    _lock.ExitUpgradeableReadLock();
                }
            }

            private static Func<object, IDictionary<string, object>> CreateConverter(Type type)
            {
                Type dictType = typeof(Dictionary<string, object>);

                // setup dynamic method
                // Important: make itemType owner of the method to allow access to internal types
                DynamicMethod dm = new DynamicMethod(string.Empty,
                    typeof(IDictionary<string, object>), new[] { typeof(object) }, type);
                ILGenerator il = dm.GetILGenerator();

                // Dictionary.Add(object key, object value)
                MethodInfo addMethod = dictType.GetMethod("Add");
                // create the Dictionary and store it in a local variable
                il.DeclareLocal(dictType);
                il.Emit(OpCodes.Newobj, dictType.GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_0);

                BindingFlags attributes = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
                foreach (PropertyInfo property in type.GetProperties(attributes).Where(info => info.CanRead))
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldstr, property.Name);
                    il.Emit(OpCodes.Ldarg_0);
                    il.EmitCall(OpCodes.Callvirt, property.GetGetMethod(), null);

                    if (property.PropertyType.IsValueType)
                        il.Emit(OpCodes.Box, property.PropertyType);

                    il.EmitCall(OpCodes.Callvirt, addMethod, null);
                }
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                return (Func<object, IDictionary<string, object>>)dm.CreateDelegate(
                    typeof(Func<object, IDictionary<string, object>>));
            }
            #endregion
        }

		private interface IBoxedConverter
		{
			bool CanConvert
			{
				get;
			}

			object Convert(object value);
		}

		private class BoxedConverter<F, T> : IBoxedConverter
		{
			#region Nested Types
			private delegate T CastMethod(F value);
			#endregion

			#region Static Fields
			private static readonly CastMethod _castMethod;
			#endregion

			#region Constructors
			static BoxedConverter()
			{
				MethodInfo method = FindCastOperator(typeof(T)) ?? FindCastOperator(typeof(F));
				_castMethod = (method == null) ? null : (CastMethod)Delegate.CreateDelegate(typeof(CastMethod), method);
			}
			#endregion

			#region Static Methods
			private static MethodInfo FindCastOperator(Type type)
			{
				foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
				{
					if (method.IsSpecialName && method.ReturnType == typeof(T) &&
						(method.Name == "op_Implicit" || method.Name == "op_Explicit"))
					{
						ParameterInfo[] parameters = method.GetParameters();
						if (parameters.Length == 1 && parameters[0].ParameterType == typeof(F))
							return method;
					}
				}
				return null;
			}
			#endregion

			#region Instance Properties
			public bool CanConvert
			{
				get
				{
					return (_castMethod != null);
				}
			}
			#endregion

			#region Instance Methods
			public object Convert(object value)
			{
				if (CanConvert)
					return _castMethod((F)value);

				throw Error.CouldNotConvertType(typeof(T), "value");
			}
			#endregion
		}
        #endregion

        #region Base16 Conversion
        /// <summary>
        /// Converts a subset of an array of 8-bit unsigned integers to its equivalent <see cref="String" /> 
        /// representation encoded with hex digits. 
        /// </summary>
        /// <param name="array">An array of 8-bit unsigned integers</param>
        public static string ToBase16String(byte[] array)
        {
            return ToBase16String(array, 0, array.Length, false);
        }

        /// <summary>
        /// Converts a subset of an array of 8-bit unsigned integers to its equivalent <see cref="String" /> 
        /// representation encoded with hex digits. 
        /// </summary>
        /// <param name="array">An array of 8-bit unsigned integers</param>
        /// <param name="lowerCase">If set to true, produces a lowercase string.</param>
        public static string ToBase16String(byte[] array, bool lowerCase)
        {
            Precondition.Require(array, () => Error.ArgumentNull("array"));
            return ToBase16String(array, 0, array.Length, lowerCase);
        }

        /// <summary>
        /// Converts a subset of an array of 8-bit unsigned integers to its equivalent <see cref="String" /> 
        /// representation encoded with hex digits. Parameters specify the subset as an offset in the input array and  
        /// the number of elements in the array to convert.
        /// </summary>
        /// <param name="array">An array of 8-bit unsigned integers</param>
        /// <param name="startIndex">An offset in the <paramref name="array"/></param>
        /// <param name="length">The number of elements of <paramref name="array"/> to convert.</param>
        public static string ToBase16String(byte[] array, int startIndex, int length)
        {
            return ToBase16String(array, startIndex, length, false);
        }

        /// <summary>
        /// Converts a subset of an array of 8-bit unsigned integers to its equivalent <see cref="String" /> 
        /// representation encoded with hex digits. Parameters specify the subset as an offset in the input array and  
        /// the number of elements in the array to convert.
        /// </summary>
        /// <param name="array">An array of 8-bit unsigned integers</param>
        /// <param name="startIndex">An offset in the <paramref name="array"/></param>
        /// <param name="length">The number of elements of <paramref name="array"/> to convert.</param>
        /// <param name="lowerCase">If set to true, produces a lowercase string.</param>
        public static string ToBase16String(byte[] array, int startIndex, 
            int length, bool lowerCase)
        {
            Precondition.Require(array, () => Error.ArgumentNull("array"));
            Precondition.Require(length > -1,
				() => Error.ParameterMustBeGreaterThanOrEqual("length", 0, length));
            Precondition.Require(startIndex > -1,
				() => Error.ParameterMustBeGreaterThanOrEqual("startIndex", 0, startIndex));
            Precondition.Require(startIndex <= (array.Length - length),
				() => Error.OffsetMustBeLessThanArrayLength("startIndex", startIndex));

            char[] chars = new char[length * 2];
            int high, low;
            for (int i = startIndex; i < length; ++i)
            {
                int j = i << 1;
                high = array[i] >> 4;
                high += (high < 0x0A) ? 0x30 : (lowerCase) ? 0x57 : 0x37;
                chars[j] = Convert.ToChar(high);

                low = array[i] & 0x0F;
                low += (low < 0x0A) ? 0x30 : (lowerCase) ? 0x57 : 0x37;
                chars[j + 1] = Convert.ToChar(low);
            }
            return new String(chars);
        }

        /// <summary>
        /// Converts the specified <see cref="String"/>, which encodes 
        /// binary data as hex digits, to an equivalent 8-bit 
        /// unsigned integer array.
        /// </summary>
        /// <param name="str">A <see cref="String"/></param>
        public static byte[] FromBase16String(string str)
        {
			Precondition.Require(str, () => Error.ArgumentNull("str"));
			Precondition.Require(str.Length % 2 == 0, 
				() => Error.EncodedStringMustLengthBeOdd("str"));

            int byteCount = str.Length / 2;
            byte[] bytes = new byte[byteCount];
            int high, low;

            for (int i = 0; i < str.Length; i += 2)
            {
                high = str[i];
                low = str[i + 1];

                high -= (high < 0x3A) ? 0x30 : (high > 0x56) ? 0x57 : 0x37;
                low -= (low < 0x3A) ? 0x30 : (low > 0x56) ? 0x57 : 0x37;

                bytes[i / 2] = Convert.ToByte(low + (high << 4));
            }
            return bytes;
        }
		#endregion

		#region Dictionary Conversion
		/// <summary>
		/// Copies all readable public property values from the 
		/// source object to the dictionary.
		/// </summary>
		/// <param name="argument">An object to convert</param>
		public static IDictionary<string, object> ToDictionary(object argument)
		{
			if (argument == null)
				return null;

			if (argument is IDictionary<string, object>)
				return (IDictionary<string, object>)argument;

			Func<object, IDictionary<string, object>> converter =
				ObjectToHashConverter.GetConverter(argument.GetType());

			if (converter == null)
				throw Error.CouldNotCreateDynamicTypeConverter(argument.GetType());

			return converter(argument);
		}
		#endregion

		#region Helper Methods
		private static bool IsExpectedException(Exception error)
		{
			return (
					error is InvalidCastException ||
					error is FormatException ||
					error is ArgumentException ||
					error is OverflowException
				);
		}

		private static bool ValidateValue(object value, Type requiredType)
		{
			if (requiredType.IsInstanceOfType(value))
				return true;

			if (Object.ReferenceEquals(null, value))
				return requiredType.IsNullable();

			return requiredType.IsAssignableFrom(value.GetType());
		}

		private static IBoxedConverter CreateCastOperator(Type from, Type to)
		{
			Type converterType = typeof(BoxedConverter<,>).MakeGenericType(from, to);
			return (IBoxedConverter)Activator.CreateInstance(converterType);
		}
		#endregion

		#region Conversion Methods
		private static object ConvertUndefinedValue(Type type)
		{
			if (type.IsNullable())
				return null;

			throw Error.TargetTypeIsNotNullable(type, "value");
		}

		private static object ConvertUndefinedValue(Type type, object defaultValue)
		{
			if (type.IsNullable())
				return null;

			return defaultValue;
		}

		private static object ConvertValue(Type type,
			object value, IFormatProvider provider)
		{
			Type from = value.GetType();
			Type to = type.MakeNonNullableType();

			if (from == to || to.IsAssignableFrom(from))
				return value;

			IBoxedConverter cast = CreateCastOperator(from, to);
			if (cast.CanConvert)
				return cast.Convert(value);

			if (to == typeof(bool))
				return ConvertToBoolean(from, value, provider);

			if (to.IsEnum)
				return ConvertToEnum(from, to, value, provider);

			if (to == typeof(Guid))
				return ConvertToGuid(value);

			if (to.GetInterface(typeof(IConvertible).Name) != null)
				return ConvertByDefault(to, value, provider);

			throw Error.CouldNotConvertType(to, "value");
		}

		private static object ConvertValue(Type type,
			object value, object defaultValue, IFormatProvider provider)
		{
			try
			{
				return ConvertValue(type, value, provider);
			}
			catch (Exception error)
			{
				if (IsExpectedException(error))
					return defaultValue;

				throw;
			}
		}

		private static bool ConvertToBoolean(Type from,
			object value, IFormatProvider provider)
		{
			if (from == typeof(string))
			{
				switch (((string)value).ToUpperInvariant())
				{
					case "1":
					case "TRUE":
					case "ON":
					case "YES":
					case "Y":
						return true;
				}
				return false;
			}
			if (from == typeof(char))
			{
				switch (Char.ToUpperInvariant((char)value))
				{
					case '1':
					case 'Y':
						return true;
				}
				return false;
			}
			return Convert.ToBoolean(value);
		}

		private static object ConvertToEnum(Type from, Type to,
			object value, IFormatProvider provider)
		{
			if (from == typeof(string)) // special case string type
				return Enum.Parse(to, (string)value, true);

			Type valueType = Enum.GetUnderlyingType(to);
			object enumValue = value;

			if (!valueType.IsAssignableFrom(from))
				enumValue = ConvertValue(valueType, value, provider);

			return Enum.ToObject(to, enumValue);
		}

		private static Guid ConvertToGuid(object value)
		{
			byte[] byteValue = (value as byte[]);
			string stringValue = (value as string);

			if (stringValue != null)
				return new Guid(stringValue);

			else if (byteValue != null && byteValue.Length == 16)
				return new Guid(byteValue);

			throw Error.CouldNotConvertType(typeof(Guid), "value");
		}

		private static object ConvertByDefault(Type to,
			object value, IFormatProvider provider)
		{
			return Convert.ChangeType(value, to, provider);
		}
		#endregion

		#region Generic Change Type Methods
		/// <summary>
        /// Converts the <paramref name="value"/> to the 
        /// <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">A type to convert to.</typeparam>
        /// <param name="value">A value to convert.</param>
        public static T ChangeType<T>(object value)
        {
            return ChangeType<T>(value, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Converts the <paramref name="value"/> to the 
        /// <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">A type to convert to.</typeparam>
        /// <param name="value">A value to convert.</param>
        /// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
        /// supplies culture-specific formatting information.</param>
        public static T ChangeType<T>(object value, IFormatProvider provider)
        {
			return (T)ChangeType(typeof(T), value, provider);
        }

        /// <summary>
        /// Converts the <paramref name="value"/> to the 
        /// <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">A type to convert to.</typeparam>
        /// <param name="value">A value to convert.</param>
        /// <param name="defaultValue">The default value of the parameter.</param>
        public static T ChangeType<T>(object value, T defaultValue)
        {
            return ChangeType<T>(value, defaultValue, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Converts the <paramref name="value"/> to the 
        /// <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">A type to convert to.</typeparam>
        /// <param name="value">A value to convert.</param>
        /// <param name="defaultValue">The default value of the parameter</param>
        /// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
        /// supplies culture-specific formatting information.</param>
        public static T ChangeType<T>(object value, T defaultValue, IFormatProvider provider)
        {
			return (T)ChangeType(typeof(T), value, defaultValue, provider);
        }
        #endregion

		#region Non-generic Change Type Methods
		/// <summary>
		/// Converts the <paramref name="value"/> to the 
		/// specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type">A type to convert to.</param>
		/// <param name="value">A value to convert.</param>
		public static object ChangeType(Type type, object value)
		{
			return ChangeType(type, value, Thread.CurrentThread.CurrentCulture);
		}

		/// <summary>
		/// Converts the <paramref name="value"/> to the 
		/// specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type">A type to convert to.</param>
		/// <param name="value">A value to convert.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
		public static object ChangeType(Type type, object value, IFormatProvider provider)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			Precondition.Require(provider, () => Error.ArgumentNull("provider"));

			if (Object.ReferenceEquals(value, null))
				return ConvertUndefinedValue(type);

			return ConvertValue(type, value, provider);
		}

		/// <summary>
		/// Converts the <paramref name="value"/> to the specified 
		/// <paramref name="type"/>.
		/// </summary>
		/// <param name="type">A type to convert to.</param>
		/// <param name="value">A value to convert.</param>
		/// <param name="defaultValue">The default value of the parameter.</param>
		public static object ChangeType(Type type, object value, object defaultValue)
		{
			return ChangeType(type, value, defaultValue, Thread.CurrentThread.CurrentCulture);
		}

		/// <summary>
		/// Converts the <paramref name="value"/> to the specified 
		/// <paramref name="type"/>.
		/// </summary>
		/// <param name="type">A type to convert to.</param>
		/// <param name="value">A value to convert.</param>
		/// <param name="defaultValue">The default value of the parameter.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
		public static object ChangeType(Type type, object value, 
			object defaultValue, IFormatProvider provider)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			Precondition.Require(provider, () => Error.ArgumentNull("provider"));
			Precondition.Require(ValidateValue(defaultValue, type),
				() => Error.InvalidArgumentType(type, "defaultValue"));

			if (Object.ReferenceEquals(value, null))
				return ConvertUndefinedValue(type);

			return ConvertValue(type, value, defaultValue, provider);
		}
		#endregion
	}
}