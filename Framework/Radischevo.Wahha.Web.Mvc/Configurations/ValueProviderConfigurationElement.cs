using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	internal sealed class ValueProviderConfigurationElement : ConfigurationElement
	{
		#region Instance Properties
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get
			{
				return base["name"].ToString();
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
		#endregion
	}
}
