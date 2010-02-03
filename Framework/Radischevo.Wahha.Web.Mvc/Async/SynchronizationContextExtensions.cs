using System;
using System.Threading;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	internal static class SynchronizationContextExtensions
	{
		#region Static Methods
		public static SynchronizationContext GetSynchronizationContext()
		{
			return SynchronizationContext.Current ?? new SynchronizationContext();
		}
		#endregion

		#region Extension Methods
		public static T Sync<T>(this SynchronizationContext context, Func<T> func)
		{
			T value = default(T);
			Exception error = null;

			context.Send(o => {
				try
				{
					value = func();
				}
				catch (Exception ex)
				{
					error = ex;
				}
			}, null);

			if (error != null)
				throw Error.SynchronizationContextError(error);
			
			return value;
		}

		public static void Sync(this SynchronizationContext context, Action action)
		{
			Sync<AsyncVoid>(context, () => {
				action();
				return default(AsyncVoid);
			});
		}
		#endregion
	}
}
