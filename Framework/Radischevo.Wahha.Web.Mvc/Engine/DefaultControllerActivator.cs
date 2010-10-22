using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class DefaultControllerActivator : IControllerActivator
	{
		#region Constructors
		public DefaultControllerActivator()
		{
		}
		#endregion

		#region Instance Methods
		public IController Create(RequestContext context, Type type)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(type, () => Error.ArgumentNull("type"));

			return (IController)ServiceLocator.Instance.GetService(type);
		}
		#endregion
	}
}
