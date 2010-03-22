using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    [ConfigurationCollection(typeof(ModelConfigurationElement))]
    internal sealed class ModelConfigurationElementCollection : ConfigurationElementCollection
    {
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

        protected override ConfigurationElement CreateNewElement()
        {
            return new ModelConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModelConfigurationElement)element).ModelType;
        }

        public ModelConfigurationElement this[int index]
        {
            get
            {
                return (ModelConfigurationElement)BaseGet(index);
            }
        }
    }
}
