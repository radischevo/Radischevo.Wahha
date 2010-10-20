using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class DictionaryModelBinderProvider : GenericModelBinderProvider
	{
		#region Constructors
		public DictionaryModelBinderProvider()
			: base(typeof(IDictionary<,>), typeof(DictionaryModelBinder<,>))
		{
		}
		#endregion
	}
}
