using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	internal sealed class ModelConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("binders", IsRequired = false)]
		public ModelBinderConfigurationElementCollection Binders
		{
			get
			{
				return (ModelBinderConfigurationElementCollection)base["binders"];
			}
		}

		[ConfigurationProperty("valueProviders", IsRequired = false)]
		public ValueProviderConfigurationElementCollection ValueProviders
		{
			get
			{
				return (ValueProviderConfigurationElementCollection)base["valueProviders"];
			}
		}

		[ConfigurationProperty("metadataProvider", IsRequired = false)]
		public string MetadataProviderType
		{
			get
			{
				return base["metadataProvider"].ToString();
			}
		}

		[ConfigurationProperty("validatorProvider", IsRequired = false)]
		public string ValidatorProviderType
		{
			get
			{
				return base["validatorProvider"].ToString();
			}
		}
	}
}
