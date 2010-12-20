using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class PropertyBindingOrderAttribute : Attribute
	{
		#region Instance Fields
		private int _order;
		#endregion

		#region Constructors
		public PropertyBindingOrderAttribute(int order)
		{
			Precondition.Require(order > 0, () => 
				Error.ArgumentOutOfRange("order"));

			_order = order;
		}
		#endregion

		#region Instance Properties
		public int Order
		{
			get
			{
				return _order;
			}
		}
		#endregion
	}
}
