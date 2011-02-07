using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core.Configurations;

namespace Radischevo.Wahha.Core
{
	public sealed class ServiceLocator : IServiceLocator
	{
		#region Static Fields
		private static ServiceLocator _instance;
		private static object _lock = new object();
		#endregion

		#region Instance Fields
		private IServiceLocator _provider;
		#endregion

		#region Constructors
		private ServiceLocator(IServiceLocator provider)
		{
			_provider = provider;
		}
		#endregion

		#region Static Properties
		public static IServiceLocator Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							ServiceLocationSettings settings = Configuration.Instance.ServiceLocation;
							_instance = Create(settings.ServiceLocatorType);
						}
					}
				}
				return _instance;
			}
		}
		#endregion

		#region Static Methods
		internal static bool IsProvider(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));

			if (type.IsAbstract || type.IsInterface ||
				type.IsGenericTypeDefinition || type.IsGenericType ||
				type.GetInterface(typeof(IServiceLocator).Name) == null ||
				type == typeof(ServiceLocator))
				return false;

			if (type.GetConstructor(Type.EmptyTypes) == null)
				return false;

			return true;
		}

		public static ServiceLocator Create(Type providerType)
		{
			if (!IsProvider(providerType))
				throw Error.IncompatibleServiceLocatorType(providerType);

			IServiceLocator provider = (IServiceLocator)Activator.CreateInstance(providerType);
			return new ServiceLocator(provider);
		}

		public static ServiceLocator Create<TLocator>()
			where TLocator : IServiceLocator, new()
		{
			IServiceLocator provider = new TLocator();
			return new ServiceLocator(provider);
		}
		#endregion

		#region Instance Methods
		public object GetService(Type serviceType)
		{
			return _provider.GetService(serviceType);
		}

		public object GetService(Type serviceType, string key)
		{
			return _provider.GetService(serviceType, key);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _provider.GetServices(serviceType);
		}
		#endregion
	}
}
