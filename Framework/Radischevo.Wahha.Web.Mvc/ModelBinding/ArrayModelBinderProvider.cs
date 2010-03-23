using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ArrayModelBinderProivider : ModelBinderProvider
	{
		#region Constructors
		public ArrayModelBinderProivider()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public override IModelBinder GetBinder(Type modelType)
		{
			Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));

			if (modelType.IsArray)
				return (IModelBinder)Activator.CreateInstance(
					typeof(ArrayModelBinder<>).MakeGenericType(
						modelType.GetElementType()));
			
			return null;
		}
		#endregion
	}
}
