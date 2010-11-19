using System;
using System.Collections.Generic;
using System.Globalization;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class ValueProviderBase : IValueProvider
	{
		#region Instance Fields
		private CultureInfo _culture;
		#endregion

		#region Constructors
		protected ValueProviderBase(CultureInfo culture)
		{
			_culture = culture;
		}
		#endregion

		#region Instance Properties
		protected CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		public abstract IEnumerable<string> Keys
		{
			get;
		}
		#endregion

		#region Static Methods
		public static IEnumerable<string> GetPrefixes(string key)
		{
			yield return key;

			for (int i = 0; i < key.Length; ++i)
			{
				if (key[i] == '-')
					yield return key.Substring(0, i);
			}
		}
		#endregion

		#region Instance Methods
		public abstract bool Contains(string prefix);

		public abstract ValueProviderResult GetValue(string key);
		#endregion
	}
}
