using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class XmlModelBinder : ModelBinderBase
    {
        #region Instance Methods
        protected override object ExecuteBind(BindingContext context)
        {
            object value;
            if (!context.TryGetValue(out value))
                return null;

            using (TextReader reader = new StringReader(
                Convert.ToString(value, CultureInfo.InvariantCulture)))
            {
                XmlSerializer serializer = new XmlSerializer(context.ModelType);
                return context.Model = serializer.Deserialize(reader);
            }
        }
        #endregion
    }
}
