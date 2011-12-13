using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	internal sealed class ModelConfigurationElement : ConfigurationElement
	{
		#region Instance Properties
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
		
		[ConfigurationProperty("validatorProviders", IsRequired = false)]
		public ModelValidatorProviderConfigurationElementCollection ValidatorProviders
		{
			get
			{
				return (ModelValidatorProviderConfigurationElementCollection)base["validatorProviders"];
			}
		}
		#endregion
	}
}
