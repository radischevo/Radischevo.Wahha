using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configuration
{
    internal sealed class CacheProviderConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = false,
            DefaultValue = "")]
        public string Type
        {
            get
            {
                return base["type"].ToString();
            }
        }
    }
}
