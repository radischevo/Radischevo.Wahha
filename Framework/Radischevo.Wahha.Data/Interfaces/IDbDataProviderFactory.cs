using System;

namespace Radischevo.Wahha.Data
{
    public interface IDbDataProviderFactory
    {
        IDbDataProvider CreateProvider(Type providerType);
        IDbDataProvider CreateProvider(string providerName);
        void DisposeProvider(IDbDataProvider provider);
    }
}
