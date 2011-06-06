using System;

namespace Radischevo.Wahha.Web.Mvc.Html.Razor 
{
    /// <summary>
    /// Specifies properties of a template such as VirtualPath.
    /// </summary>
    public class TemplateFileInfo
	{
		#region Instance Fields
		private readonly string _virtualPath;
		#endregion

		#region Constructors
		public TemplateFileInfo(string virtualPath)
		{
			_virtualPath = virtualPath;
		}
		#endregion

		#region Instance Properties
		public string VirtualPath
		{
			get
			{
				return _virtualPath;
			}
		}
		#endregion
    }
}
