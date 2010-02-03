using System;
using System.Threading;

namespace Radischevo.Wahha.Core.Async
{
	/// <summary>
	/// This class is used to synchronize access 
	/// to a single-use consumable resource.
	/// </summary>
	public sealed class SingleEntryGate
	{
		#region Constants
		private const int NOT_ENTERED = 0;
		private const int ENTERED = 1;
		#endregion

		#region Instance Fields
		private int _status;
		#endregion

		#region Constructors
		public SingleEntryGate()
		{
		}
		#endregion

		#region Instance Methods
		public bool TryEnter()
		{
			int oldStatus = Interlocked.Exchange(ref _status, ENTERED);
			return (oldStatus == NOT_ENTERED);
		}
		#endregion
	}
}
