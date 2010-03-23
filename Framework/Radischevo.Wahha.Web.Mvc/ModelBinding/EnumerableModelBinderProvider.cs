using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class EnumerableModelBinderProvider : GenericModelBinderProvider
	{
		#region Constructors
		public EnumerableModelBinderProvider()
			: base(typeof(IEnumerable<>), typeof(ArrayModelBinder<>))
		{
		}
		#endregion
	}
}
