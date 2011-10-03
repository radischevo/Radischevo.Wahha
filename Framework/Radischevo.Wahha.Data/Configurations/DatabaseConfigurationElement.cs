using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
	internal sealed class DatabaseConfigurationElement : ConfigurationElement
	{
		#region Instance Properties
		[ConfigurationProperty("provider", IsRequired = true)]
		public DbDataProviderFactoryConfigurationElement Provider
		{
			get
			{
				return (DbDataProviderFactoryConfigurationElement)base["provider"];
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

		[ConfigurationProperty("connectionStrings")]
		public ConnectionStringSettingsCollection ConnectionStrings
		{
			get
			{
				return (ConnectionStringSettingsCollection)base["connectionStrings"];
			}
		}
		#endregion
	}
}