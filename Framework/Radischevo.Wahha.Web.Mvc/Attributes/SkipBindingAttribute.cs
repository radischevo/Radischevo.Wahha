using System;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface | 
		AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Struct | 
		AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
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
				IValueProvider provider = new DictionaryValueProvider(context.Parameters);

				// Rewrite the provided context to ensure all the parameters 
				// provided by user is set correctly.
				BindingContext inner = new BindingContext(context, 
					context.ModelType, context.ModelName, provider, context.ModelState);

				ValueProviderResult value;
				if (inner.TryGetValue(out value))
					return value.Value;

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
