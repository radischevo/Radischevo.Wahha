using System;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
		Inherited = true, AllowMultiple = false)]
	public abstract class ActionFilterAttribute : FilterAttribute, IActionFilter, IResultFilter
	{
		#region Constructors
		protected ActionFilterAttribute()
		{
		}
		#endregion

		#region Instance Methods
		public virtual void OnExecuting(ActionExecutionContext context)
		{
		}

		public virtual void OnExecuted(ActionExecutedContext context)
		{
		}

		public virtual void OnResultExecuting(ResultExecutionContext context)
		{
		}

		public virtual void OnResultExecuted(ResultExecutedContext context)
		{
		}
		#endregion
	}
}
