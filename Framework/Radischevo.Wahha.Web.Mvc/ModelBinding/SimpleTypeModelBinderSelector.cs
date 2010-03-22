using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class SimpleTypeModelBinderSelector : ModelBinderSelector
	{
		#region Constructors
		public SimpleTypeModelBinderSelector()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public override IModelBinder GetBinder(Type modelType)
		{
			if (modelType.IsSimple() || modelType.IsEnum)
				return new SimpleTypeModelBinder();

			return null;
		}
		#endregion
	}
}
