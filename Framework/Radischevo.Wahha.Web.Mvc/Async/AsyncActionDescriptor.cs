using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public abstract class AsyncActionDescriptor : ActionDescriptor
	{
		#region Constructors
		protected AsyncActionDescriptor()
			: base()
		{
		}
		#endregion

		#region Static Methods
		internal static AsyncManager GetAsyncManager(ControllerBase controller)
		{
			IAsyncManagerContainer container = (controller as IAsyncManagerContainer);
			Precondition.Require(container, () => Error
				.ControllerMustImplementAsyncManagerContainer(controller.GetType()));

			return container.AsyncManager;
		}
		#endregion

		#region Instance Methods
		public abstract IAsyncResult BeginExecute(ControllerContext context, 
			IDictionary<string, object> parameters, 
			AsyncCallback callback, object state);

		public abstract object EndExecute(IAsyncResult result);

		public override object Execute(ControllerContext context, IDictionary<string, object> parameters)
		{
			IAsyncResult result = BeginExecute(context, parameters, null, null);
			AsyncTask.WaitForAsyncResultCompletion(result, context.Context.ApplicationInstance);

			return EndExecute(result);
		}
		#endregion
	}
}
