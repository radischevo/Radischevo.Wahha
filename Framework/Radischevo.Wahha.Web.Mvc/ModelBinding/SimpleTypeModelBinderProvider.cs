using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class SimpleTypeModelBinderProvider : ModelBinderProvider
	{
		#region Constructors
		public SimpleTypeModelBinderProvider()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public override IModelBinder GetBinder(Type modelType)
		{
			Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));

			if (modelType.IsSimple() || modelType.IsEnum)
				return new SimpleTypeModelBinder();

			return null;
		}
		#endregion
	}
}
