using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
    internal sealed class DbDataProviderConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, 
            DefaultValue = "", IsKey = true)]
        public string Name
        {
            get
            {
                return base["name"].ToString();
            }
        }

        [ConfigurationProperty("type", IsRequired = false, 
            DefaultValue = "")]
        public string Type
        {
            get
            {
                return base["type"].ToString();
            }
        }

        [ConfigurationProperty("default", IsRequired = false,
            DefaultValue = false)]
        public bool IsDefault
        {
            get
            {
                return (bool)base["default"];
            }
        }
    }
}
