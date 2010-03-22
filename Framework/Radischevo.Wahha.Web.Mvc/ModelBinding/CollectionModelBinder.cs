using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class CollectionModelBinder<T> : ArrayModelBinder<T>
	{
		#region Constructors
		public CollectionModelBinder()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		protected override object ExecuteBind(BindingContext context)
		{
			ICollection<T> convertedValues = ReadCollection(context);
			ICollection<T> collection = (CreateModelInstance(context) as ICollection<T>);
			if (collection == null)
				return null;

			if (convertedValues.Count == 0)
				return collection;

			CollectionHelpers.CopyCollection<T>(collection, convertedValues);
			return collection;
		}
		#endregion	
	}
}
