using System;

namespace Radischevo.Wahha.Web.Mvc.Html.Razor
{
    public interface IVirtualPathFactory
	{
		#region Instance Methods
		bool Exists(string virtualPath);

		object CreateInstance(string virtualPath);
		#endregion
    }
}
