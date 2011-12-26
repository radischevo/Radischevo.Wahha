using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    internal sealed class ControllerConfigurationElement : ConfigurationElement, IConfigurator<ControllerConfigurationSettings>
    {
        #region Constructors
        public ControllerConfigurationElement()
            : base()
        {
        }
        #endregion

        #region Instance Properties
		/// <summary>
		/// Gets the controller factory settings.
		/// </summary>
		[ConfigurationProperty("factory", IsRequired = false)]
		public ControllerFactoryConfigurationElement Factory
		{
			get
			{
				return (ControllerFactoryConfigurationElement)base["factory"];
			}
		}

        /// <summary>
		/// Gets the collection of configured filter providers.
		/// </summary>
		[ConfigurationProperty("filterProviders", IsRequired = false)]
		public FilterProviderConfigurationElementCollection FilterProviders
		{
			get
			{
				return (FilterProviderConfigurationElementCollection)base["filterProviders"];
			}
		}

        /// <summary>
        /// Gets the predefined controller name-to-type mappings
        /// </summary>
        [ConfigurationProperty("mappings", IsRequired = false)]
        public ControllerMappingConfigurationElementCollection Mappings
        {
            get
            {
                return (ControllerMappingConfigurationElementCollection)base["mappings"];
            }
        }
        #endregion
		
		#region Static Methods
		private static IFilterProvider CreateFilterProvider(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IFilterProvider).IsAssignableFrom(type))
				throw Error.IncompatibleFilterProviderType(type);

			return (IFilterProvider)ServiceLocator.Instance.GetService(type);
		}

		private static IControllerFactory CreateControllerFactory(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IControllerFactory).IsAssignableFrom(type))
				throw Error.IncompatibleControllerFactoryType(type);

			return (IControllerFactory)ServiceLocator.Instance.GetService(type);
		}
		#endregion
		
		#region Instance Methods
		public void Configure (ControllerConfigurationSettings module)
		{
			if (Factory != null) 
			{
				Type type = String.IsNullOrEmpty(Factory.FactoryType) ? 
					typeof(DefaultControllerFactory) : 
					Type.GetType(Factory.FactoryType, true, true);
				
				IControllerFactory factory = CreateControllerFactory(type);
				factory.Init(Factory.Parameters);
				
				module.Factory = factory;
			}
			
			if (Mappings != null) 
			{
				foreach (ControllerMappingConfigurationElement map in Mappings)
					module.Mappings.Add(map.Name, Type.GetType(map.ControllerType, true, true));
			}
			
			if (FilterProviders != null)
			{
				foreach (FilterProviderConfigurationElement elem in FilterProviders)
				{
					IFilterProvider provider = CreateFilterProvider(
						Type.GetType(elem.ProviderType, true, true));
	
					provider.Init(elem.Parameters);
					module.FilterProviders.Add(provider);
				}
			}
		}
		#endregion
    }
}
