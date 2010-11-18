using System;

namespace Radischevo.Wahha.Web.Mvc
{
	public interface IValueProviderFactory
	{
		#region Instance Methods
		IValueProvider Create(ControllerContext context);
		#endregion
	}
}
