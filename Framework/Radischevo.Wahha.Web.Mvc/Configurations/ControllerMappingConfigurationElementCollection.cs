using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    [ConfigurationCollection(typeof(ControllerMappingConfigurationElement))]
    internal sealed class ControllerMappingConfigurationElementCollection : ConfigurationElementCollection
    {
		#region Instance Properties
		public ControllerMappingConfigurationElement this[int index]
        {
            get
            {
                return (ControllerMappingConfigurationElement)BaseGet(index);
            }
        }
		#endregion
		
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
        {
            return new ControllerMappingConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ControllerMappingConfigurationElement)element).Name;
        }
		#endregion
    }
}
