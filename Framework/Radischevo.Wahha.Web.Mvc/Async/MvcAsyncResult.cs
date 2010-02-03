using System;
using System.Threading;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	internal sealed class MvcAsyncResult : IAsyncResult
	{
		#region Instance Fields
		private readonly object _state;
		private volatile bool _isCompleted;
		private bool _completedSynchronously;
		#endregion

		#region Constructors
		public MvcAsyncResult(object state)
		{
			_state = state;
		}
		#endregion

		#region Instance Properties
		public object AsyncState
		{
			get
			{
				return _state;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return null;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return _completedSynchronously;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return _isCompleted;
			}
		}
		#endregion

		#region Instance Methods
		public void MarkCompleted(bool completedSynchronously, 
			AsyncCallback callback)
		{
			_completedSynchronously = completedSynchronously;
			_isCompleted = true;

			if (callback != null)
				callback(this);
		}
		#endregion
	}
}
