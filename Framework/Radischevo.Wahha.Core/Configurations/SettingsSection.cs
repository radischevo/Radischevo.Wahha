using System;
using System.Configuration;

namespace Radischevo.Wahha.Core.Configurations
{
    internal sealed class SettingsSection : ConfigurationSection, IConfigurator<Configuration>
    {
		#region Instance Properties	
		[ConfigurationProperty("serviceLocation", IsRequired = false)]
        public ServiceLocationConfigurationElement ServiceLocation
        {
            get
            {
                return (ServiceLocationConfigurationElement)base["serviceLocation"];
            }
        }
		#endregion
		
		#region Instance Methods
		public void Configure(Configuration module)
		{
			if (ServiceLocation == null)
				return;
			
			module.ServiceLocation.ServiceLocatorType = Type.GetType(
				ServiceLocation.ProviderType, false, true);
		}
		#endregion
    }
}
