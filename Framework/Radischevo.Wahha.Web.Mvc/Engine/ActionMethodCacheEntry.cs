using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	internal sealed class ActionMethodCacheEntry
	{
		#region Instance Fields
		private MethodInfo _method;
		private List<ActionSelectorAttribute> _selectors;
		#endregion

		#region Constructors
		public ActionMethodCacheEntry(MethodInfo method)
		{
			Precondition.Require(method, Error.ArgumentNull("method"));
			_method = method;
			_selectors = GetActionSelectors(method);
		}
		#endregion

		#region Static Methods
		private static List<ActionSelectorAttribute> GetActionSelectors(MethodInfo method)
		{
			Precondition.Require(method, Error.ArgumentNull("method"));
			return ((ActionSelectorAttribute[])method.GetCustomAttributes(typeof(ActionSelectorAttribute), true)).ToList();
		}
		#endregion

		#region Instance Properties
		public MethodInfo Method
		{
			get
			{
				return _method;
			}
		}
		#endregion

		#region Instance Methods
		public bool IsValidFor(ControllerContext context)
		{
			return _selectors.All(selector => selector.IsValid(context, _method));
		}
		#endregion
	}
}
