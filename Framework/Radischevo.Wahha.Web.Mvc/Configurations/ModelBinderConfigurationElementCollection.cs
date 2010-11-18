using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    [ConfigurationCollection(typeof(ModelBinderConfigurationElement))]
    internal sealed class ModelBinderConfigurationElementCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("default", IsRequired = false)]
        public string DefaultType
        {
            get
            {
                return base["default"].ToString();
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ModelBinderConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModelBinderConfigurationElement)element).ModelType;
        }

        public ModelBinderConfigurationElement this[int index]
        {
            get
            {
                return (ModelBinderConfigurationElement)BaseGet(index);
            }
        }
    }
}
