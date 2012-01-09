using System;
using System.Configuration;
using System.Reflection;

using Res = Radischevo.Wahha.Core.Resources.Resources;

namespace Radischevo.Wahha.Core
{
    internal static class Error
    {
        internal static Exception ArgumentNull(string name)
        {
            return new ArgumentNullException(name);
        }

		internal static Exception ArgumentOutOfRange(string name, object actualValue)
		{
			return new ArgumentOutOfRangeException(name); // TODO: написать нормальное сообщение
		}

        internal static Exception ParameterMustBeGreaterThan(string parameterName, object lowerBound, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue,
                String.Format(Res.Error_ParameterMustBeGreaterThan, parameterName, lowerBound));
        }

        internal static Exception ParameterMustBeGreaterThanOrEqual(string parameterName, object lowerBound, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue,
                String.Format(Res.Error_ParameterMustBeGreaterThanOrEqual, parameterName, lowerBound));
        }

        internal static Exception OffsetMustBeLessThanArrayLength(string parameterName, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue, 
                Res.Error_OffsetMustBeLessThanArrayLength);
        }

        internal static Exception PropertyIsReadOnly(PropertyInfo propertyInfo)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_PropertyIsReadOnly,
                propertyInfo.Name, propertyInfo.ReflectedType));
        }

        internal static Exception PropertyIsWriteOnly(PropertyInfo pi)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_PropertyIsWriteOnly,
                pi.Name, pi.ReflectedType.Name));
        }

        internal static Exception CouldNotCreateAccessorForIndexedProperty(PropertyInfo pi)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CouldNotCreateAccessorForIndexer,
                pi.Name, pi.ReflectedType.Name));
        }

        internal static Exception TargetTypeIsNotNullable(Type type, string paramName)
        {
            return new InvalidCastException(String.Format(
                Res.Error_TargetTypeIsNotNullable, 
                paramName, type.FullName));
        }

        internal static Exception CouldNotConvertType(Type type, string paramName)
        {
            return new InvalidCastException(String.Format(
                Res.Error_CouldNotConvertType, 
                paramName, type.FullName));
        }

        internal static Exception MethodCouldNotBeGenericDefinition(MethodBase method)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_MethodCouldNotBeGenericDefinition, 
                method.Name, method.ReflectedType.FullName));
        }

        internal static Exception BindingExpressionCannotBeEmpty(string paramName)
        {
            return new ArgumentException(
                Res.Error_MemberBindingExpressionIsEmpty, paramName);
        }

        internal static Exception InvalidBindingExpressionFormat(string paramName)
        {
            return new ArgumentException(
                Res.Error_InvalidBindingExpressionFormat, paramName);
        }

        internal static Exception InvalidIndexerExpressionFormat(string paramName)
        {
            return new ArgumentException(
                Res.Error_InvalidIndexerExpressionFormat, paramName);
        }

        internal static Exception IndexerNotFound(Type type, string paramName)
        {
            return new MissingMemberException(String.Format(
                Res.Error_MissingIndexer,
                type.FullName, paramName));
        }

        internal static Exception MissingMember(Type type, string name)
        {
            return new MissingMemberException(
                String.Format(Res.Error_MissingMember,
                type.FullName, name));
        }

        internal static Exception EncodedStringMustLengthBeOdd(string paramName)
        {
            return new ArgumentException(
                Res.Error_EncodedStringLengthMustBeOdd, paramName);
        }

        internal static Exception CouldNotCreateDynamicTypeConverter(Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CouldNotCreateDynamicConverter,
                type.FullName));
        }

        internal static Exception LinkSourceIsNotInitialized()
        {
            return new InvalidOperationException(
                Res.Error_LinkSourceIsNotInitialized);
        }

        internal static Exception CouldNotCreateTryGetValueDelegateForType(Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CouldNotCreateTryGetValueDelegateForType, 
                type.FullName));
        }

		internal static Exception OperationTimeout(TimeSpan timeout)
		{
			return new TimeoutException(String.Format(
				Res.Error_OperationTimeout, timeout));
		}

		internal static Exception UnableToLoadConfiguration(Exception inner)
		{
			return new InvalidOperationException(
				Res.Error_UnableToLoadConfiguration, inner);
		}

		internal static Exception IncompatibleServiceLocatorType(Type type)
		{
			return new ArgumentException(String.Format(
				Res.Error_IncompatibleServiceLocatorType, type.Name));
		}

		internal static Exception InvalidArgumentType(Type type, string parameterName)
		{
			return new ArgumentException(String.Format(
				Res.Error_InvalidArgumentType, type.Name, parameterName));
		}

		internal static Exception TypeIsNotTuple(object argument, string parameterName)
		{
			return new ArgumentException(String.Format(Res.Error_TypeIsNotTuple, 
				argument.GetType().Name), "other");
		}

		internal static Exception InvalidFormatExpression(string expression)
		{
			return new ArgumentException(String.Format(
				Res.Error_InvalidFormatExpression, expression));
		}

		internal static Exception CouldNotFindAppropriateConstructor (Type type)
		{
			return new MissingMethodException(String.Format(
				Res.Error_CouldNotConstructObject, type.FullName));
		}
		
		internal static Exception DictionaryIsReadOnly () 
		{
			return new NotSupportedException(Res.Error_DictionaryIsReadOnly);
		}
	}
}
