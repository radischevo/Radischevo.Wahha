using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
	public class DataTypeValidationRule :
		DataAnnotationsValidationRule<DataTypeValidationAttribute>
	{
		#region Static Fields
		private static readonly HashSet<Type> _unsignedNumerics =
			new HashSet<Type>(new Type[] {
				typeof(byte), typeof(ushort), typeof(uint), typeof(ulong)
			});

		private static readonly HashSet<Type> _signedNumerics =
			new HashSet<Type>(new Type[] {
				typeof(sbyte), typeof(short), typeof(int), typeof(long)
			});

		private static readonly HashSet<Type> _floatingPoints =
			new HashSet<Type>(new Type[] {
				typeof(float), typeof(double), typeof(decimal)
			});
		#endregion

		#region Constructors
		public DataTypeValidationRule(
			DataAnnotationsModelValidator validator,
			DataTypeValidationAttribute attribute)
			: base(validator, attribute)
		{
			Type modelType = attribute.Type ?? validator.Type;
			ErrorMessage = attribute.ErrorMessage;

			ApplyValidationRule(modelType);
		}
		#endregion

		#region Static Methods
		protected static bool IsUnsignedNumericType(Type type)
		{
			return _unsignedNumerics.Contains(type.MakeNonNullableType());
		}

		protected static bool IsSignedNumericType(Type type)
		{
			return _signedNumerics.Contains(type.MakeNonNullableType());
		}

		protected static bool IsFloatingPointType(Type type)
		{
			return _floatingPoints.Contains(type.MakeNonNullableType());
		}

		protected static bool IsDateTimeType(Type type)
		{
			return (type == typeof(DateTime) ||
				type == typeof(DateTimeOffset));
		}
		#endregion

		#region Instance Methods
		protected virtual void ApplyValidationRule(Type type)
		{
			if (IsUnsignedNumericType(type))
			{
				ValidationType = "digits";
			}
			else if (IsSignedNumericType(type) || IsFloatingPointType(type))
			{
				ValidationType = "number";
			}
			else if (IsDateTimeType(type))
			{
				ValidationType = Resources.Resources.String_DateTimeDataTypeRule;
			}
		}
		#endregion
	}
}
