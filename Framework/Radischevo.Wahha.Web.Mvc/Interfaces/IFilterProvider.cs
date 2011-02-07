using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public interface IFilterProvider
	{
		#region Instance Methods
		void Init(IValueSet settings);

		IEnumerable<Filter> GetFilters(ControllerContext context, ActionDescriptor action);
		#endregion
	}
}
