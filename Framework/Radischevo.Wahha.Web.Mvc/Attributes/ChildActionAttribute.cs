using System;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
		AllowMultiple = false, Inherited = true)]
	public sealed class ChildActionAttribute : ActionSelectorAttribute
	{
		#region Constructors
		public ChildActionAttribute()
		{
		}
		#endregion

		#region Instance Methods
		public override bool IsValid(ControllerContext context, 
			MethodInfo actionMethod)
		{
			Precondition.Require(context, Error.ArgumentNull("context"));
			return context.IsChild;
		}
		#endregion
	}
}
