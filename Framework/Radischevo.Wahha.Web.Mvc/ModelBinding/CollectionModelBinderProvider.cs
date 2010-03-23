using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
	public class CollectionModelBinderProvider : GenericModelBinderProvider
	{
		#region Constructors
		public CollectionModelBinderProvider()
			: base(typeof(ICollection<>), typeof(CollectionModelBinder<>))
		{
		}
		#endregion
	}
}
