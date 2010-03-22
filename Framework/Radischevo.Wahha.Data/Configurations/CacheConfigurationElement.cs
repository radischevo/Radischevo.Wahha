using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
    internal sealed class CacheConfigurationElement : ConfigurationElement
    {
        #region Instance Properties
        [ConfigurationProperty("provider", IsRequired = true)]
        public CacheProviderConfigurationElement Provider
        {
            get
            {
                return (CacheProviderConfigurationElement)base["provider"];
            }
        }

        [ConfigurationProperty("settings")]
        public NameValueConfigurationCollection Settings
        {
            get
            {
                return (NameValueConfigurationCollection)base["settings"];
            }
        }
        #endregion
    }
}
