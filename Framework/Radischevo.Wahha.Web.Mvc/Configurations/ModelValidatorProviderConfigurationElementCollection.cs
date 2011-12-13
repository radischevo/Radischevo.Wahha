using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	[ConfigurationCollection(typeof(ModelValidatorProviderConfigurationElement))]
    internal sealed class ModelValidatorProviderConfigurationElementCollection : ConfigurationElementCollection
    {
		#region Instance Properties
		public ModelValidatorProviderConfigurationElement this[int index]
        {
            get
            {
                return (ModelValidatorProviderConfigurationElement)BaseGet(index);
            }
        }
		#endregion
		
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
        {
            return new ModelValidatorProviderConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModelValidatorProviderConfigurationElement)element).Type;
        }
		#endregion
    }
}

