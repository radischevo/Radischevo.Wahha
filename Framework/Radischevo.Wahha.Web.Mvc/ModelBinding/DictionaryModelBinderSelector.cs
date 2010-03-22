using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class DictionaryModelBinderSelector : GenericModelBinderSelector
	{
		#region Constructors
		public DictionaryModelBinderSelector()
			: base(typeof(IDictionary<,>), typeof(DictionaryModelBinder<,>))
		{
		}
		#endregion
	}
}
