using System;
using System.Threading;
using System.Web;

using Radischevo.Wahha.Core.Async;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class MvcAsyncHandler 
		: MvcHandler, IHttpAsyncHandler
	{
		#region Static Fields
		private static readonly object _tag = new object();
		#endregion

		#region Constructors
		public MvcAsyncHandler(RequestContext context)
			: base(context)
		{
		}
		#endregion

		#region Instance Methods
		protected virtual IAsyncResult BeginProcessRequest(
			HttpContextBase context, AsyncCallback callback, object state)
		{
			AppendVersionHeader(context);
			string controllerName = Context.RouteData.GetRequiredValue<string>("controller");

			IControllerFactory factory = Builder.GetControllerFactory();
			IController controller = factory.CreateController(Context, controllerName);
			if (controller == null)
				throw Error.CouldNotCreateController(controllerName);			

			IAsyncController asyncController = (controller as IAsyncController);
			if (asyncController == null) // synchronous controller
			{
				Action action = delegate {
					try
					{
						controller.Execute(Context);
					}
					finally
					{
						factory.ReleaseController(controller);
					}
				};
				return AsyncResultWrapper.BeginSynchronous(callback, state, action, _tag);
			}

			// asynchronous controller
			BeginInvokeDelegate beginDelegate = delegate(AsyncCallback asyncCallback, object asyncState) {
				try
				{
					return asyncController.BeginExecute(Context, asyncCallback, asyncState);
				}
				finally
				{
					factory.ReleaseController(asyncController);
				}
			};
			EndInvokeDelegate endDelegate = delegate(IAsyncResult asyncResult) {
				try
				{
					asyncController.EndExecute(asyncResult);
				}
				finally
				{
					factory.ReleaseController(asyncController);
				}
			};
			return AsyncResultWrapper.Begin(AsyncTask.WrapCallbackForSynchronizedExecution(callback,
				SynchronizationContextExtensions.GetSynchronizationContext()), 
				state, beginDelegate, endDelegate, _tag);
		}

		protected virtual void EndProcessRequest(IAsyncResult result)
		{
			AsyncResultWrapper.End(result, _tag);
		}
		#endregion

		#region IHttpAsyncHandler Members
		private IAsyncResult BeginProcessRequestImpl(HttpContext context, 
			AsyncCallback callback, object state)
		{
			return BeginProcessRequest(new HttpContextWrapper(context), callback, state);
		}

		IAsyncResult IHttpAsyncHandler.BeginProcessRequest(
			HttpContext context, AsyncCallback cb, object extraData)
		{
			return BeginProcessRequestImpl(context, cb, extraData);
		}

		void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result)
		{
			EndProcessRequest(result);
		}
		#endregion
	}
}
