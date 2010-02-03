using System;
using System.Web;
using System.Web.SessionState;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Async;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class MvcHttpAsyncHandler
		: MvcHttpHandler, IHttpAsyncHandler
	{
		#region Nested Types
		private sealed class StubHttpHandler : UrlRoutingHandler
		{
			#region Instance Fields
			public IHttpHandler Handler;
			#endregion

			#region Constructors
			public StubHttpHandler()
			{
			}
			#endregion

			#region Instance Methods
			public void ProcessRequestInternal(HttpContextBase context)
			{
				ProcessRequest(context);
			}

			protected override void VerifyAndProcessRequest(
				IHttpHandler handler, HttpContextBase context)
			{
				Handler = handler;
			}
			#endregion
		}
		#endregion

		#region Static Fields
		private static readonly object _tag = new object();
		#endregion

		#region Constructors
		public MvcHttpAsyncHandler()
			: base()
		{
		}
		#endregion

		#region Static Methods
		private static IHttpHandler GetHttpHandler(HttpContextBase context)
		{
			StubHttpHandler handler = new StubHttpHandler();
			handler.ProcessRequestInternal(context);

			return handler.Handler;
		}
		#endregion

		#region Instance Methods
		protected virtual IAsyncResult BeginProcessRequest(
			HttpContextBase context, AsyncCallback callback, object state)
		{
			IHttpHandler handler = GetHttpHandler(context);
			IHttpAsyncHandler asyncHandler = (handler as IHttpAsyncHandler);

			if (asyncHandler == null)
			{
				Action action = delegate {
					handler.ProcessRequest(context.Unwrap());
				};
				return AsyncResultWrapper.BeginSynchronous(callback, state, action, _tag);
			}
			BeginInvokeDelegate beginDelegate = delegate(AsyncCallback asyncCallback, object asyncState) {
				return asyncHandler.BeginProcessRequest(context.Unwrap(), asyncCallback, asyncState);
			};
			EndInvokeDelegate endDelegate = delegate(IAsyncResult asyncResult) {
				asyncHandler.EndProcessRequest(asyncResult);
			};
			return AsyncResultWrapper.Begin(callback, state, beginDelegate, endDelegate, _tag);
		}

		protected virtual void EndProcessRequest(IAsyncResult asyncResult)
		{
			AsyncResultWrapper.End(asyncResult, _tag);
		}
		#endregion

		#region IHttpAsyncHandler Members
		private IAsyncResult BeginProcessRequestImpl(HttpContext context, AsyncCallback callback, object state)
		{
			return BeginProcessRequest(new HttpContextWrapper(context), callback, state);
		}

		IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
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