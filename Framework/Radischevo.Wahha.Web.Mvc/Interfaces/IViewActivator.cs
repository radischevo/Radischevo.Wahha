using System;

namespace Radischevo.Wahha.Web.Mvc
{
	public interface IViewActivator
	{
		#region Instance Methods
		object Create(ControllerContext context, Type type);
		#endregion
	}
}
