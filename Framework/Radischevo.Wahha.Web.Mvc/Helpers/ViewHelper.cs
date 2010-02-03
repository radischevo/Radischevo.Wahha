using System;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc.UI
{
	internal static class ViewHelper
	{
		#region Static Methods
		internal static string RenderControl(ViewContext context, UserControl control)
		{
			ViewPage page = new ViewPage();
			page.Controls.Add(control);

			return ViewHelper.RenderPage(context, page);
		}

		internal static string RenderPage(ViewContext context, Page page)
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb, CultureInfo.CurrentCulture))
			{
				context.Context.Server.Execute(page, sw, true);
			}
			return sb.ToString();
		}

		internal static ViewPage CreatePage(string virtualPath)
		{
			if (!virtualPath.StartsWith("~/"))
				virtualPath = String.Concat("~/", virtualPath);
			ViewPage page = null;

			try
			{
				Type type = BuildManager.GetCompiledType(virtualPath);
				page = (ViewPage)Activator.CreateInstance(type);
			}
			catch (Exception)
			{
				throw Error.CouldNotCreateView(virtualPath);
			}
			return page;
		}

		internal static ViewUserControl CreateControl(string virtualPath)
		{
			if (!virtualPath.StartsWith("~/"))
				virtualPath = String.Concat("~/", virtualPath);
			ViewUserControl control = null;

			try
			{
				Type ctype = BuildManager.GetCompiledType(virtualPath);
				control = (ViewUserControl)Activator.CreateInstance(ctype);
			}
			catch (Exception)
			{
				throw Error.UnableToInstantiateUserControl(virtualPath);
			}
			return control;
		}
		#endregion

		#region Page Rendering Methods
		internal static string RenderPage(string virtualPath, ViewContext context)
		{
			ViewPage page = CreatePage(virtualPath);
			return DoRender(page, context, null, null);
		}

		internal static string RenderPage(string virtualPath,
			ViewContext context, ViewDataDictionary viewData, object propertySettings)
		{
			ViewPage page = CreatePage(virtualPath);
			return DoRender(page, context, viewData, propertySettings);
		}

		internal static string RenderPage<TPage>(ViewContext context,
			ViewDataDictionary viewData, object propertySettings)
			where TPage : ViewPage
		{
			TPage page = Activator.CreateInstance<TPage>();
			return DoRender(page, context, viewData, propertySettings);
		}
		#endregion

		#region Control Rendering Methods
		internal static string RenderControl(string virtualPath, ViewContext context)
		{
			ViewUserControl ctl = CreateControl(virtualPath);
			return DoRender(ctl, context, null, null);
		}

		internal static string RenderControl(string virtualPath,
			ViewContext context, ViewDataDictionary controlData, object propertySettings)
		{
			ViewUserControl ctl = CreateControl(virtualPath);
			return DoRender(ctl, context, controlData, propertySettings);
		}

		internal static string RenderControl<TControl>(ViewContext context,
			ViewDataDictionary controlData, object propertySettings)
			where TControl : ViewUserControl
		{
			TControl ctl = Activator.CreateInstance<TControl>();
			return DoRender(ctl, context, controlData, propertySettings);
		}
		#endregion

		#region Private Helper Methods
		private static void SetInstanceProperties(object instance, object propertySettings)
		{
			try
			{
				if (propertySettings != null)
				{
					ValueDictionary values = new ValueDictionary(propertySettings);
					Type type = instance.GetType();

					foreach (string key in values.Keys)
					{
						PropertyInfo pi = type.GetProperty(key, BindingFlags.IgnoreCase |
							BindingFlags.Instance | BindingFlags.Public);

						if (pi != null)
							pi.CreateAccessor().SetValue(instance, values[key]);
					}
				}
			}
			catch (Exception ex)
			{
				throw Error.CouldNotSetControlProperties(instance, ex);
			}
		}

		private static string DoRender(ViewPage instance,
			ViewContext context, ViewDataDictionary viewData,
			object propertySettings)
		{
			instance.InitHelpers(context);
			SetInstanceProperties(instance, propertySettings);

			if (viewData == null)
				instance.ViewData = context.ViewData;
			else
				instance.ViewData = viewData;

			return ViewHelper.RenderPage(context, instance);
		}

		private static string DoRender(ViewUserControl instance,
			ViewContext context, ViewDataDictionary viewData,
			object propertySettings)
		{
			ViewPage page = new ViewPage();
			page.Controls.Add(instance);
			page.InitHelpers(context);

			SetInstanceProperties(instance, propertySettings);

			if (viewData == null)
				instance.ViewData = context.ViewData;
			else
				instance.ViewData = viewData;

			return ViewHelper.RenderPage(context, page);
		}
		#endregion
	}
}
