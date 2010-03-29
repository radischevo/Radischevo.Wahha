using System;
using System.Threading;

namespace Radischevo.Wahha.Core.Async
{
	public struct TimedLock : IDisposable
	{
		#region Nested Types
		#if DEBUG
		// (In Debug mode, we make it a class so that we can add a finalizer
		// in order to detect when the object is not freed.)
		private class Sentinel
		{
			~Sentinel()
			{
				// If this finalizer runs, someone somewhere failed to
				// call Dispose, which means we've failed to leave
				// a monitor!
				System.Diagnostics.Debug.Fail("Undisposed lock");
			}
		}
		#endif
		#endregion

		#region Instance Fields
		private object _target;
		#if DEBUG
		private Sentinel _leakDetector;
		#endif
		#endregion

		#region Constructors
		private TimedLock(object o)
		{
			_target = o;
			#if DEBUG
			_leakDetector = new Sentinel();
			#endif
		}
		#endregion

		#region Static Methods
		public static TimedLock Lock(object obj)
		{
			return Lock(obj, TimeSpan.FromSeconds(10));
		}

		public static TimedLock Lock(object obj, TimeSpan timeout)
		{
			TimedLock timedLock = new TimedLock(obj);
			if (!Monitor.TryEnter(obj, timeout))
			{
				#if DEBUG
				GC.SuppressFinalize(tl._leakDetector);
				#endif
				throw Error.OperationTimeout(timeout);
			}
			return timedLock;
		}
		#endregion

		#region Instance Methods
		public void Dispose()
		{
			Monitor.Exit(_target);
			// It's a bad error if someone forgets to call Dispose,
			// so in Debug builds, we put a finalizer in to detect
			// the error. If Dispose is called, we suppress the
			// finalizer.
			#if DEBUG
			GC.SuppressFinalize(_leakDetector);
			#endif
		}
		#endregion
	}
}
