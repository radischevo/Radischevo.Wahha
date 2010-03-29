using System;
using System.Threading;

namespace Radischevo.Wahha.Core.Async
{
	public sealed class WaitFor<TResult>
	{
		#region Instance Fields
		private readonly TimeSpan _timeout;
		#endregion

		#region Constructors
		public WaitFor(TimeSpan timeout)
		{
			_timeout = timeout;
		}
		#endregion

		#region Static Methods
		public static TResult Run(TimeSpan timeout, Func<TResult> action)
		{
			return new WaitFor<TResult>(timeout).Run(action);
		}
		#endregion

		#region Instance Methods
		public TResult Run(Func<TResult> action)
		{
			Precondition.Require(action, () => Error.ArgumentNull("action"));

			var sync = new object();
			var isCompleted = false;

			WaitCallback watcher = obj => {
				Thread watchedThread = (obj as Thread);
				lock (sync)
				{
					if (!isCompleted)
						Monitor.Wait(sync, _timeout);
					
					if (!isCompleted)
						watchedThread.Abort();
				}
			};
			try
			{
				ThreadPool.QueueUserWorkItem(watcher, Thread.CurrentThread);
				return action();
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
				throw Error.OperationTimeout(_timeout);
			}
			finally
			{
				lock (sync)
				{
					isCompleted = true;
					Monitor.Pulse(sync);
				}
			}
		}
		#endregion
	}
}
