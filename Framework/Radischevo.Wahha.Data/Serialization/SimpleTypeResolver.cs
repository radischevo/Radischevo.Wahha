using System;

namespace Radischevo.Wahha.Data.Serialization
{
    public class SimpleTypeResolver : JavaScriptTypeResolver
    {
        #region Constructors
        public SimpleTypeResolver()
        {   }
        #endregion

        #region Instance Methods
        public override Type ResolveType(string typeId)
        {
            return Type.GetType(typeId, false, true);
        }

        public override string ResolveTypeId(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            return type.AssemblyQualifiedName;
        }
        #endregion
    }
}
