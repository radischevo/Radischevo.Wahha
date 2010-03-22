using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
	public class CollectionModelBinderSelector : GenericModelBinderSelector
	{
		#region Constructors
		public CollectionModelBinderSelector()
			: base(typeof(ICollection<>), typeof(CollectionModelBinder<>))
		{
		}
		#endregion
	}
}
