using System;
using System.Collections.Generic;
using System.Threading;

using Radischevo.Wahha.Core.Async;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class AsyncManager
	{
		#region Static Fields
		private static object _taskLock = new object();
		#endregion

		#region Instance Fields
		private readonly SynchronizationContext _context;
		private OperationCounter _outstandingOperations;
		private Dictionary<string, object> _parameters;
		private int _timeout = 45 * 1000;
		#endregion

		#region Events
		public event EventHandler Finished;
		#endregion

		#region Constructors
		public AsyncManager()
			: this(null)
		{
		}

		public AsyncManager(SynchronizationContext context)
		{
			_context = context ?? SynchronizationContextExtensions.GetSynchronizationContext();
			_parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			_outstandingOperations = new OperationCounter();
			_outstandingOperations.Completed += delegate {
				Finish();
			};
		}
		#endregion

		#region Instance Properties
		public OperationCounter OutstandingOperations
		{
			get
			{
				return _outstandingOperations;
			}
		}

		public IDictionary<string, object> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				if (value < -1)
					throw Error.InvalidTimeout("value");
				
				_timeout = value;
			}
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Signals that all operations are complete.
		/// </summary>
		public virtual void Finish()
		{
			EventHandler handler = Finished;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		/// <summary>
		/// Executes a callback in the current synchronization context, 
		/// which gives access to HttpContext and related items.
		/// </summary>
		/// <param name="action">The callback action.</param>
		public virtual void Sync(Action action)
		{
			_context.Sync(action);
		}

		public virtual void RegisterTask(Func<AsyncCallback, IAsyncResult> beginDelegate, 
			Action<IAsyncResult> endDelegate)
		{
			Precondition.Require(beginDelegate, () => Error.ArgumentNull("beginDelegate"));
			Precondition.Require(endDelegate, () => Error.ArgumentNull("endDelegate"));
			
			AsyncCallback callback = ar => {
				lock (_taskLock)
				{
				}
				if (!ar.CompletedSynchronously)
				{
					Sync(() => endDelegate(ar));
					OutstandingOperations.Decrement();
				}
			};

			OutstandingOperations.Increment();

			IAsyncResult asyncResult;
			lock (_taskLock)
			{
				asyncResult = beginDelegate(callback);
			}
			if (asyncResult.CompletedSynchronously)
			{
				endDelegate(asyncResult);
				OutstandingOperations.Decrement();
			}
		}
		#endregion
	}
}
