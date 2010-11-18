using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class ParameterValueProvider : HttpValueProvider
	{
		#region Constructors
		public ParameterValueProvider(ControllerContext context, CultureInfo culture)
			: base(context, culture)
		{
		}
		#endregion

		#region Instance Methods
		protected override IValueSet GetDataSource(ControllerContext context)
		{
			return context.Parameters;
		}
		#endregion
	}

	public sealed class ParameterValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public ParameterValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new ParameterValueProvider(context, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}
