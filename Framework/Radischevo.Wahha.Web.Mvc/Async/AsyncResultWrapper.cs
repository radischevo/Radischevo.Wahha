using System;
using System.Threading;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Async;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	internal static class AsyncResultWrapper
	{
		#region Nested Types
		private sealed class WrappedAsyncResult<TResult> : IAsyncResult
		{
			#region Static Fields
			private static readonly object _timerDisabled = new object();
			#endregion

			#region Instance Fields
			private readonly SingleEntryGate _endExecutedGate;
			private readonly SingleEntryGate _handleCallbackGate;
			private readonly object _delegateLock;
			private readonly object _tag;
			private volatile bool _timedOut;
			private object _timer;
			private readonly BeginInvokeDelegate _beginDelegate;
			private readonly EndInvokeDelegate<TResult> _endDelegate;
			private IAsyncResult _innerAsyncResult;
			private AsyncCallback _originalCallback;
			#endregion

			#region Constructors
			public WrappedAsyncResult(BeginInvokeDelegate beginDelegate,
				EndInvokeDelegate<TResult> endDelegate, object tag)
			{
				_endExecutedGate = new SingleEntryGate();
				_handleCallbackGate = new SingleEntryGate();
				_delegateLock = new object();

				_beginDelegate = beginDelegate;
				_endDelegate = endDelegate;
				_tag = tag;
			}
			#endregion

			#region Instance Properties
			public object AsyncState
			{
				get
				{
					return _innerAsyncResult.AsyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return _innerAsyncResult.AsyncWaitHandle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return _innerAsyncResult.CompletedSynchronously;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return _innerAsyncResult.IsCompleted;
				}
			}
			#endregion

			#region Instance Methods
			private void ExecuteAsynchronousCallback(bool timedOut)
			{
				DestroyTimer();

				if (_handleCallbackGate.TryEnter())
				{
					_timedOut = timedOut;

					if (_originalCallback != null)
						_originalCallback(this);
				}
			}

			private void HandleAsynchronousCompletion(IAsyncResult asyncResult)
			{
				if (asyncResult.CompletedSynchronously)
					return;

				ExecuteAsynchronousCallback(false);
			}

			private void HandleTimeout(object state)
			{
				ExecuteAsynchronousCallback(true);
			}

			private void CreateTimer(int timeout)
			{
				Timer newTimer = new Timer(HandleTimeout,
					null, timeout, Timeout.Infinite);

				if (Interlocked.CompareExchange(ref _timer, newTimer, null) != null)
					newTimer.Dispose();
			}

			private void DestroyTimer()
			{
				Timer oldTimer = (Interlocked.Exchange(ref _timer, _timerDisabled) as Timer);
				if (oldTimer != null)
					oldTimer.Dispose();
			}

			public void Begin(AsyncCallback callback, object state, int timeout)
			{
				_originalCallback = callback;
				lock (_delegateLock)
				{
					_innerAsyncResult =
						_beginDelegate(HandleAsynchronousCompletion, state);
				}
				if (_innerAsyncResult.CompletedSynchronously)
				{
					if (callback != null)
						callback(this);
				}
				else
				{
					if (timeout > Timeout.Infinite)
						CreateTimer(timeout);
				}
			}

			public static WrappedAsyncResult<TResult> Cast(IAsyncResult result, object tag)
			{
				Precondition.Require(result, Error.ArgumentNull("result"));

				WrappedAsyncResult<TResult> castResult = (result as WrappedAsyncResult<TResult>);
				if (castResult != null && Object.Equals(castResult._tag, tag))
					return castResult;
				
				throw Error.InvalidAsyncResult("result");	
			}

			public TResult End()
			{
				if (!_endExecutedGate.TryEnter())
					throw Error.AsyncResultAlreadyConsumed();

				if (_timedOut)
					throw Error.AsyncTimeout();
				
				DestroyTimer();

				lock (_delegateLock)
				{
				}
				return _endDelegate(_innerAsyncResult);
			}
			#endregion
		}
		#endregion

		#region Helper Methods
		private static Func<AsyncVoid> MakeVoidDelegate(Action action)
		{
			return () => {
				action();
				return default(AsyncVoid);
			};
		}

		private static EndInvokeDelegate<AsyncVoid> MakeVoidDelegate(EndInvokeDelegate endDelegate)
		{
			return ar => {
				endDelegate(ar);
				return default(AsyncVoid);
			};
		}
		#endregion

		#region Begin Methods
		public static IAsyncResult Begin<TResult>(AsyncCallback callback, object state, 
			BeginInvokeDelegate beginDelegate, EndInvokeDelegate<TResult> endDelegate)
		{
			return Begin<TResult>(callback, state, beginDelegate, endDelegate, null);
		}

		public static IAsyncResult Begin<TResult>(AsyncCallback callback, object state, 
			BeginInvokeDelegate beginDelegate, EndInvokeDelegate<TResult> endDelegate, object tag)
		{
			return Begin<TResult>(callback, state, beginDelegate, endDelegate, tag, Timeout.Infinite);
		}

		public static IAsyncResult Begin<TResult>(AsyncCallback callback, object state, 
			BeginInvokeDelegate beginDelegate, EndInvokeDelegate<TResult> endDelegate, 
			object tag, int timeout)
		{
			WrappedAsyncResult<TResult> result = new WrappedAsyncResult<TResult>(beginDelegate, endDelegate, tag);
			result.Begin(callback, state, timeout);

			return result;
		}

		public static IAsyncResult Begin(AsyncCallback callback, object state, 
			BeginInvokeDelegate beginDelegate, EndInvokeDelegate endDelegate)
		{
			return Begin(callback, state, beginDelegate, endDelegate, null);
		}

		public static IAsyncResult Begin(AsyncCallback callback, object state, 
			BeginInvokeDelegate beginDelegate, EndInvokeDelegate endDelegate, object tag)
		{
			return Begin(callback, state, beginDelegate, endDelegate, tag, Timeout.Infinite);
		}

		public static IAsyncResult Begin(AsyncCallback callback, object state, BeginInvokeDelegate beginDelegate, 
			EndInvokeDelegate endDelegate, object tag, int timeout)
		{
			return Begin<AsyncVoid>(callback, state, beginDelegate, 
				MakeVoidDelegate(endDelegate), tag, timeout);
		}
		#endregion

		#region Begin Synchronous Methods
		public static IAsyncResult BeginSynchronous<TResult>(AsyncCallback callback, 
			object state, Func<TResult> func)
		{
			return BeginSynchronous<TResult>(callback, state, func, null);
		}

		public static IAsyncResult BeginSynchronous(AsyncCallback callback, 
			object state, Action action)
		{
			return BeginSynchronous(callback, state, action, null);
		}

		public static IAsyncResult BeginSynchronous(AsyncCallback callback, 
			object state, Action action, object tag)
		{
			return BeginSynchronous<AsyncVoid>(callback, state, MakeVoidDelegate(action), tag);
		}

		public static IAsyncResult BeginSynchronous<TResult>(AsyncCallback callback, 
			object state, Func<TResult> func, object tag)
		{
			BeginInvokeDelegate beginDelegate = (asyncCallback, asyncState) => {
				MvcAsyncResult inner = new MvcAsyncResult(asyncState);
				inner.MarkCompleted(true, asyncCallback);

				return inner;
			};

			EndInvokeDelegate<TResult> endDelegate = _ => {
				return func();
			};

			WrappedAsyncResult<TResult> result = 
				new WrappedAsyncResult<TResult>(beginDelegate, endDelegate, tag);
			result.Begin(callback, state, Timeout.Infinite);

			return result;
		}
		#endregion

		#region End Methods
		public static TResult End<TResult>(IAsyncResult result)
		{
			return End<TResult>(result, null);
		}

		public static TResult End<TResult>(IAsyncResult result, object tag)
		{
			return WrappedAsyncResult<TResult>.Cast(result, tag).End();
		}

		public static void End(IAsyncResult result)
		{
			End(result, null);
		}

		public static void End(IAsyncResult result, object tag)
		{
			End<AsyncVoid>(result, tag);
		}
		#endregion
	}
}
