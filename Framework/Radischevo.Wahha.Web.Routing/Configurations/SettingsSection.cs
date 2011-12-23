using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Configurations
{
    /// <summary>
    /// Describes the configuration section, 
    /// storing the routing provider settings
    /// </summary>
    internal sealed class SettingsSection : ConfigurationSection, IConfigurator<Configuration>
    {
	    #region Instance Properties
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
	    #endregion
	
		#region Instance Methods
		public void Configure (Configuration module)
		{
			module.Provider = CreateRouteTableProvider();
			
			if (Variables != null) 
			{
				foreach (NameValueConfigurationElement element in Variables)
					module.Variables.Add(element.Name, element.Value);
			}
			
			if (Routes != null) 
			{
				module.DefaultHandlerType = Type.GetType(Routes.DefaultHandlerType, false, true);
				
				foreach (RouteConfigurationElement element in Routes)
                	module.Routes.Add(element);		
			}
		}
		
		private IRouteTableProvider CreateRouteTableProvider ()
        {
			if (Provider == null)
				return null;
			
            Type providerType = Type.GetType(Provider.Type, false, true);
            if (providerType == null)
                return null;

            if (!typeof(IRouteTableProvider).IsAssignableFrom(providerType))
                throw Error.IncompatibleRouteTableProvider(providerType);

            IRouteTableProvider provider = (IRouteTableProvider)ServiceLocator.Instance.GetService(providerType);

            ValueDictionary settings = new ValueDictionary();
            foreach (string key in Provider.Settings.AllKeys)
                settings.Add(key, Provider.Settings[key].Value);

            provider.Init(settings);
			return provider;
        }
		#endregion
    }
}
