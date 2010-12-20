using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    public struct ParameterSource
	{
		#region Constants
		private const string ALLOW_ALL_TOKEN = "*";
		#endregion

		#region Instance Fields
		private HashSet<string> _sources;
		private bool _allowAll;
		#endregion

		#region Constructors
		private ParameterSource(bool allowAll)
		{
			_allowAll = allowAll;
			_sources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public ParameterSource(params string[] sources)
			: this(false)
		{
			_sources.UnionWith(sources);
		}
		#endregion

		#region Static Properties
		public static ParameterSource Any
		{
			get
			{
				return new ParameterSource(true);
			}
		}

		public static ParameterSource Default
		{
			get
			{
				return new ParameterSource(
					"Parameters", "Url", "Form", 
					"File", "QueryString"
				);
			}
		}
		#endregion

		#region Instance Properties
		public IEnumerable<string> Sources
		{
			get
			{
				return _sources;
			}
		}

		public bool AllowAll
		{
			get
			{
				return _allowAll;
			}
		}
		#endregion

		#region Static Methods
		public static ParameterSource FromString(string value)
		{
			if (String.IsNullOrEmpty(value))
				return ParameterSource.Default;

			if (String.Equals(value, ALLOW_ALL_TOKEN,
				StringComparison.OrdinalIgnoreCase))
				return ParameterSource.Any;

			return new ParameterSource(value.Split(new char[] { ',' }, 
				StringSplitOptions.RemoveEmptyEntries));
		}
		#endregion

		#region Instance Methods
		public bool Contains(string name)
		{
			return _sources.Contains(name);
		}
		#endregion
	}
}
