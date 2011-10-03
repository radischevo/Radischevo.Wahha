using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
    internal sealed class DbDataProviderFactoryConfigurationElement : ConfigurationElement
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
        #endregion
    }
}
