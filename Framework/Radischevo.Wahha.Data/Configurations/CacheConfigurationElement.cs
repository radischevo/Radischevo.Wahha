using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Configurations
{
    internal sealed class CacheConfigurationElement : ConfigurationElement, IConfigurator<CacheSettings>
    {
        #region Instance Properties
        [ConfigurationProperty("provider", IsRequired = true)]
        public CacheProviderConfigurationElement Provider
        {
            get
            {
                return (CacheProviderConfigurationElement)base["provider"];
            }
        }

        [ConfigurationProperty("settings")]
        public NameValueConfigurationCollection Settings
        {
            get
            {
                return (NameValueConfigurationCollection)base["settings"];
            }
        }
        #endregion

		#region Instance Methods
		public void Configure (CacheSettings module)
		{
			module.ProviderType = Type.GetType(Provider.Type, false, true);
			
			if (Settings != null) 
			{
				foreach (NameValueConfigurationElement item in Settings) 
					module.Settings[item.Name] = item.Value;
			}
		}
		#endregion
    }
}
