using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	[Serializable]
	public class ValueProviderResult
	{
		#region Static Fields
		private static readonly CultureInfo _defaultCulture = CultureInfo.InvariantCulture;
		#endregion

		#region Instance Fields
		private object _value;
		private CultureInfo _culture;
		#endregion

		#region Constructors
		protected ValueProviderResult()
		{
		}

		public ValueProviderResult(object value, CultureInfo culture)
			: this()
		{
			_value = value;
			_culture = culture;				 
		}
		#endregion

		#region Instance Properties
		public object Value
		{
			get
			{
				return _value;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				if (_culture == null)
					_culture = _defaultCulture;

				return _culture;
			}
			set
			{
				_culture = value;
			}
		}
		#endregion

		#region Instance Methods
		public TValue GetValue<TValue>()
		{
			return GetValue<TValue>(default(TValue));
		}

		public virtual TValue GetValue<TValue>(TValue defaultValue)
		{
			return Converter.ChangeType<TValue>(_value, defaultValue, Culture);
		}
		#endregion
	}
}
