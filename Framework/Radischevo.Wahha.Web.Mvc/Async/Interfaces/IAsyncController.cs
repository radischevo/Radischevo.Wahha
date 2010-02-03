using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public interface IAsyncController : IController
	{
		IAsyncResult BeginExecute(RequestContext context, AsyncCallback callback, object state);

		void EndExecute(IAsyncResult result);
	}
}
