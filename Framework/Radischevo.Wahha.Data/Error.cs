using System;
using System.Configuration;
using System.Data;

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
                String.Format(Resources.Resources.Error_ColumnOrdinalDoesNotExistInResultSet, ordinal));
        }

        internal static Exception ColumnNameDoesNotExistInResultSet(string name)
        {
            return new ArgumentOutOfRangeException("name", name,
                String.Format(Resources.Resources.Error_ColumnNameDoesNotExistInResultSet, name));
        }

        internal static Exception ParameterMustBeGreaterThan(string parameterName, object lowerBound, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue, 
                String.Format(Resources.Resources.Error_ParameterMustBeGreaterThan, parameterName, lowerBound));
        }

        internal static Exception ParameterMustBeGreaterThanOrEqual(string parameterName, object lowerBound, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue,
                String.Format(Resources.Resources.Error_ParameterMustBeGreaterThanOrEqual, parameterName, lowerBound));
        }

        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        internal static Exception ParameterMismatch()
        {
            return new FormatException(Resources.Resources.Error_FormatParameterMismatch);
        }

        internal static Exception CommandTextIsNull()
        {
            return new ArgumentException(Resources.Resources.Error_CommandTextIsNull);
        }

        internal static Exception UnableToConnect()
        {
            return new DataException(Resources.Resources.Error_UnableToConnect);
        }

        internal static Exception ConnectionStringNotInitialized()
        {
            return new ArgumentException(Resources.Resources.Error_ConnectionStringNotInitialized);
        }

        internal static Exception ConnectionStringNotConfigured()
        {
            return new ConfigurationErrorsException(Resources.Resources.Error_ConnectionStringNotConfigured);
        }

        internal static Exception IncompatibleProviderType(Type type)
        {
            return new InvalidOperationException(
                String.Format(Resources.Resources.Error_IncompatibleProviderType, type.FullName));
        }

        internal static Exception UnableToLoadConfiguration(Exception inner)
        {
            return new ConfigurationErrorsException(Resources.Resources.Error_UnableToLoadConfiguration, inner);
        }

        internal static Exception AmbiguousColumnName(string name)
        {
            return new InvalidOperationException(
                String.Format(Resources.Resources.Error_AmbiguousColumnName, name));
        }

        internal static Exception ReaderIsEmpty()
        {
            return new InvalidOperationException(Resources.Resources.Error_ReaderIsEmpty);
        }

        internal static Exception ProviderNotConfigured()
        {
            return new InvalidOperationException(Resources.Resources.Error_ProviderNotConfigured);
        }

        internal static Exception CannotEnumerateMoreThanOnce()
        {
            return new InvalidOperationException(Resources.Resources.Error_CannotEnumerateMoreThanOnce);
        }

        internal static Exception UnsupportedCommandType(Type requiredType, Type actualType)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_UnsupportedCommandType, actualType.FullName, requiredType.Name));
        }

        internal static Exception CommandIsNotInitialized(string propertyName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_CommandIsNotInitialized, propertyName));
        }

        internal static Exception IncompatibleCacheProviderType(Type type)
        {
            return new InvalidOperationException(
                String.Format(Resources.Resources.Error_IncompatibleCacheProviderType, type.FullName));
        }

        internal static Exception CachingProviderDoesNotSupportTags(Type type)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_CacheProviderDoesNotSupportTags, type.FullName));
        }

        internal static Exception IncompatibleDataProviderFactoryType(Type type)
        {
            return new InvalidOperationException(
                String.Format(Resources.Resources.Error_IncompatibleProviderFactoryType, type.FullName));
        }

        internal static Exception CannotAddMoreThanOneDefaultProvider(string existingName, string attemptedName)
        {
            return new InvalidOperationException(
                String.Format(Resources.Resources.Error_CannotAddMoreThanOneDefaultProvider, existingName, attemptedName));
        }

        internal static Exception ProviderTypeMappingNotConfigured(string providerName)
        {
            return new InvalidOperationException(
                String.Format(Resources.Resources.Error_ProviderTypeMappingNotConfigured, providerName));
        }

        internal static Exception ObjectDisposed(string objectName)
        {
            return new ObjectDisposedException(objectName,
                String.Format(Resources.Resources.Error_ObjectDisposed, objectName));
        }
	}
}
