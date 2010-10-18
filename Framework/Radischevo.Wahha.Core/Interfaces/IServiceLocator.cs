using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Core
{
	public interface IServiceLocator : IServiceProvider
	{
		#region Instance Methods
		object GetService(Type serviceType, string key);
		IEnumerable<object> GetServices(Type serviceType);
		#endregion
	}
}
