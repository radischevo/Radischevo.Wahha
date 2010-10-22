using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class DefaultViewActivator : IViewActivator
	{
		#region Constructors
		public DefaultViewActivator()
		{
		}
		#endregion

		#region Instance Methods
		public object Create(ControllerContext context, Type type)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(type, () => Error.ArgumentNull("type"));

			return ServiceLocator.Instance.GetService(type);
		}
		#endregion
	}
}
