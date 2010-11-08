using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Routing.Configurations
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

		[ConfigurationProperty("variables")]
		public NameValueConfigurationCollection Variables
		{
			get
			{
				return (NameValueConfigurationCollection)base["variables"];
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
