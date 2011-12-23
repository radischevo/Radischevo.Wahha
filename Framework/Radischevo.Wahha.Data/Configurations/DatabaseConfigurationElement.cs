using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Configurations
{
	internal sealed class DatabaseConfigurationElement : ConfigurationElement, IConfigurator<DatabaseConfigurationSettings>
	{
		#region Instance Properties
		[ConfigurationProperty("providers", IsRequired = true)]
		public DbDataProviderFactoryConfigurationElementCollection Providers
		{
			get
			{
				return (DbDataProviderFactoryConfigurationElementCollection)base["providers"];
			}
		}
		#endregion
		
		#region Static Methods
		private static IDbDataProviderFactory CreateFactory(Type type)
		{
			if (!typeof(IDbDataProviderFactory).IsAssignableFrom(type))
				throw Error.IncompatibleDataProviderFactoryType(type);

			return (IDbDataProviderFactory)ServiceLocator.Instance.GetService(type);
		}
		#endregion
		
		#region Instance Methods
		public void Configure (DatabaseConfigurationSettings module)
		{
			foreach (DbDataProviderFactoryConfigurationElement element in Providers)
				module.Providers.Add(element.Name, ConfigureFactory(element));
		}
		
		private IDbDataProviderFactory ConfigureFactory(DbDataProviderFactoryConfigurationElement element)
		{
			IDbDataProviderFactory factory = CreateFactory(Type.GetType(element.FactoryType, false, true));
			DbDataProviderFactorySettings settings = new DbDataProviderFactorySettings();
			
			foreach (ConnectionStringSettings item in element.ConnectionStrings)
				settings.ConnectionStrings.Add(item.Name, item.ConnectionString);
			
			settings.Parameters = element.Parameters;
			factory.Init(settings);
			
			return factory;
		}
		#endregion
	}
}