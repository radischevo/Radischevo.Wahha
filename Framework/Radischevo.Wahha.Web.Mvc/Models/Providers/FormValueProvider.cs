using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class FormValueProvider : HttpValueProvider
	{
		#region Constructors
		public FormValueProvider(ControllerContext context, CultureInfo culture)
			: base(context, culture)
		{
		}
		#endregion

		#region Instance Methods
		protected override IValueSet GetDataSource(ControllerContext context)
		{
			return context.Context.Request.Parameters.Form;
		}
		#endregion
	}

	public sealed class FormValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public FormValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new FormValueProvider(context, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}