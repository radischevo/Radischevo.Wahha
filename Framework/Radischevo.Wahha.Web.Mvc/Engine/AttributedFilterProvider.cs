using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class AttributedFilterProvider : IFilterProvider
	{
		#region Instance Fields
		private bool _useCache;
		#endregion

		#region Constructors
		public AttributedFilterProvider()
			: this(true)
		{
		}

		public AttributedFilterProvider(bool useCache)
		{
			_useCache = useCache;
		}
		#endregion

		#region Instance Methods
		public virtual void Init(IValueSet settings)
		{
			_useCache = settings.GetValue<bool>("useCache", false);
		}

		protected virtual IEnumerable<FilterAttribute> GetActionAttributes(
			ControllerContext context, ActionDescriptor action)
		{
			return action.GetFilters(_useCache);
		}

		protected virtual IEnumerable<FilterAttribute> GetControllerAttributes(
			ControllerContext context, ActionDescriptor action)
		{
			return action.Controller.GetFilters(_useCache);
		}

		public virtual IEnumerable<Filter> GetFilters(ControllerContext context, ActionDescriptor action)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(action, () => Error.ArgumentNull("action"));

			ControllerBase controller = context.Controller;
			if (controller == null)
				return Enumerable.Empty<Filter>();

			return GetControllerAttributes(context, action)
				.Select(attr => new Filter(attr, FilterScope.Controller, null))
				.Concat(GetActionAttributes(context, action)
				.Select(attr => new Filter(attr, FilterScope.Action, null)))
				.ToList();
		}
		#endregion
	}
}
