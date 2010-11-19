using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public static class ValueProviderExtensions
	{
		#region Extension Methods
		public static TValue GetValue<TValue>(this IValueProvider provider, string name)
		{
			return GetValue<TValue>(provider, name, default(TValue), CultureInfo.CurrentCulture);
		}

		public static TValue GetValue<TValue>(this IValueProvider provider,
			string name, TValue defaultValue)
		{
			return GetValue<TValue>(provider, name, defaultValue, CultureInfo.CurrentCulture);
		}

		public static TValue GetValue<TValue>(this IValueProvider provider,
			string name, TValue defaultValue, CultureInfo culture)
		{
			Precondition.Require(provider, () => Error.ArgumentNull("provider"));
			ValueProviderResult result = provider.GetValue(name);
			result.Culture = culture;

			return (result == null) ? defaultValue : result.GetValue<TValue>(defaultValue);
		}
		#endregion
	}
}