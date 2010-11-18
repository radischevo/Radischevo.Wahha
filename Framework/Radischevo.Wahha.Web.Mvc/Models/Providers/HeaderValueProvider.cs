using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class HeaderValueProvider : HttpValueProvider
	{
		#region Constructors
		public HeaderValueProvider(ControllerContext context, CultureInfo culture)
			: base(context, culture)
		{
		}
		#endregion

		#region Instance Methods
		protected override IValueSet GetDataSource(ControllerContext context)
		{
			return context.Context.Request.Parameters.Headers;
		}
		#endregion
	}

	public sealed class HeaderValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public HeaderValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new HeaderValueProvider(context, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}