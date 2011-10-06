using System;

namespace Radischevo.Wahha.Data
{
    public interface IDbDataProviderFactory
	{
		#region Instance Methods
		IDbDataProvider CreateProvider();

		void DisposeProvider(IDbDataProvider provider);
		#endregion
    }
}
