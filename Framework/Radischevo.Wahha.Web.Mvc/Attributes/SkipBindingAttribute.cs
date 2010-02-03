using System;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface |
        AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class,
        AllowMultiple = false, Inherited = false)]
    public sealed class SkipBindingAttribute : ModelBinderAttribute
    {
        #region Nested Types
        private sealed class NullModelBinder : IModelBinder
        {
            #region Constructors
            public NullModelBinder()
            {   }
            #endregion

            #region Instance Methods
            public object Bind(BindingContext context)
            {
                return null;
            }
            #endregion
        }
        #endregion

        #region Static Fields
        private static readonly IModelBinder _nullBinder = new NullModelBinder();
        #endregion

        #region Constructors
        public SkipBindingAttribute()
            : base(typeof(NullModelBinder))
        {
        }
        #endregion

        #region Instance Methods
        public override IModelBinder GetBinder()
        {
            return _nullBinder;
        }
        #endregion
    }
}
