using System;
using System.Configuration;

namespace Radischevo.Wahha.Core.Configurations
{
	internal sealed class ServiceLocationConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("provider", IsRequired = false, DefaultValue = "")]
		public string ProviderType
		{
			get
			{
				return base["provider"].ToString();
			}
		}
	}
}
