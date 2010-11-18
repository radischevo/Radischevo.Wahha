using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class QueryStringValueProvider : HttpValueProvider
	{
		#region Constructors
		public QueryStringValueProvider(ControllerContext context, CultureInfo culture)
			: base(context, culture)
		{
		}
		#endregion

		#region Instance Methods
		protected override IValueSet GetDataSource(ControllerContext context)
		{
			return context.Context.Request.Parameters.QueryString;
		}
		#endregion
	}

	public sealed class QueryStringValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public QueryStringValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new QueryStringValueProvider(context, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}