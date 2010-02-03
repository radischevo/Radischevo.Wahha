using System;
using System.Threading;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
		Inherited = true, AllowMultiple = false)]
	public sealed class NoAsyncTimeoutAttribute : AsyncTimeoutAttribute
	{
		#region Constructors
		public NoAsyncTimeoutAttribute()
			: base(Timeout.Infinite)
		{
		}
		#endregion
	}
}
