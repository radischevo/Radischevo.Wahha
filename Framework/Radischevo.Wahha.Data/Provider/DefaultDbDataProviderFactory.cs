using System;
using System.Reflection;

using Radischevo.Wahha.Core;
using Config = Radischevo.Wahha.Data.Configuration;

namespace Radischevo.Wahha.Data
{
    public class DefaultDbDataProviderFactory : IDbDataProviderFactory
    {
        #region Constructors
        public DefaultDbDataProviderFactory()
        {   }
        #endregion

        #region Static Methods
        private static bool IsDataProvider(Type type)
        {
            if (type.IsAbstract || type.IsInterface ||
                type.IsGenericTypeDefinition || type.IsGenericType ||
                type.GetInterface(typeof(IDbDataProvider).Name) == null ||
                type == typeof(DbDataProvider))
                return false;

            if (type.GetConstructor(Type.EmptyTypes) == null)
                return false;

            return true;
        }
        #endregion

        #region Instance Methods
        public IDbDataProvider CreateProvider(Type providerType)
        {
            Precondition.Require(providerType, Error.ArgumentNull("providerType"));

            if (!IsDataProvider(providerType))
                throw Error.IncompatibleProviderType(providerType);

            try
            {
                return (IDbDataProvider)Activator.CreateInstance(providerType);
            }
            catch (TargetInvocationException te)
            {
                throw te.InnerException;
            }
        }

        public IDbDataProvider CreateProvider(string providerName)
        {
            Precondition.Require(!String.IsNullOrEmpty(providerName),
                Error.ArgumentNull("providerName"));

            Type type;
            if (!Config.Configuration.Instance.Providers
                .Mappings.TryGetValue(providerName, out type)
                || type == null)
                throw Error.ProviderTypeMappingNotConfigured(providerName);

            return CreateProvider(type);
        }

        public void DisposeProvider(IDbDataProvider provider)
        {
            Precondition.Require(provider, 
                Error.ArgumentNull("provider"));

            provider.Dispose();
        }
        #endregion
    }
}
