using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Configurations
{
    internal sealed class SettingsSection : ConfigurationSection, IConfigurator<Configuration>
    {
		#region Instance Properties
		[ConfigurationProperty("database", IsRequired = true)]
        public DatabaseConfigurationElement Database
        {
            get
            {
                return (DatabaseConfigurationElement)base["database"];
            }
        }

        [ConfigurationProperty("cache", IsRequired = false)]
        public CacheConfigurationElement Cache
        {
            get
            {
                return (CacheConfigurationElement)base["cache"];
            }
        }
		#endregion

		#region Instance Methods
		public void Configure (Configuration module)
		{
			if (Database != null) 
				Database.Configure(module.Database);
			
			if (Cache != null)
				Cache.Configure(module.Caching);
		}
		#endregion
    }
}
