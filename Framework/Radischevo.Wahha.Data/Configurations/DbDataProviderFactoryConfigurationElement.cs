using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
    [ConfigurationCollection(typeof(DbDataProviderConfigurationElement))]
    internal sealed class DbDataProviderFactoryConfigurationElement : ConfigurationElementCollection
    {
        #region Instance Properties
        [ConfigurationProperty("factory", IsRequired = false, DefaultValue = "")]
        public string FactoryType
        {
            get
            {
                return base["factory"].ToString();
            }
        }

        [ConfigurationProperty("commandTimeout", IsRequired = false)]
        public int CommandTimeout
        {
            get
            {
                return (int)base["commandTimeout"];
            }
        }

        public DbDataProviderConfigurationElement this[int index]
        {
            get
            {
                return (DbDataProviderConfigurationElement)BaseGet(index);
            }
        }

        public new DbDataProviderConfigurationElement this[string name]
        {
            get
            {
                return (DbDataProviderConfigurationElement)BaseGet(name);
            }
        }
        #endregion

        #region Instance Methods
        protected override ConfigurationElement CreateNewElement()
        {
            return new DbDataProviderConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DbDataProviderConfigurationElement)element).Name;
        }
        #endregion
    }
}
