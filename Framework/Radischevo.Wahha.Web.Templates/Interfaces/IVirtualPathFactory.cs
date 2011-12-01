using System;

namespace Radischevo.Wahha.Web.Templates
{
    public interface IVirtualPathFactory
	{
		#region Instance Methods
		bool Exists(string virtualPath);

		object CreateInstance(string virtualPath);
		#endregion
    }
}
