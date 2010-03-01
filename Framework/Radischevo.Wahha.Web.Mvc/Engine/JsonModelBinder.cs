using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Mvc
{
    public class JsonModelBinder : DefaultModelBinder
    {
        #region Nested Types
        private delegate object JavaScriptDeserializationExecutor(string value);
        #endregion

        #region Instance Methods
        public override object Bind(BindingContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            object value;
            context.TryGetValue(out value);

            return context.Model = Deserialize(context.ModelType, 
                Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        private object Deserialize(Type type, string serializedString)
        {
            if (String.IsNullOrEmpty(serializedString))
                return null;

            try
            {
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				return serializer.Deserialize(type, serializedString);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
