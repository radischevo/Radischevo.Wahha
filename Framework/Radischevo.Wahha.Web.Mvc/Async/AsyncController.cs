using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Async;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class AsyncController : Controller, IAsyncManagerContainer, IAsyncController
	{
		#region Static Fields
		private static readonly object _tag = new object();
		private static readonly object _executeTag = new object();
		#endregion

		#region Instance Fields
		private readonly AsyncManager _asyncManager;
		#endregion

		#region Constructors
		public AsyncController()
		{
			_asyncManager = new AsyncManager();
		}
		#endregion

		#region Instance Properties
		public AsyncManager AsyncManager
		{
			get
			{
				return _asyncManager;
			}
		}
		#endregion

		#region Instance Methods
		protected override IActionExecutor CreateActionExecutor()
		{
			return new AsyncActionExecutor(Context);
		}

		protected virtual IAsyncResult BeginExecute(RequestContext context, 
			AsyncCallback callback, object state)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));

			VerifyExecuteCalledOnce();
			Initialize(context);

			return AsyncResultWrapper.Begin(callback, state, 
				OnBeginExecute, OnEndExecute, _executeTag);
		}

		protected virtual void EndExecute(IAsyncResult result)
		{
			AsyncResultWrapper.End(result, _executeTag);
		}

		protected virtual IAsyncResult OnBeginExecute(AsyncCallback callback, object state)
		{
			LoadTempData();

			try
			{
				string actionName = RouteData.GetRequiredValue<string>("action");
				
				IActionExecutor executor = ActionExecutor;
				IAsyncActionExecutor asyncExecutor = (executor as IAsyncActionExecutor);

				if (asyncExecutor != null)
				{
					BeginInvokeDelegate beginDelegate = delegate(AsyncCallback asyncCallback, object asyncState) {
						return asyncExecutor.BeginInvokeAction(Context, actionName, asyncCallback, asyncState, new ValueDictionary());
					};
					EndInvokeDelegate endDelegate = delegate(IAsyncResult asyncResult) {
						if (!asyncExecutor.EndInvokeAction(asyncResult))
							HandleUnknownAction(actionName);
					};
					return AsyncResultWrapper.Begin(callback, state, beginDelegate, endDelegate, _tag);
				}
				else
				{
					Action action = () => {
						if (!executor.InvokeAction(Context, actionName, null))
							HandleUnknownAction(actionName);
					};
					return AsyncResultWrapper.BeginSynchronous(callback, state, action, _tag);
				}
			}
			catch
			{
				SaveTempData();
				throw;
			}
		}

		protected virtual void OnEndExecute(IAsyncResult result)
		{
			try
			{
				AsyncResultWrapper.End(result, _tag);
			}
			finally
			{
				SaveTempData();
			}
		}
		#endregion

		#region IAsyncController Members
		IAsyncResult IAsyncController.BeginExecute(
			RequestContext context, AsyncCallback callback, object state)
		{
			return BeginExecute(context, callback, state);
		}

		void IAsyncController.EndExecute(IAsyncResult result)
		{
			EndExecute(result);
		}
		#endregion
	}
}
