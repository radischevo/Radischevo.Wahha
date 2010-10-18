using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Microsoft.Practices.Unity;

namespace ConsoleTester
{
	public class UnityServiceLocator : IServiceLocator
	{
		#region Constructors
		public UnityServiceLocator()
		{
		}
		#endregion

		#region Instance Properties
		protected IUnityContainer Container
		{
			get
			{
				return Configuration.Instance.Container;
			}
		}
		#endregion

		#region Instance Methods
		public object GetService(Type serviceType)
		{
			return Container.Resolve(serviceType);
		}

		public object GetService(Type serviceType, string key)
		{
			return Container.Resolve(serviceType, key);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return Container.ResolveAll(serviceType);
		}
		#endregion
	}
}
