using System;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class PropertyInfoExtensions
    {
        #region Nested Types
        private sealed class PropertyAccessorCache : ReaderWriterCache<PropertyInfo, PropertyAccessor>
        {
            #region Constructors
            public PropertyAccessorCache()
                : base()
            { }
            #endregion

            #region Instance Methods
            public PropertyAccessor GetAccessor(PropertyInfo property)
            {
                return base.GetOrCreate(property, () => {
                    return new PropertyAccessor(property);
                });
            }
            #endregion
        }
        #endregion

        #region Static Fields
        private static PropertyAccessorCache _cache = new PropertyAccessorCache();
        #endregion

        #region Static Extension Methods
        public static PropertyAccessor CreateAccessor(this PropertyInfo property)
        {
            Precondition.Require(property, Error.ArgumentNull("property"));
            return _cache.GetAccessor(property);
        }
        #endregion
    }
}
