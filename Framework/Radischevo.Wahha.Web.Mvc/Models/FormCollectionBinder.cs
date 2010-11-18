using System;
using System.Collections.Generic;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class FormCollectionBinder : IModelBinder
    {
        #region Constructors
        public FormCollectionBinder()
        {   }
        #endregion

		#region Static Methods
		private static IValueSet BindDataSource(IValueProvider provider)
		{
			ValueDictionary values = new ValueDictionary();
			foreach (string key in provider.Keys)
			{
				ValueProviderResult result = provider.GetValue(key);
				if (result == null)
					continue;

				values.Add(key, result.Value);
			}
			return values;
		}
		#endregion

		#region Instance Methods
		public object Bind(BindingContext context)
        {
            // Заполняем FormCollection теми данными, 
            // что были использованы как источник в 
            // атрибуте BindAttribute.

            Precondition.Require(context, () => Error.ArgumentNull("context"));
			IValueSet source = BindDataSource(context.ValueProvider);

			return new FormCollection(source);
        }
        #endregion
    }
}
