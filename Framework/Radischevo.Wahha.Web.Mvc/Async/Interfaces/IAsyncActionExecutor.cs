using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public interface IAsyncActionExecutor : IActionExecutor
	{
		IAsyncResult BeginInvokeAction(ControllerContext context, 
			string actionName, AsyncCallback callback, object state,
			IDictionary<string, object> values);

		bool EndInvokeAction(IAsyncResult result);
	}
}
