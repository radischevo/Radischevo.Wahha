using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using Radischevo.Wahha.Data;

namespace ConsoleTester
{
	public sealed class Configuration
	{
		private static Configuration _instance;
		private static object _lock = new object();
		private IUnityContainer _container;

		static Configuration()
		{
		}

		private Configuration()
		{
			UnityConfigurationSection section = ConfigurationManager.GetSection("unity")
				as UnityConfigurationSection;

			_container = new UnityContainer();
			section.Configure(_container, "default");
		}

		public static Configuration Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
							_instance = new Configuration();
					}
				}
				return _instance;
			}
		}

		public IUnityContainer Container
		{
			get
			{
				return _container;
			}
		}
	}
}
