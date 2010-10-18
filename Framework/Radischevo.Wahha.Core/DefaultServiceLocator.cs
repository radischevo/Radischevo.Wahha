using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Core
{
	public sealed class DefaultServiceLocator : IServiceLocator
	{
		#region Instance Methods
		public object GetService(Type serviceType)
		{
			return Activator.CreateInstance(serviceType);
		}

		public object GetService(Type serviceType, string key)
		{
			return GetService(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return Enumerable.Empty<object>();
		}
		#endregion
	}
}
