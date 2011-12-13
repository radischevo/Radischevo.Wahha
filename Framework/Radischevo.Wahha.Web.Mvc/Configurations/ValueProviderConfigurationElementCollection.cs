using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	[ConfigurationCollection(typeof(ValueProviderConfigurationElement))]
	internal sealed class ValueProviderConfigurationElementCollection : ConfigurationElementCollection
	{
		#region Instance Properties
		public ValueProviderConfigurationElement this[int index]
		{
			get
			{
				return (ValueProviderConfigurationElement)BaseGet(index);
			}
		}
		#endregion
		
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
		{
			return new ModelBinderConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ValueProviderConfigurationElement)element).Name;
		}
		#endregion
	}
}
