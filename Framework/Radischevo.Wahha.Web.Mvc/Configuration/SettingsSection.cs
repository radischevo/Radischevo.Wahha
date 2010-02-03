using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configuration
{
    internal sealed class SettingsSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the configuration element, 
        /// which provides the information required for  
        /// controller instantiation
        /// </summary>
        [ConfigurationProperty("controllers", IsRequired = false)]
        public ControllerConfigurationElement Controllers
        {
            get
            {
                return (ControllerConfigurationElement)base["controllers"];
            }
        }

        /// <summary>
        /// Gets the configuration element, 
        /// which provides the information required for  
        /// view discovery and instantiation.
        /// </summary>
        [ConfigurationProperty("views", IsRequired = false)]
        public ViewConfigurationElementCollection Views
        {
            get
            {
                return (ViewConfigurationElementCollection)base["views"];
            }
        }

        /// <summary>
        /// Gets the configuration element, 
        /// which provides the information required for  
        /// model binding.
        /// </summary>
        [ConfigurationProperty("models", IsRequired = false)]
        public ModelConfigurationElementCollection Models
        {
            get
            {
                return (ModelConfigurationElementCollection)base["models"];
            }
        }
    }
}
