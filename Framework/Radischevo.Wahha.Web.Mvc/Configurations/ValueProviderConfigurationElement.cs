using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	internal sealed class ValueProviderConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get
			{
				return base["name"].ToString();
			}
		}

		[ConfigurationProperty("order", IsRequired = false)]
		public int Order
		{
			get
			{
				return (int)base["order"];
			}
		}

		[ConfigurationProperty("factory", IsRequired = true)]
		public string FactoryType
		{
			get
			{
				return base["factory"].ToString();
			}
		}
	}
}
