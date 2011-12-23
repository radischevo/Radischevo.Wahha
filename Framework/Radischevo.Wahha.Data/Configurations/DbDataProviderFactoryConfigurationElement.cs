using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Configurations
{
    internal sealed class DbDataProviderFactoryConfigurationElement : ConfigurationElement
    {
		#region Instance Fields
		private ValueDictionary _parameters;
		#endregion
		
		#region Constructors
		public DbDataProviderFactoryConfigurationElement()
			: base()
		{
			_parameters = new ValueDictionary();
		}
		#endregion
		
        #region Instance Properties
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return base["name"].ToString();
			}
		}
		
        [ConfigurationProperty("factory", IsRequired = true)]
        public string FactoryType
        {
            get
            {
                return base["factory"].ToString();
            }
        }
		
		[ConfigurationProperty("connectionStrings", IsRequired = true)]
		public ConnectionStringSettingsCollection ConnectionStrings
		{
			get
			{
				return (ConnectionStringSettingsCollection)base["connectionStrings"];
			}
		}
		
		public IValueSet Parameters
		{
			get
			{
				return _parameters;
			}
		}
        #endregion
		
		#region Instance Methods
		protected sealed override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			_parameters[name] = value;
			return true;
		}
		#endregion
    }
}
