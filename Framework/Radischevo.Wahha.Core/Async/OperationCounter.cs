using System;
using System.Threading;

namespace Radischevo.Wahha.Core.Async
{
	public sealed class OperationCounter
	{
		#region Instance Fields
		private int _count;
		public event EventHandler Completed;
		#endregion

		#region Constructors
		public OperationCounter()
		{
		}
		#endregion

		#region Instance Properties
		public int Count
		{
			get
			{
				return Thread.VolatileRead(ref _count);
			}
		}
		#endregion

		#region Instance Methods
		private void OnCompleted()
		{
			EventHandler handler = Completed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		private int AddAndExecuteCallbackIfCompleted(int value)
		{
			int count = Interlocked.Add(ref _count, value);
			if (count == 0)
				OnCompleted();

			return count;
		}

		public int Decrement()
		{
			return AddAndExecuteCallbackIfCompleted(-1);
		}

		public int Decrement(int value)
		{
			return AddAndExecuteCallbackIfCompleted(-value);
		}

		public int Increment()
		{
			return AddAndExecuteCallbackIfCompleted(1);
		}

		public int Increment(int value)
		{
			return AddAndExecuteCallbackIfCompleted(value);
		}
		#endregion
	}
}
