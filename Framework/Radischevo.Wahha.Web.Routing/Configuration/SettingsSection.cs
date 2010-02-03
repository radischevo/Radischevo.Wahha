using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Routing.Configuration
{
    /// <summary>
    /// Describes the configuration section, 
    /// storing the routing provider settings
    /// </summary>
    internal sealed class SettingsSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the configuration element, 
        /// which provides the information required for  
        /// <see cref="IRouteTableProvider"/> instantiation.
        /// </summary>
        [ConfigurationProperty("provider", IsRequired = false)]
        public RouteTableProviderConfigurationElement Provider
        {
            get
            {
                return (RouteTableProviderConfigurationElement)base["provider"];
            }
        }

        [ConfigurationProperty("routes", IsRequired = false)]
        public RouteConfigurationElementCollection Routes
        {
            get
            {
                return (RouteConfigurationElementCollection)base["routes"];
            }
        }
    }
}
