using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
	[ConfigurationCollection(typeof(DbDataProviderFactoryConfigurationElement))]
    internal sealed class DbDataProviderFactoryConfigurationElementCollection : ConfigurationElementCollection
    {
		#region Instance Properties
		public DbDataProviderFactoryConfigurationElement this[int index]
        {
            get
            {
                return (DbDataProviderFactoryConfigurationElement)BaseGet(index);
            }
        }
		#endregion
		
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
        {
            return new DbDataProviderFactoryConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DbDataProviderFactoryConfigurationElement)element).Name;
        }
		#endregion
    }
}