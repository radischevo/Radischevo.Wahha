using System;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
	public interface IControllerActivator
	{
		#region Instance Methods
		IController Create(RequestContext context, Type type);
		#endregion
	}
}
