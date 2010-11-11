using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class ModelMapperBase<TModel> : IModelBinder
	{
		#region Constructors
		protected ModelMapperBase()
		{
		}
		#endregion

		#region Instance Methods
		public object Bind(BindingContext context)
		{
			TModel model = Bind(ExtractValue(context));
			if (Validate(context, model))
				return model;

			return null;
		}

		protected abstract TModel Bind(object value);

		protected virtual bool Validate(BindingContext context, TModel model)
		{
			return true;
		}

		protected virtual object ExtractValue(BindingContext context)
		{
			object value;
			context.TryGetValue(out value);

			return value;
		}
		#endregion
	}
}
