using System;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, 
		AllowMultiple = false)]
	public sealed class FieldOrderAttribute : Attribute
	{
		#region Instance Fields
		private int _order;
		#endregion

		#region Constructors
		public FieldOrderAttribute(int order)
		{
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
			set
			{
				_order = value;
			}
		}
		#endregion
	}
}
