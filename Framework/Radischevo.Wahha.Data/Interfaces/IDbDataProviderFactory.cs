using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    public interface IDbDataProviderFactory
	{
		#region Instance Methods
		void Init(DbDataProviderFactorySettings settings);
		
		IDbDataProvider CreateProvider ();

		void DisposeProvider (IDbDataProvider provider);
		#endregion
    }
}
