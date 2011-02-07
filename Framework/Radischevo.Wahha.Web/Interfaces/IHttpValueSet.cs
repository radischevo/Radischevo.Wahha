using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web
{
	public interface IHttpValueSet : IValueSet
	{
		#region Instance Methods
		bool ContainsKey(string key);

		IEnumerable<TValue> GetValues<TValue>(string key);
		#endregion
	}
}
