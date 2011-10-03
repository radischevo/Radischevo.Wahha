using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data.Serialization
{
    public abstract class JavaScriptConverter
    {
        #region Constructors
        protected JavaScriptConverter()
        {
        }
        #endregion

        #region Instance Properties
        public abstract IEnumerable<Type> SupportedTypes
        {
            get;
        }
        #endregion

        #region Instance Methods
        public abstract IDictionary<string, object> Serialize(object obj, 
            JavaScriptSerializer serializer);

        public abstract object Deserialize(IDictionary<string, object> dictionary, 
            Type type, JavaScriptSerializer serializer);
        #endregion
    }
}
