using System;

namespace Radischevo.Wahha.Data.Provider
{
	public class OdbcDbDataProviderFactory : CommonDbDataProviderFactory
	{
		#region Instance Methods
		protected override IDbDataProvider CreateProvider(string connectionString)
		{
			return new OdbcDbDataProvider(connectionString);
		}
		#endregion
	}
}
