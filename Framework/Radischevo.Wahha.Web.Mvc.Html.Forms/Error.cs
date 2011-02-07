using System;
using System.Web;

using Res = Radischevo.Wahha.Web.Mvc.Html.Forms.Resources.Resources;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
    internal static class Error
    {
        internal static Exception ArgumentNull(string name)
        {
            return new ArgumentNullException(name);
        }

		internal static Exception ViewMasterPageRequiresViewPage()
		{
			return new InvalidOperationException(
				Res.Error_ViewMasterPageRequiresViewPage);
		}

		internal static Exception ViewControlRequiresViewPage()
		{
			return new InvalidOperationException(
				Res.Error_ViewControlRequiresViewPage);
		}

		internal static Exception WrongViewBase(Type type)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_WrongViewBase, type.Name));
		}

		internal static Exception ControlRequiresViewDataProvider()
		{
			return new InvalidOperationException(
				Res.Error_ControlRequiresViewDataProvider);
		}

		internal static Exception ChildRequestExecutionError(HttpException ex)
		{
			return new HttpException(500, Res.Error_ChildRequestExecutionError, ex);
		}
	}
}
