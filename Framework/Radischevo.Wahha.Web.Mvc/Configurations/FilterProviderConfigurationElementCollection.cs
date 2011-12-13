using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	[ConfigurationCollection(typeof(FilterProviderConfigurationElement))]
	internal sealed class FilterProviderConfigurationElementCollection : ConfigurationElementCollection
	{
		#region Instance Properties
		public FilterProviderConfigurationElement this[int index]
		{
			get
			{
				return (FilterProviderConfigurationElement)BaseGet(index);
			}
		}
		#endregion
		
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
		{
			return new FilterProviderConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((FilterProviderConfigurationElement)element).ProviderType;
		}
		#endregion
	}
}
