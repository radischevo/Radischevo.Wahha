﻿using System;

namespace Radischevo.Wahha.Data.Provider
{
	public class OleDbDataProviderFactory : CommonDbDataProviderFactory
	{
		#region Instance Methods
		public override IDbDataProvider CreateProvider(string connectionString)
		{
			return new OleDbDataProvider(connectionString);
		}
		#endregion
	}
}