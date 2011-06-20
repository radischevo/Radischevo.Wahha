using System;
using System.Collections.Generic;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web
{
	public class HttpContainerModule : IHttpModule
	{
		#region Instance Fields
		private readonly EnumerableLink<IHttpModule> _modules;
		#endregion

		#region Constructors
		public HttpContainerModule()
		{
			_modules = new EnumerableLink<IHttpModule>(RetrieveModules);
		}
		#endregion

		#region Static Methods
		private static IEnumerable<IHttpModule> RetrieveModules()
		{
			return ServiceLocator.Instance.GetServices<IHttpModule>();
		}
		#endregion

		#region Instance Methods
		public void Init(HttpApplication context)
		{
			foreach (IHttpModule module in _modules)
				module.Init(context);
		}

		public void Dispose()
		{
			foreach (IHttpModule module in _modules)
				module.Dispose();
		}
		#endregion
	}
}
