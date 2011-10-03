using System;
using System.Configuration;
using System.Data;

using Res = Radischevo.Wahha.Data.Resources.Resources;

namespace Radischevo.Wahha.Data
{
    internal static class Error
    {
        internal static Exception ArgumentNull(string argumentName)
        {
            return new ArgumentNullException(argumentName);
        }

        internal static Exception ColumnOrdinalDoesNotExistInResultSet(int ordinal)
        {
            return new ArgumentOutOfRangeException("ordinal", ordinal,
                String.Format(Res.Error_ColumnOrdinalDoesNotExistInResultSet, ordinal));
        }

        internal static Exception ColumnNameDoesNotExistInResultSet(string name)
        {
            return new ArgumentOutOfRangeException("name", name,
                String.Format(Res.Error_ColumnNameDoesNotExistInResultSet, name));
        }

		internal static Exception ParameterMustBeGreaterThan(string parameterName, object lowerBound, object actualValue)
		{
			return new ArgumentOutOfRangeException(parameterName, actualValue,
				String.Format(Resources.Resources.Error_ParameterMustBeGreaterThan, parameterName, lowerBound));
		}

        internal static Exception ParameterMustBeGreaterThanOrEqual(string parameterName, object lowerBound, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue,
                String.Format(Res.Error_ParameterMustBeGreaterThanOrEqual, parameterName, lowerBound));
        }

        internal static Exception ConnectionStringNotInitialized()
        {
            return new ArgumentException(Res.Error_ConnectionStringNotInitialized);
        }

        internal static Exception ConnectionStringNotConfigured()
        {
            return new ConfigurationErrorsException(Res.Error_ConnectionStringNotConfigured);
        }

        internal static Exception UnableToLoadConfiguration(Exception inner)
        {
            return new ConfigurationErrorsException(Res.Error_UnableToLoadConfiguration, inner);
        }

        internal static Exception AmbiguousColumnName(string name)
        {
            return new InvalidOperationException(
                String.Format(Res.Error_AmbiguousColumnName, name));
        }

        internal static Exception ReaderIsEmpty()
        {
            return new InvalidOperationException(Res.Error_ReaderIsEmpty);
        }

        internal static Exception CannotEnumerateMoreThanOnce()
        {
            return new InvalidOperationException(Res.Error_CannotEnumerateMoreThanOnce);
        }

