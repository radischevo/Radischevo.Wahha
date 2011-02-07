using System;
using Radischevo.Wahha.Web.Mvc.Html;

namespace Radischevo.Wahha.Web.Mvc.Razor
{
	public interface IViewStartPageChild
	{
		#region Instance Properties
		HtmlHelper<object> Html
		{
			get;
		}
		
		UrlHelper Url
		{
			get;
		}

		ViewContext ViewContext
		{
			get;
		}
		#endregion
	}
}
