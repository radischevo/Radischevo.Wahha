using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class HttpPostedFileCollectionBinder : IModelBinder
    {
        #region Constructors
        public HttpPostedFileCollectionBinder()
        {   }
        #endregion

		#region Instance Methods
		public object Bind(BindingContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            ValueProviderResult result = context.ValueProvider.GetValue(context.ModelName);
			if (result == null)
				return null;

			List<HttpPostedFileBase> files = result
				.GetValue<IEnumerable<HttpPostedFileBase>>().ToList();

            Type type = context.ModelType;
            if (type == typeof(IEnumerable<HttpPostedFileBase>) || type == typeof(HttpPostedFileBase[]))
                return files.ToArray();

            if (type == typeof(ICollection<HttpPostedFileBase>) || type == typeof(Collection<HttpPostedFileBase>))
                return new Collection<HttpPostedFileBase>(files);

            if (type == typeof(IList<HttpPostedFileBase>) || type == typeof(List<HttpPostedFileBase>))
                return files;

            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(IEnumerable<HttpPostedFileBase>) });
            if (constructor == null)
                throw Error.UnsupportedModelType(type);

            return constructor.CreateInvoker().Invoke(files);
        }
        #endregion
    }
}
