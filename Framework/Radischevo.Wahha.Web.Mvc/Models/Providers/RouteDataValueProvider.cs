using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class RouteDataValueProvider : HttpValueProvider
	{
		#region Constructors
		public RouteDataValueProvider(ControllerContext context, CultureInfo culture)
			: base(context, culture)
		{
		}
		#endregion

		#region Instance Methods
		protected override IValueSet GetDataSource(ControllerContext context)
		{
			return context.RouteData.Values;
		}
		#endregion
	}

	public sealed class RouteDataValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public RouteDataValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new RouteDataValueProvider(context, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}