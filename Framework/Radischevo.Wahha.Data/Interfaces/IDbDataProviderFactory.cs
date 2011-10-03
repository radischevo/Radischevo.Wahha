using System;

namespace Radischevo.Wahha.Data
{
    public interface IDbDataProviderFactory
	{
		#region Instance Methods
		IDbDataProvider CreateProvider();

		IDbDataProvider CreateProvider(string connectionString);

		void DisposeProvider(IDbDataProvider provider);
		#endregion
    }
}
