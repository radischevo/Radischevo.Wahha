using System;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ArrayModelBinderSelector : ModelBinderSelector
	{
		#region Constructors
		public ArrayModelBinderSelector()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public override IModelBinder GetBinder(Type modelType)
		{
			if (modelType.IsArray)
				return (IModelBinder)Activator.CreateInstance(
					typeof(ArrayModelBinder<>).MakeGenericType(
						modelType.GetElementType()));
			
			return null;
		}
		#endregion
	}
}
