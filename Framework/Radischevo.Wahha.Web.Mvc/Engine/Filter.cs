using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class Filter
	{
		#region Constants
		public const int DefaultOrder = -1;
		#endregion

		#region Instance Fields
		private object _instance;
		private FilterScope _scope;
		private int _order;
		#endregion

		#region Constructors
		public Filter(object instance, FilterScope scope, int? order)
		{
			Precondition.Require(instance, () => Error.ArgumentNull("instance"));

			if (!order.HasValue)
			{
				IMvcFilter filter = (instance as IMvcFilter);
				order = (filter == null) ? DefaultOrder : filter.Order;
			}

			_instance = instance;
			_order = order.Value;
			_scope = scope;
		}
		#endregion

		#region Instance Properties
		public object Instance
		{
			get
			{
				return _instance;
			}
		}

		public int Order
		{
			get
			{
				return _order;
			}
		}

		public FilterScope Scope
		{
			get
			{
				return _scope;
			}
		}
		#endregion
	}
}
