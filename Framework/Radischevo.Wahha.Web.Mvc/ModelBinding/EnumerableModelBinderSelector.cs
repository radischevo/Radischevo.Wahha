using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class EnumerableModelBinderSelector : GenericModelBinderSelector
	{
		#region Constructors
		public EnumerableModelBinderSelector()
			: base(typeof(IEnumerable<>), typeof(ArrayModelBinder<>))
		{
		}
		#endregion
	}
}
