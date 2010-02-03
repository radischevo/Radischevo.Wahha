using System;
using System.Threading;
using System.Web;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	internal static class AsyncTask
	{
		#region Static Methods
		public static void WaitForAsyncResultCompletion(
			IAsyncResult result, HttpApplication application)
		{
			if (!result.IsCompleted)
			{
				bool needToRelock = false;
				try
				{
					try
					{
					}
					finally
					{
						Monitor.Exit(application);
						needToRelock = true;
					}

					WaitHandle waitHandle = result.AsyncWaitHandle;
					if (waitHandle != null)
					{
						waitHandle.WaitOne();
					}
					else
					{
						while (!result.IsCompleted)
							Thread.Sleep(1);
					}
				}
				finally
				{
					if (needToRelock)
						Monitor.Enter(application);
				}
			}
		}

		public static AsyncCallback WrapCallbackForSynchronizedExecution(
			AsyncCallback callback, SynchronizationContext context)
		{
			if (callback == null || context == null)
				return callback;

			return delegate(IAsyncResult result) {
				if (result.CompletedSynchronously)
					callback(result);
				else
					context.Sync(() => callback(result));
			};
		}
		#endregion
	}
}
