using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class CookieValueProvider : HttpValueProvider
	{
		#region Constructors
		public CookieValueProvider(ControllerContext context, CultureInfo culture)
			: base(context, culture)
		{
		}
		#endregion

		#region Instance Methods
		protected override IValueSet GetDataSource(ControllerContext context)
		{
			return context.Context.Request.Parameters.Cookies;
		}
		#endregion
	}

	public sealed class CookieValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public CookieValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new CookieValueProvider(context, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}
