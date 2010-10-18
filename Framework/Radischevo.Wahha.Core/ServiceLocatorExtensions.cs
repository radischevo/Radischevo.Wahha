using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Core
{
	public static class ServiceLocatorExtensions
	{
		#region Extension Methods
		public static TService GetService<TService>(this IServiceLocator locator)
		{
			return (TService)locator.GetService(typeof(TService));
		}

		public static TService GetService<TService>(this IServiceLocator locator, string key)
		{
			return (TService)locator.GetService(typeof(TService), key);
		}

		public static IEnumerable<TService> GetServices<TService>(this IServiceLocator locator)
		{
			return locator.GetServices(typeof(TService)).Cast<TService>();
		}
		#endregion
	}
}
