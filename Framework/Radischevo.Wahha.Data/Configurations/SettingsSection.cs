using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
    internal sealed class SettingsSection : ConfigurationSection
    {
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
    }
}
