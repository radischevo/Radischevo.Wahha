using System;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class ModelBinderSelector
	{
		#region Instance Fields
		private int _priority;
		#endregion

		#region Constructors
		protected ModelBinderSelector()
		{
		}
		#endregion

		#region Instance Properties
		public int Priority
		{
			get
			{
				return _priority;
			}
			set
			{
				_priority = value;
			}
		}
		#endregion

		#region Instance Methods
		public abstract IModelBinder GetBinder(Type modelType);
		#endregion
	}
}