        internal static Exception UnsupportedCommandType(Type actualType)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_UnsupportedCommandType, actualType.FullName));
        }

        internal static Exception CommandIsNotInitialized()
        {
            return new InvalidOperationException(Res.Error_CommandIsNotInitialized);
        }

        internal static Exception IncompatibleCacheProviderType(Type type)
        {
            return new InvalidOperationException(
                String.Format(Res.Error_IncompatibleCacheProviderType, type.FullName));
        }

        internal static Exception CachingProviderDoesNotSupportTags(Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CacheProviderDoesNotSupportTags, type.FullName));
        }

        internal static Exception IncompatibleDataProviderFactoryType(Type type)
        {
            return new InvalidOperationException(
                String.Format(Res.Error_IncompatibleProviderFactoryType, type.FullName));
        }

        internal static Exception ObjectDisposed(string objectName)
        {
            return new ObjectDisposedException(objectName,
                String.Format(Res.Error_ObjectDisposed, objectName));
        }

		internal static Exception SelectorMustBeAMethodCall(string parameter)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_SelectorMustBeAMethodCall, parameter));
		}

		internal static Exception MethodCallMustTargetLambdaArgument(string parameter)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_MethodCallMustTargetLambdaArgument, parameter));
		}

		internal static Exception IncompatibleMaterializerType(Type type)
		{
			return new InvalidOperationException(
				String.Format(Res.Error_IncompatibleMaterializerType, type.FullName));
		}

		internal static Exception CouldNotMaterializeCollectionLink(string parameter)
		{
			return new ArgumentException(Res.Error_CouldNotMaterializeCollectionLink, parameter);
		}

		internal static Exception InvalidMethodReturnType(string parameterName, 
			Type actualType, Type requiredType)
		{
			return new ArgumentException(String.Format(Res.Error_InvalidMethodReturnType, 
				actualType.FullName, requiredType.FullName), parameterName);
		}

		internal static Exception InvalidConstructorExpressionType(string parameterName, 
			Type actualType, Type requiredType)
		{
			return new ArgumentException(String.Format(Res.Error_InvalidConstructorExpressionType,
				actualType.FullName, requiredType.FullName), parameterName);
		}

		internal static Exception ExpressionMustBeAConstructorCall(string parameterName)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_ExpressionMustBeAConstructorCall, parameterName));
		}

		internal static Exception CouldNotSetIsolationLevelAfterInitialize()
		{
			return new InvalidOperationException(
				Res.Error_CouldNotSetIsolationLevelAfterInitialize);
		}

		internal static Exception ArrayTypeIsNotSupported(Type type)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_ArrayTypeIsNotSupported, type.FullName));
		}

		internal static Exception CircularReferenceFoundAtType(Type type)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_CircularReferenceFoundAtType, type.FullName));
		}

		internal static Exception CouldNotConvertObjectToType(Type type)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_CouldNotConvertObjectToType, type.FullName));
		}

		internal static Exception CommaWasExpectedInArrayDeclaration()
		{
			return new ArgumentException(Resources.Resources.Error_CommaWasExpectedInArrayDeclaration);
		}

		internal static Exception ExtraCommaAtArrayEndingFound()
		{
			return new ArgumentException(Resources.Resources.Error_ExtraCommaAtArrayEndingFound);
		}

		internal static Exception InvalidArrayEndingSymbol()
		{
			return new ArgumentException(Resources.Resources.Error_InvalidArrayEndingSymbol);
		}

		internal static Exception OpeningBraceExpected()
		{
			return new ArgumentException(Resources.Resources.Error_OpeningBraceExpected);
		}

		internal static Exception CouldNotResolveType(string typeId)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_CouldNotResolveType, typeId));
		}

		internal static Exception TypeMustHaveParameterlessConstructor(Type type)
		{
			return new MissingMethodException(String.Format(
				Resources.Resources.Error_TypeMustHaveParameterlessConstructor, type.FullName));
		}

		internal static Exception DeserializerTypeMismatch()
		{
			return new InvalidOperationException(Resources.Resources.Error_DeserializerTypeMismatch);
		}

		internal static Exception CouldNotCreateListType(Type type)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_CouldNotCreateListType, type.FullName));
		}

		internal static Exception IllegalJavaScriptPrimitive()
		{
			return new ArgumentException(Resources.Resources.Error_IllegalJavaScriptPrimitive);
		}

		internal static Exception StringIsNotQuoted()
		{
			return new ArgumentException(Resources.Resources.Error_StringIsNotQuoted);
		}

		internal static Exception InvalidEscapeSequence()
		{
			return new ArgumentException(Resources.Resources.Error_InvalidEscapeSequence);
		}

		internal static Exception UnterminatedStringConstant()
		{
			return new ArgumentException(Resources.Resources.Error_UnterminatedStringConstant);
		}

		internal static Exception IllegalArrayStartingSymbol()
		{
			return new ArgumentException(Resources.Resources.Error_IllegalArrayStartingSymbol);
		}

		internal static Exception InvalidMemberName()
		{
			return new ArgumentException(Resources.Resources.Error_InvalidMemberName);
		}

		internal static Exception InvalidObjectDefinition()
		{
			return new ArgumentException(Resources.Resources.Error_InvalidObjectDefinition);
		}

		internal static Exception DepthLimitExceeded()
		{
			return new ArgumentException(Resources.Resources.Error_DepthLimitExceeded);
		}

		internal static Exception MaximumJsonStringLengthExceeded()
		{
			return new ArgumentException(Resources.Resources.Error_MaximumJsonStringLengthExceeded);
		}

		internal static Exception SuppliedDictionaryTypeIsNotSupported()
		{
			return new ArgumentException(Resources.Resources.Error_SuppliedDictionaryTypeIsNotSupported);
		}

		internal static Exception MaximumRecursionDepthExceeded()
		{
			return new ArgumentException(Resources.Resources.Error_MaximumRecursionDepthExceeded);
		}

		internal static Exception InvalidEnumType(Type type)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_InvalidEnumType, type.FullName));
		}
	}
}
