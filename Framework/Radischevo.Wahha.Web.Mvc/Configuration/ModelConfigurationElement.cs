using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configuration
{
    internal sealed class ModelConfigurationElement : ConfigurationElement
    {
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

