using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class RouteTokenValueProvider : HttpValueProvider
	{
		#region Constructors
		public RouteTokenValueProvider(ControllerContext context, CultureInfo culture)
			: base(context, culture)
		{
		}
		#endregion

		#region Instance Methods
		protected override IValueSet GetDataSource(ControllerContext context)
		{
			return context.RouteData.Tokens;
		}
		#endregion
	}

	public sealed class RouteTokenValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public RouteTokenValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new RouteTokenValueProvider(context, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}