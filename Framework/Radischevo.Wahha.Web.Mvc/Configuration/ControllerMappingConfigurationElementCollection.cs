using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configuration
{
    [ConfigurationCollection(typeof(ControllerMappingConfigurationElement))]
    internal sealed class ControllerMappingConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ControllerMappingConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ControllerMappingConfigurationElement)element).Name;
        }

        public ControllerMappingConfigurationElement this[int index]
        {
            get
            {
                return (ControllerMappingConfigurationElement)BaseGet(index);
            }
        }
    }
}
