using System;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class |
        AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class FilterAttribute : Attribute, IMvcFilter
	{
		#region Nested Types
		private sealed class MultiUseAttributeCache : ReaderWriterCache<Type, bool>
		{
			#region Constructors
			public MultiUseAttributeCache()
				: base()
			{
			}
			#endregion

			#region Instance Methods
			public bool AllowMultiple(Type type)
			{
				return base.GetOrCreate(type, () => type
					.GetCustomAttributes<AttributeUsageAttribute>(true)
					.First().AllowMultiple);
			}
			#endregion
		}
		#endregion

		#region Static Fields
		private readonly static MultiUseAttributeCache _multiuseAttributeCache = new MultiUseAttributeCache();
		#endregion

		#region Instance Fields
		private int _order;
        #endregion

        #region Constructors
        public FilterAttribute()
        {
            _order = -1;
        }
        #endregion

        #region Instance Properties
        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("order");
                _order = value;
            }
        }

		public bool AllowMultiple
		{
			get
			{
				return AllowsMultiple(GetType());
			}
		}
        #endregion

		#region Static Methods
		private static bool AllowsMultiple(Type type)
		{
			return _multiuseAttributeCache.AllowMultiple(type);
		}
		#endregion
	}
}
