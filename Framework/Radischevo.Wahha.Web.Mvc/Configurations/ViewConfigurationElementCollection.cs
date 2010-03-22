using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    [ConfigurationCollection(typeof(ViewConfigurationElement))]
    internal sealed class ViewConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ViewConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return new object();
        }

        public ViewConfigurationElement this[int index]
        {
            get
            {
                return (ViewConfigurationElement)BaseGet(index);
            }
        }
    }
}
