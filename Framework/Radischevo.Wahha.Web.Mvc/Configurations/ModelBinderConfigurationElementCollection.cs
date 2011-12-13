using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    [ConfigurationCollection(typeof(ModelBinderConfigurationElement))]
    internal sealed class ModelBinderConfigurationElementCollection : ConfigurationElementCollection
    {
		#region Instance Properties
		[ConfigurationProperty("default", IsRequired = false)]
        public string DefaultType
        {
            get
            {
                return base["default"].ToString();
            }
        }
		
		public ModelBinderConfigurationElement this[int index]
        {
            get
            {
                return (ModelBinderConfigurationElement)BaseGet(index);
            }
        }
		#endregion
        
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
        {
            return new ModelBinderConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModelBinderConfigurationElement)element).ModelType;
        }
		#endregion
    }
}
