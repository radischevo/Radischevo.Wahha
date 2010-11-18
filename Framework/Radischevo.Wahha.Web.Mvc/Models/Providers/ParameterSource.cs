using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    public struct ParameterSource
	{
		#region Constants
		public const string QueryString = "QueryString";
		public const string Form = "Form";
		public const string File = "File";
		public const string Url = "Url";
		public const string Token = "Token";
		public const string Cookie = "Cookie";
		public const string Header = "Header";
		public const string Session = "Session";
		public const string Parameters = "Parameters";
		public const string InputStream = "InputStream";
		#endregion
		
		#region Instance Fields
		private HashSet<string> _sources;
		#endregion

		#region Constructors
		public ParameterSource(params string[] sources)
		{
			_sources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			_sources.UnionWith(sources);
		}
		#endregion

		#region Static Properties
		public static ParameterSource Any
		{
			get
			{
				return new ParameterSource(
					ParameterSource.Parameters, 
					ParameterSource.Url,
					ParameterSource.Form,
					ParameterSource.File,
					ParameterSource.QueryString,
					ParameterSource.Token,
					ParameterSource.Session,
					ParameterSource.Cookie,
					ParameterSource.Header,
					ParameterSource.InputStream
				);
			}
		}

		public static ParameterSource Default
		{
			get
			{
				return new ParameterSource(
					ParameterSource.Parameters,
					ParameterSource.Url,
					ParameterSource.Form,
					ParameterSource.File,
					ParameterSource.QueryString
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
		#endregion

		#region Static Methods
		public static ParameterSource FromString(string value)
		{
			if (String.IsNullOrEmpty(value))
				return ParameterSource.Default;

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
