using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class TypeMatchModelBinderProvider : ModelBinderProvider
	{
		#region Instance Fields
		private Type _type;
		private IModelBinder _binder;
		#endregion

		#region Constructors
		public TypeMatchModelBinderProvider(Type type, IModelBinder binder)
			: base()
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			Precondition.Require(binder, () => Error.ArgumentNull("binder"));

			_type = type;
			_binder = binder;
		}
		#endregion

		#region Instance Methods
		public override IModelBinder GetBinder(Type modelType)
		{
			Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));

			if (_type.IsAssignableFrom(modelType))
				return _binder;

			return null;
		}
		#endregion
	}
}
