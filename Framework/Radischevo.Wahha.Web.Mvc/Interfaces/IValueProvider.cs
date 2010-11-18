using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
	public interface IValueProvider
	{
		#region Instance Properties
		IEnumerable<string> Keys
		{
			get;
		}
		#endregion

		#region Instance Methods
		bool Contains(string prefix);

		ValueProviderResult GetValue(string key);
		#endregion
	}
}
