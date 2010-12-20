using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core
{
    public interface IFieldAccessor : IDynamicAccessor
    {
    }

    public class FieldAccessor : IFieldAccessor
    {
        #region Nested Types
        private class Accessor<T, V> : DynamicAccessor<T, V>
        {
            #region Instance Fields
            private FieldInfo _field;
            private DGet<T, V> _dget;
            private DSet<T, V> _dset;
            #endregion

            #region Constructors
            internal Accessor(FieldInfo field, DGet<T, V> dget, DSet<T, V> dset)
                : base()
            {
                _field = field;
                _dget = dget;
                _dset = dset;
            }
            #endregion

            #region Instance Properties
            public override V GetValue(T instance)
            {
                if (_dget != null)
                    return _dget.Invoke(instance);

                return (V)_field.GetValue(instance);
            }

            public override void SetValue(T instance, V value)
            {
                if (_dset != null)
                    _dset.Invoke(instance, value);
                else
                    _field.SetValue(instance, value);
            }
            #endregion
        }
        #endregion

        #region Instance Fields
        private FieldInfo _field;
        private DynamicAccessor _accessor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAccessor"/> class
        /// </summary>
        /// <param name="field">The field to build accessor for</param>
        public FieldAccessor(FieldInfo field)
        {
			Precondition.Require(field, () => Error.ArgumentNull("field"));

            _field = field;
            _accessor = CreateAccessor(field);
        }
        #endregion

        #region Static Methods
        private static DynamicAccessor CreateAccessor(FieldInfo field)
        {
            Type objectType = field.ReflectedType;
            Delegate getDelegate = null;
            Delegate setDelegate = null;

            if (!objectType.IsGenericType)
            {
                DynamicMethod dynamicMethod = new DynamicMethod("xget_" + field.Name,
                    field.FieldType, new Type[] { objectType }, objectType, true);

                ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, field);
                iLGenerator.Emit(OpCodes.Ret);

                getDelegate = dynamicMethod.CreateDelegate(
                    typeof(DGet<,>).MakeGenericType(objectType, field.FieldType));

                dynamicMethod = new DynamicMethod(("xset_" + field.Name),
                    typeof(void), new Type[] { objectType, field.FieldType }, objectType, true);

                iLGenerator = dynamicMethod.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Stfld, field);
                iLGenerator.Emit(OpCodes.Ret);

                setDelegate = dynamicMethod.CreateDelegate(
                    typeof(DSet<,>).MakeGenericType(objectType, field.FieldType));
            }

            return (DynamicAccessor)Activator.CreateInstance(
                typeof(Accessor<,>).MakeGenericType(objectType, field.FieldType),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, 
                null, new object[] { field, getDelegate, setDelegate }, null);
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets the value of a field supported by 
        /// the specified object instance
        /// </summary>
        /// <param name="instance">An object whose field value will be returned</param>
        public object GetValue(object instance)
        {
			Precondition.Require(instance, () => Error.ArgumentNull("instance"));
            return _accessor.GetBoxedValue(instance);
        }

        /// <summary>
        /// Gets the typed value of a field supported by 
        /// the specified object instance.
        /// </summary>
        /// <param name="instance">An object whose field value will be returned.</param>
        /// <typeparam name="T">The type of an instance.</typeparam>
        /// <typeparam name="V">The type of the field.</typeparam>
        public V GetValue<T, V>(T instance)
        {
			Precondition.Require(instance, () => Error.ArgumentNull("instance"));
            return (V)_accessor.GetBoxedValue(instance);
        }

        /// <summary>
        /// Sets the value of a field supported by 
        /// the specified object instance
        /// </summary>
        /// <param name="instance">An object whose field value will be set</param>
        /// <param name="value">The value to assign to the field</param>
        public void SetValue(object instance, object value)
        {
			Precondition.Require(instance, () => Error.ArgumentNull("instance"));
            _accessor.SetBoxedValue(instance, value);
        }
        #endregion
    }
}
