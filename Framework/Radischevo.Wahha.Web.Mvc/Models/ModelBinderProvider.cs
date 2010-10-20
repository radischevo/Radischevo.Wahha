using System;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class ModelBinderProvider
	{
		#region Instance Fields
		private int _order;
		#endregion

		#region Constructors
		protected ModelBinderProvider()
		{
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

		#region Instance Methods
		public abstract IModelBinder GetBinder(Type modelType);
		#endregion
	}
}
