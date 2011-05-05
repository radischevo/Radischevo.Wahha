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
                String.Format(Res.Error_ParameterMustBeGreaterThan, parameterName, lowerBound));
        }

        internal static Exception ParameterMustBeGreaterThanOrEqual(string parameterName, object lowerBound, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue,
                String.Format(Res.Error_ParameterMustBeGreaterThanOrEqual, parameterName, lowerBound));
        }

        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        internal static Exception ParameterMismatch()
        {
            return new FormatException(Res.Error_FormatParameterMismatch);
        }

        internal static Exception CommandTextIsNull()
        {
            return new ArgumentException(Res.Error_CommandTextIsNull);
        }

        internal static Exception UnableToConnect()
        {
            return new DataException(Res.Error_UnableToConnect);
        }

        internal static Exception ConnectionStringNotInitialized()
        {
            return new ArgumentException(Res.Error_ConnectionStringNotInitialized);
        }

        internal static Exception ConnectionStringNotConfigured()
        {
            return new ConfigurationErrorsException(Res.Error_ConnectionStringNotConfigured);
        }

        internal static Exception IncompatibleProviderType(Type type)
        {
            return new InvalidOperationException(
                String.Format(Res.Error_IncompatibleProviderType, type.FullName));
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

        internal static Exception ProviderNotConfigured()
        {
            return new InvalidOperationException(Res.Error_ProviderNotConfigured);
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

        internal static Exception CommandIsNotInitialized(string propertyName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CommandIsNotInitialized, propertyName));
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

        internal static Exception CannotAddMoreThanOneDefaultProvider(string existingName, string attemptedName)
        {
            return new InvalidOperationException(
                String.Format(Res.Error_CannotAddMoreThanOneDefaultProvider, existingName, attemptedName));
        }

        internal static Exception ProviderTypeMappingNotConfigured(string providerName)
        {
            return new InvalidOperationException(
                String.Format(Res.Error_ProviderTypeMappingNotConfigured, providerName));
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

		internal static Exception OperationIsNotValid()
		{
			return new DbOperationValidationException();
		}

		internal static Exception OperationCommandIsNotInitialized()
		{
			return new DbOperationValidationException(
				Res.Error_OperationCommandIsNotInitialized);
		}
	}
}
