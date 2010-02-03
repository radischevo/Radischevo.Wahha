using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configuration
{
    internal sealed class SettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("providers", IsRequired = true, IsDefaultCollection = true)]
        public DbDataProviderFactoryConfigurationElement Providers
        {
            get
            {
                return (DbDataProviderFactoryConfigurationElement)base["providers"];
            }
        }

        [ConfigurationProperty("connectionStrings")]
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return (ConnectionStringSettingsCollection)base["connectionStrings"];
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
    }
}
