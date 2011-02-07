using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    internal sealed class ControllerConfigurationElement : ConfigurationElement
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
    }
}
