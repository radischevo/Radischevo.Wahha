using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	internal sealed class ModelValidatorProviderConfigurationElement : ConfigurationElement
	{
		#region Instance Properties
		[ConfigurationProperty("type", IsKey = true, IsRequired = true)]
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

