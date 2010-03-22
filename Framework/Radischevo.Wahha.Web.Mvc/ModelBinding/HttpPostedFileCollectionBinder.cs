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

        #region Static Methods
        private static List<HttpPostedFileBase> GetPostedFiles(HttpFileCollectionBase files, string key)
        {
            List<HttpPostedFileBase> list = files.AllKeys
                .Select((thisKey, index) => (String.Equals(thisKey, key, StringComparison.OrdinalIgnoreCase)) ? index : -1)
                .Where(index => index > -1).Select(index => files[index]).ToList();

            if (files.Count == 0)
            {
                for (int i = 0; ; ++i)
                {
                    HttpPostedFileBase file = files[String.Format(
                        CultureInfo.InvariantCulture, "{0}-{1}", key, i)];

                    if (file == null)
                        break;
                    
                    list.Add(file);
                }
            }

            return list.Where(f => f != null && f.ContentLength > 0 && 
                !String.IsNullOrEmpty(f.FileName)).ToList();
        }
        #endregion

        #region Instance Methods
        public object Bind(BindingContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            List<HttpPostedFileBase> files = GetPostedFiles(context.Context.Request.Files, context.ModelName);
            
            if (files.Count < 1)
                return null;

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
