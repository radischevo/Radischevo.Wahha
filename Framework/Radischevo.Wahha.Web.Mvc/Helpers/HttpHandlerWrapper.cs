using System;
using System.Web;
using System.Web.UI;

namespace Radischevo.Wahha.Web.Mvc
{
	internal static class HttpHandlerWrapper
	{
		#region Nested Types
		internal class HttpSyncHandlerWrapper : Page
		{
			#region Instance Fields
			private readonly IHttpHandler _handler;
			#endregion

			#region Constructors
			public HttpSyncHandlerWrapper(IHttpHandler handler)
			{
				_handler = handler;
			}
			#endregion

			#region Instance Properties
			public IHttpHandler Handler
			{
				get
				{
					return _handler;
				}
			}
			#endregion

			#region Static Methods
			protected static void Wrap(Action action)
			{
				Wrap(delegate {
					action();
					return (object)null;
				});
			}

			protected static TResult Wrap<TResult>(Func<TResult> func)
			{
				try
				{
					return func();
				}
				catch (HttpException ex)
				{
					if (ex.GetHttpCode() == 500)
						throw;

					throw Mvc.Error.ChildRequestExecutionError(ex);
				}
			}
			#endregion

			#region Instance Methods
			public override void ProcessRequest(HttpContext context)
			{
				Wrap(() => _handler.ProcessRequest(context));
			}
			#endregion
		}

		private sealed class HttpAsyncHandlerWrapper : HttpSyncHandlerWrapper, IHttpAsyncHandler
		{
			#region Instance Fields
			private readonly IHttpAsyncHandler _handler;
			#endregion

			#region Constructors
			public HttpAsyncHandlerWrapper(IHttpAsyncHandler handler)
				: base(handler)
			{
				_handler = handler;
			}
			#endregion

			#region Instance Methods
			public IAsyncResult BeginProcessRequest(HttpContext context, 
				AsyncCallback callback, object extraData)
			{
				return Wrap(() => _handler.BeginProcessRequest(context, callback, extraData));
			}

			public void EndProcessRequest(IAsyncResult result)
			{
				Wrap(() => _handler.EndProcessRequest(result));
			}
			#endregion
		}
		#endregion

		#region Static Methods
		public static IHttpHandler Wrap(IHttpHandler handler)
		{
			IHttpAsyncHandler asyncHandler = (handler as IHttpAsyncHandler);
			return (asyncHandler != null) 
				? new HttpAsyncHandlerWrapper(asyncHandler) 
				: new HttpSyncHandlerWrapper(handler);
		}
		#endregion
	}
}
