using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    internal sealed class ModelBinderConfigurationElement : ConfigurationElement
    {
		#region Instance Properties
		[ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public string ModelType
        {
            get
            {
                return base["type"].ToString();
            }
        }

        [ConfigurationProperty("binder", IsRequired = false)]
        public string BinderType
        {
            get
            {
                return base["binder"].ToString();
            }
        }
		#endregion
    }
}

