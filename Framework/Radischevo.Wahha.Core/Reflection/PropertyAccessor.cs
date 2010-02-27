using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core
{
    public interface IPropertyAccessor : IDynamicAccessor
    {
    }
    
    public class PropertyAccessor : IPropertyAccessor
    {
        #region Nested Types
        private class Accessor<T, V> : DynamicAccessor<T, V>
        {
            #region Instance Fields
            private PropertyInfo _property;
            private DGet<T, V> _dget;
            private DSet<T, V> _dset;
            #endregion

            #region Constructors
            internal Accessor(PropertyInfo property, 
                DGet<T, V> dget, DSet<T, V> dset)
                : base()
            {
                _property = property;
                _dget = dget;
                _dset = dset;
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Gets the value of the property of the 
            /// supplied instance
            /// </summary>
            /// <param name="instance">An object instance, whose 
            /// property value will be retrieved</param>
            public override V GetValue(T instance)
            {
                if(_dget != null)
                    return _dget.Invoke(instance);
                if (_property.CanRead)
                    return (V)_property.GetValue(instance, null);

                throw Error.PropertyIsWriteOnly(_property);
            }

            /// <summary>
            /// Sets the value of the property of the 
            /// supplied instance
            /// </summary>
            /// <param name="instance">An object instance, whose 
            /// property value will be set</param>
            /// <param name="value">A value to set</param>
            public override void SetValue(T instance, V value)
            {
                if (_dset != null)
                {
                    _dset.Invoke(instance, value);
                }
                else
                {
                    if (_property.CanWrite)
                        _property.SetValue(instance, value, null);
                    else
                        throw Error.PropertyIsReadOnly(_property);
                }
            }
            #endregion
        }
        #endregion

        #region Instance Fields
        private PropertyInfo _property;
        private DynamicAccessor _accessor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessor"/> class
        /// </summary>
        /// <param name="property">The property to build accessor for</param>
        public PropertyAccessor(PropertyInfo property)
        {
			Precondition.Require(property, () => Error.ArgumentNull("property"));

            if (property.GetIndexParameters().Length > 0)
                throw Error.CouldNotCreateAccessorForIndexedProperty(property);

            _property = property;
            _accessor = CreateAccessor(property);
        }
        #endregion

        #region Instance Properties
        public MemberInfo Member
        {
            get
            {
                return _property;
            }
        }
        #endregion

        #region Static Methods
        private static DynamicAccessor CreateAccessor(PropertyInfo property)
        {
            Type objectType = property.ReflectedType;
            Type getterType = typeof(DGet<,>).MakeGenericType(objectType, property.PropertyType);
            Type setterType = typeof(DSet<,>).MakeGenericType(objectType, property.PropertyType);

            MethodInfo getMethod = property.GetGetMethod(true);
            MethodInfo setMethod = property.GetSetMethod(true);

            Delegate getDelegate = null;
            Delegate setDelegate = null;

            if(getMethod != null)
                getDelegate = Delegate.CreateDelegate(getterType, getMethod, true);

            if(setMethod != null)
                setDelegate = Delegate.CreateDelegate(setterType, setMethod, true);

            return (DynamicAccessor)Activator.CreateInstance(
                typeof(Accessor<,>).MakeGenericType(objectType, property.PropertyType),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, 
                null, new object[] { property, getDelegate, setDelegate }, null);
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets the value of a property supported by 
        /// the specified object instance
        /// </summary>
        /// <param name="instance">An object whose property value will be returned</param>
        public object GetValue(object instance)
        {
			Precondition.Require(instance, () => Error.ArgumentNull("instance"));
            return _accessor.GetBoxedValue(instance);
        }

        /// <summary>
        /// Gets the typed value of a property supported by 
        /// the specified object instance.
        /// </summary>
        /// <param name="instance">An object whose property value will be returned.</param>
        /// <typeparam name="T">The type of an instance.</typeparam>
        /// <typeparam name="V">The type of the property.</typeparam>
        public V GetValue<T, V>(T instance)
        {
			Precondition.Require(instance, () => Error.ArgumentNull("instance"));
            return (V)_accessor.GetBoxedValue(instance);
        }

        /// <summary>
        /// Sets the value of a property supported by 
        /// the specified object instance
        /// </summary>
        /// <param name="instance">An object whose property value will be set</param>
        /// <param name="value">The value to assign to the property</param>
        public void SetValue(object instance, object value)
        {
			Precondition.Require(instance, () => Error.ArgumentNull("instance"));
            _accessor.SetBoxedValue(instance, value);
        }
        #endregion
    }
}
