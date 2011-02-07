using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web
{
	internal static class HttpValueCollectionConverter
	{
		#region Static Methods
		public static IEnumerable<TValue> Convert<TValue>(string value)
		{
			List<TValue> list = new List<TValue>();
			if (value == null)
				return list;

			string[] values = value.Split(new char[] { ',' },
				StringSplitOptions.RemoveEmptyEntries);

			foreach (string sv in values)
				list.Add(Converter.ChangeType<TValue>(sv, default(TValue)));

			return list;
		}
		#endregion
	}
}
