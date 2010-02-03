using System;
using System.Threading;

namespace Radischevo.Wahha.Core.Async
{
	public sealed class TriggerListener
	{
		#region Instance Fields
		private readonly Trigger _activateTrigger;
		private volatile Action _continuation;
		private readonly SingleEntryGate _continuationGate;
		private readonly Trigger _setContinuationTrigger;
		private int _outstandingTriggers;
		#endregion

		#region Constructors
		public TriggerListener()
		{
			_continuationGate = new SingleEntryGate();
			_activateTrigger = CreateTrigger();
			_setContinuationTrigger = CreateTrigger();
		}
		#endregion

		#region Instance Methods
		private void HandleTriggerFired()
		{
			if (Interlocked.Decrement(ref _outstandingTriggers) == 0)
			{
				if (_continuationGate.TryEnter())
					_continuation();
			}
		}

		public void Activate()
		{
			_activateTrigger.Fire();
		}

		public void SetContinuation(Action continuation)
		{
			if (continuation != null)
			{
				_continuation = continuation;
				_setContinuationTrigger.Fire();
			}
		}

		public Trigger CreateTrigger()
		{
			Interlocked.Increment(ref _outstandingTriggers);
			SingleEntryGate triggerGate = new SingleEntryGate();

			return new Trigger(() => {
				if (triggerGate.TryEnter())
					HandleTriggerFired();
			});
		}
		#endregion
	}
}
