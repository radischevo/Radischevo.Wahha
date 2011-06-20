using System;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web
{
	public class HttpContainerModule<TModule> : IHttpModule 
		where TModule : IHttpModule
	{
		#region Instance Fields
		private readonly Link<IHttpModule> _module;
		#endregion

		#region Constructors
		public HttpContainerModule()
		{
			_module = new Link<IHttpModule>(RetrieveModule);
		}
		#endregion

		#region Static Methods
		private static IHttpModule RetrieveModule()
		{
			return ServiceLocator.Instance.GetService<IHttpModule>();
		}
		#endregion

		#region Instance Methods
		public void Init(HttpApplication context)
		{
			IHttpModule module = _module.Value;
			if (module != null)
				module.Init(context);
		}

		public void Dispose()
		{
			IHttpModule module = _module.Value;
			if (module != null)
				module.Dispose();
		}
		#endregion
	}
}
