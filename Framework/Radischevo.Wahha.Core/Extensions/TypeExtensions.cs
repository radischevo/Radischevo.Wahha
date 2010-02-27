using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class TypeExtensions
    {
        #region Helper Methods
        public static Type FindEnumerable(Type type)
        {
            if (type == null || type == typeof(string))
                return null;

            if (type.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());

            if (type.IsGenericType)
            {
                foreach (Type gt in type.GetGenericArguments())
                {
                    Type t = typeof(IEnumerable<>).MakeGenericType(gt);
                    if (t.IsAssignableFrom(type))
                        return t;
                }
            }

            Type[] it = type.GetInterfaces();
            if (it != null && it.Length > 0)
            {
                foreach (Type et in it)
                {
                    Type t = FindEnumerable(et);
                    if (t != null)
                        return t;
                }
            }

            if (type.BaseType == null)
                return null;
            if (type.BaseType == typeof(object))
                return null;

            return TypeExtensions.FindEnumerable(type.BaseType);
        }
        #endregion

        #region Static Extension Methods
        public static Type MakeNonNullableType(this Type type)
        {
            if (type == null)
                return null;

            if (!type.IsGenericType)
                return type;

            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments()[0];

            return type;
        }

        public static Type MakeNullableType(this Type type)
        {
            if(type == null)
                return null;

            if (IsNullable(type))
                return type;

            return typeof(Nullable<>).MakeGenericType(type);
        }

        public static Type MakeSequenceType(this Type type)
        {
            if (type == null)
                return null;
            
            return typeof(IEnumerable<>).MakeGenericType(type);
        }

        public static bool IsNullable(this Type type)
        {
            if (type == null)
                return false;
            
            if (!type.IsValueType)
                return true;

            if (!type.IsGenericType)
                return false;

            return (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static bool IsSequence(this Type type)
        {
            if (type == typeof(string))
                return false;

            if (type == typeof(byte[]))
                return false;

            if (type == typeof(char[]))
                return false;

            return (TypeExtensions.FindEnumerable(type) != null);
        }

        public static Type GetSequenceElementType(this Type type)
        {
            Type sequenceType = TypeExtensions.FindEnumerable(type);
            if (sequenceType == null)
                return type;

            return sequenceType.GetGenericArguments()[0];
        }

        public static Type GetInterface(this Type t, Type type)
        {
			Precondition.Require(t, () => Error.ArgumentNull("t"));
            return t.GetInterface(type.FullName);
        }

        public static Type GetGenericInterface(this Type type, Type interfaceType)
        {
            Func<Type, bool> matchesInterface = t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType;
            return (matchesInterface(type)) ? type : type.GetInterfaces().FirstOrDefault(matchesInterface);
        }

        public static bool IsSimple(this Type type)
        {
            type = TypeExtensions.MakeNonNullableType(type);

            if (type.IsEnum)
                return true;

            if (type == typeof(Guid))
                return true;

            switch (Type.GetTypeCode(type))
            {
                case ((TypeCode)17):
                    return false;
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return true;
            }
            return false;
        }

        public static object CreateInstance(this Type type)
        {
            if (type == null)
                return null;

            if (!IsNullable(type))
                return Activator.CreateInstance(type);

            return null;
        }
        #endregion
    }
}
