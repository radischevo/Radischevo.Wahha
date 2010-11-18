using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class HttpValueProvider : ValueProviderBase
	{
		#region Instance Fields
		private HashSet<string> _prefixes;
		private IValueSet _values;
		#endregion

		#region Constructors
		protected HttpValueProvider(ControllerContext context, CultureInfo culture)
			: base(culture)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));

			_prefixes = new HashSet<string>(
				StringComparer.OrdinalIgnoreCase);
			_values = GetDataSource(context);

			EnsureValues();
			Initialize();
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
		private void EnsureValues()
		{
			Precondition.Require(_values, () => Error.ArgumentNull("values"));
		}

		protected abstract IValueSet GetDataSource(ControllerContext context);

		protected virtual void Initialize()
		{
			if (!_values.Keys.Any())
				_prefixes.Add(String.Empty);

			foreach (string key in _values.Keys)
				_prefixes.UnionWith(GetPrefixes(key));
		}

		public override bool Contains(string prefix)
		{
			if (String.IsNullOrEmpty(prefix))
				return _values.Keys.Any();

			return _prefixes.Contains(prefix);
		}

		public override ValueProviderResult GetValue(string key)
		{
			if (_values.ContainsAny(key))
				return new ValueProviderResult(_values[key], Culture);

			return null;
		}
		#endregion
	}
}
