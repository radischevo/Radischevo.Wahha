using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Core
{
	public sealed class DelegatedServiceLocator : IServiceLocator
	{
		#region Instance Fields
		private Func<Type, object> _serviceLocator;
		private Func<Type, string, object> _namedServiceLocator;
		private Func<Type, IEnumerable<object>> _serviceSelector;
		#endregion

		#region Constructors
		public DelegatedServiceLocator(Func<Type, object> serviceLocator,
			Func<Type, IEnumerable<object>> serviceSelector)
			: this(serviceLocator, CreateNamedLocator(serviceLocator), serviceSelector)
		{
		}

		public DelegatedServiceLocator(Func<Type, object> serviceLocator, 
			Func<Type, string, object> namedServiceLocator, 
			Func<Type, IEnumerable<object>> serviceSelector)
		{
			Precondition.Require(serviceLocator, () => 
				Error.ArgumentNull("serviceLocator"));
			Precondition.Require(namedServiceLocator, () => 
				Error.ArgumentNull("namedServiceLocator"));
			Precondition.Require(serviceSelector, () => 
				Error.ArgumentNull("serviceSelector"));

			_serviceLocator = serviceLocator;
			_namedServiceLocator = namedServiceLocator;
			_serviceSelector = serviceSelector;
		}
		#endregion

		#region Static Methods
		private static Func<Type, string, object> CreateNamedLocator(Func<Type, object> locator)
		{
			Precondition.Require(locator, () => Error.ArgumentNull("locator"));
			return (type, name) => locator(type);
		}
		#endregion

		#region Instance Methods
		public object GetService(Type serviceType)
		{
			return _serviceLocator(serviceType);
		}

		public object GetService(Type serviceType, string key)
		{
			return _namedServiceLocator(serviceType, key);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _serviceSelector(serviceType);
		}
		#endregion
	}
}
