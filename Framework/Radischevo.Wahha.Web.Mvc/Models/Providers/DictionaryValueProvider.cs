using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class DictionaryValueProvider : ValueProviderBase
	{
		#region Instance Fields
		private HashSet<string> _prefixes;
		private ValueDictionary _values;
		#endregion

		#region Constructors
		public DictionaryValueProvider(IValueSet values)
			: this(values, CultureInfo.CurrentCulture)
		{
		}

		public DictionaryValueProvider(IValueSet values, CultureInfo culture)
			: base(culture)
		{
			_values = new ValueDictionary(values);
			_prefixes = new HashSet<string>();

			foreach (string key in _values.Keys)
				_prefixes.UnionWith(GetPrefixes(key));
		}
		#endregion

		#region Instance Properties
		public override IEnumerable<string> Keys
		{
			get
			{
				return _values.Keys;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Contains(string prefix)
		{
			if (String.IsNullOrEmpty(prefix))
				return _values.Any();

			return _prefixes.Contains(prefix);
		}

		public override ValueProviderResult GetValue(string key)
		{
			object value;
			if (_values.TryGetValue(key, out value))
				return new ValueProviderResult(value, Culture);

			return null;
		}
		#endregion
	}
}
