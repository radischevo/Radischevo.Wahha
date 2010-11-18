using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	[ConfigurationCollection(typeof(ValueProviderConfigurationElement))]
	internal sealed class ValueProviderConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new ModelBinderConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ValueProviderConfigurationElement)element).Name;
		}

		public ValueProviderConfigurationElement this[int index]
		{
			get
			{
				return (ValueProviderConfigurationElement)BaseGet(index);
			}
		}
	}
}
