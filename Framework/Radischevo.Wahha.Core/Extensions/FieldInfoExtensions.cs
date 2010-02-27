using System;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class FieldInfoExtensions
    {
        #region Nested Types
        private sealed class FieldAccessorCache : ReaderWriterCache<FieldInfo, FieldAccessor>
        {
            #region Constructors
            public FieldAccessorCache()
                : base()
            { }
            #endregion

            #region Instance Methods
            public FieldAccessor GetAccessor(FieldInfo field)
            {
                return base.GetOrCreate(field, () => {
                    return new FieldAccessor(field);
                });
            }
            #endregion
        }
        #endregion

        #region Static Fields
        private static FieldAccessorCache _cache = new FieldAccessorCache();
        #endregion

        #region Static Extension Methods
        public static FieldAccessor CreateAccessor(this FieldInfo field)
        {
			Precondition.Require(field, () => Error.ArgumentNull("field"));
            return _cache.GetAccessor(field);
        }
        #endregion
    }
}
