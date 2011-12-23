using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
    internal sealed class CacheProviderConfigurationElement : ConfigurationElement
    {
		#region Instance Properties
		[ConfigurationProperty("type", IsRequired = false,
            DefaultValue = "")]
        public string Type
        {
            get
            {
                return base["type"].ToString();
            }
        }
		#endregion
    }
}
