using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;
using Radischevo.Wahha.Data;
using Radischevo.Wahha.Core;

namespace ConsoleTester
{
	public class UnityDbDataProviderFactory : IDbDataProviderFactory
	{
		#region Instance Fields
		private IUnityContainer _container;
		#endregion

		#region Constructors
		public UnityDbDataProviderFactory(IUnityContainer container)
        {
			_container = container;
		}
        #endregion

		#region Instance Properties
		protected IUnityContainer Container
		{
			get
			{
				return _container;
			}
		}
		#endregion

        #region Instance Methods
        public IDbDataProvider CreateProvider(Type providerType)
        {
			return (IDbDataProvider)Container.Resolve(providerType);
        }

        public IDbDataProvider CreateProvider(string providerName)
        {
			return Container.Resolve<IDbDataProvider>(providerName);
        }

        public void DisposeProvider(IDbDataProvider provider)
        {
			Precondition.Require(provider,
				() => new ArgumentNullException("provider"));

            provider.Dispose();
        }
        #endregion
	}
}
