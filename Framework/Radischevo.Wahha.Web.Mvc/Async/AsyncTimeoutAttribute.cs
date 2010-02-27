using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
		Inherited = true, AllowMultiple = false)]
	public class AsyncTimeoutAttribute : FilterAttribute, IActionFilter
	{
		#region Instance Fields
		private int _duration;
		#endregion

		#region Constructors
		public AsyncTimeoutAttribute(int duration)
		{
			Precondition.Require(duration > -2, () => Error.InvalidTimeout("duration"));
			_duration = duration;
		}
		#endregion

		#region Instance Properties
		public int Duration
		{
			get
			{
				return _duration;
			}
		}
		#endregion

		#region Instance Methods
		public void OnExecuting(ActionExecutionContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));

			IAsyncManagerContainer container = (context.Controller as IAsyncManagerContainer);
			Precondition.Require(container, () => Error.ControllerMustImplementAsyncManagerContainer(context.Controller.GetType()));
			
			container.AsyncManager.Timeout = Duration;
		}
		#endregion

		#region IActionFilter Members
		void IActionFilter.OnExecuted(ActionExecutedContext context)
		{
		}
		#endregion
	}
}
