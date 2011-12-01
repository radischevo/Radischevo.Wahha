using System;

namespace Radischevo.Wahha.Web.Templates
{
    /// <summary>
    /// An interface that provides information about the current executing file.
    /// WebPageRenderingBase implements this type so that all pages excluding 
	/// AppStart pages could be queried to identify the current executing file.
    /// </summary>
    public interface ITemplateFile
	{
		#region Instance Properties
		TemplateFileInfo TemplateInfo
		{
			get;
		}
		#endregion
    }
}
