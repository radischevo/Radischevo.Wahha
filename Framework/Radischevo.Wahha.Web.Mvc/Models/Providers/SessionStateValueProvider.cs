using System;
using System.Globalization;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class SessionStateValueProvider : HttpValueProvider
	{
		#region Constructors
		public SessionStateValueProvider(ControllerContext context, CultureInfo culture)
			: base(context, culture)
		{
		}
		#endregion

		#region Instance Methods
		protected override IValueSet GetDataSource(ControllerContext context)
		{
			HttpSessionStateBase sessionState = context.Context.Session;

			if (sessionState == null)
				return new ValueDictionary();

			return sessionState.AsValueSet();
		}
		#endregion
	}

	public sealed class SessionStateValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public SessionStateValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new SessionStateValueProvider(context, CultureInfo.CurrentCulture);
		}
		#endregion
	}
}
