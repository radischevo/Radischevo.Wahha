using System;

namespace Radischevo.Wahha.Core
{
    internal delegate V DGet<T, V>(T t);
    internal delegate void DSet<T, V>(T t, V v);

    /// <summary>
    /// Provides methods to get or set 
    /// field or property value
    /// </summary>
    internal abstract class DynamicAccessor
    {
        #region Constructors
        protected DynamicAccessor()
        {   }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets the value of the instance member
        /// </summary>
        /// <param name="instance">The object instance, 
        /// whose member value will be retrieved</param>
        internal abstract object GetBoxedValue(object instance);

        /// <summary>
        /// Sets the value of the instance member
        /// </summary>
        /// <param name="instance">The object instance, 
        /// whose member value will be set</param>
        /// <param name="value">A value to set</param>
        internal abstract void SetBoxedValue(object instance, object value);
        #endregion
    }

    /// <summary>
    /// Provides methods to get or set 
    /// field or property value
    /// </summary>
    /// <typeparam name="T">The type of object, 
    /// that owns the member</typeparam>
    /// <typeparam name="V">The member type</typeparam>
    internal abstract class DynamicAccessor<T, V> : DynamicAccessor
    {
        #region Constructors
        protected DynamicAccessor() 
            : base()
        {
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets the boxed value of the object member
        /// </summary>
        /// <param name="instance">The object, whose member value 
        /// will be retrieved</param>
        internal override object GetBoxedValue(object instance)
        {
            return GetValue((T)instance);
        }

        /// <summary>
        /// Sets the boxed value of the object member
        /// </summary>
        /// <param name="instance">The object, whose member value 
        /// will be set</param>
        /// <param name="value">A value to set</param>
        internal override void SetBoxedValue(object instance, object value)
        {
            SetValue((T)instance, ((V)value));
        }

        /// <summary>
        /// Gets the value of the instance member
        /// </summary>
        /// <param name="instance">The object instance, 
        /// whose member value will be retrieved</param>
        public abstract V GetValue(T instance);

        /// <summary>
        /// Sets the value of the instance member
        /// </summary>
        /// <param name="instance">The object instance, 
        /// whose member value will be set</param>
        /// <param name="value">A value to set</param>
        public abstract void SetValue(T instance, V value);
        #endregion
    }
}
