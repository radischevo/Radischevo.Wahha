using System;
using System.Configuration;

namespace Radischevo.Wahha.Core.Configurations
{
    internal sealed class SettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("serviceLocation", IsRequired = false)]
        public ServiceLocationConfigurationElement ServiceLocation
        {
            get
            {
                return (ServiceLocationConfigurationElement)base["serviceLocation"];
            }
        }
    }
}
