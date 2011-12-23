using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Configurations
{
    public sealed class DatabaseConfigurationSettings
    {
        #region Instance Fields
        private DbDataProviderFactoryCollection _providers;
        #endregion

        #region Constructors
        internal DatabaseConfigurationSettings()
        {
            _providers = new DbDataProviderFactoryCollection();
        }
        #endregion

        #region Instance Properties
		public DbDataProviderFactoryCollection Providers
		{
			get
			{
				return _providers;
			}
		}
        #endregion
    }
}
