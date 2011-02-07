using System;

namespace Radischevo.Wahha.Web.Mvc
{
	public interface IMvcFilter
	{
		#region Instance Properties
		bool AllowMultiple
		{
			get;
		}

		int Order
		{
			get;
		}
		#endregion
	}
}
