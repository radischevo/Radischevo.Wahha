using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ModelBinderRouter : IModelBinder
	{
		#region Instance Fields
		private List<ModelBinderSelector> _selectors;
		#endregion

		#region Constructors
		public ModelBinderRouter()
		{
			_selectors = new List<ModelBinderSelector>();
			Initialize();
		}
		#endregion

		#region Instance Properties
		public ICollection<ModelBinderSelector> Selectors
		{
			get
			{
				return _selectors;
			}
		}
		#endregion

		#region Instance Methods
		protected virtual void Initialize()
		{
			Selectors.Add(new SimpleTypeModelBinderSelector() { Priority = 5 });
			Selectors.Add(new ArrayModelBinderSelector() { Priority = 4 });
			Selectors.Add(new DictionaryModelBinderSelector() { Priority = 3 });
			Selectors.Add(new CollectionModelBinderSelector() { Priority = 2 });
			Selectors.Add(new EnumerableModelBinderSelector() { Priority = 1 });
		}

		public object Bind(BindingContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));

			Type modelType = context.ModelType;
			if (modelType == null)
				return null;

			IModelBinder binder = SelectModelBinder(modelType);
			return binder.Bind(context);
		}

		protected virtual IModelBinder SelectModelBinder(Type modelType)
		{
			IModelBinder binder = Selectors.OrderByDescending(s => s.Priority)
				.Select(s => s.GetBinder(modelType))
				.Where(b => b != null).FirstOrDefault();

			return binder ?? new ComplexTypeModelBinder();
		}
		#endregion
	}
}
