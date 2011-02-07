using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ControllerFilterProvider : IFilterProvider
	{
		#region Constructors
		public ControllerFilterProvider()
		{
		}
		#endregion

		#region Instance Methods
		public virtual void Init(IValueSet settings)
		{
		}

		public IEnumerable<Filter> GetFilters(ControllerContext context, ActionDescriptor action)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(action, () => Error.ArgumentNull("action"));

			if (context.Controller != null)
				yield return new Filter(context.Controller, FilterScope.First, Int32.MinValue);
		}
		#endregion
	}
}
