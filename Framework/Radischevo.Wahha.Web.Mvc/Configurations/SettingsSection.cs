using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    internal sealed class SettingsSection : ConfigurationSection, IConfigurator<Configuration>
    {
		#region Instance Properties
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
        public ModelConfigurationElement Models
        {
            get
            {
                return (ModelConfigurationElement)base["models"];
            }
        }
		#endregion
		
        #region Instance Methods
		public void Configure (Configuration module)
		{
			if(Controllers != null)
				Controllers.Configure(module.Controllers);
			
			if (Models != null)
                Models.Configure(module.Models);
			
            if (Views != null)
                Views.Configure(module.Views);
		}
		#endregion
    }
}
