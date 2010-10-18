using System;
using System.Reflection;

using Radischevo.Wahha.Core;
using Config = Radischevo.Wahha.Data.Configurations;

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
            if (!typeof(IDbDataProvider).IsAssignableFrom(type) ||
                type == typeof(DbDataProvider))
                return false;

            return true;
        }
        #endregion

        #region Instance Methods
        public IDbDataProvider CreateProvider(Type providerType)
        {
            Precondition.Require(providerType, () => Error.ArgumentNull("providerType"));

            if (!IsDataProvider(providerType))
                throw Error.IncompatibleProviderType(providerType);

            try
            {
				return (IDbDataProvider)ServiceLocator.Instance.GetService(providerType);
            }
            catch (TargetInvocationException te)
            {
                throw te.InnerException;
            }
        }

        public IDbDataProvider CreateProvider(string providerName)
        {
            Precondition.Defined(providerName,
				() => Error.ArgumentNull("providerName"));

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
				() => Error.ArgumentNull("provider"));

            provider.Dispose();
        }
        #endregion
    }
}
