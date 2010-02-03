using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;
using System.Reflection;

namespace Jeltofiol.Wahha.Data.Linq
{
    internal static class TypeHelper
    {
        internal static ConstantExpression GetNullConstant(Type type)
        {
            return Expression.Constant(null, type.MakeNullableType());
        }

        internal static Type GetMemberType(MemberInfo mi)
        {
            FieldInfo fi = (mi as FieldInfo);
            PropertyInfo pi = (mi as PropertyInfo);
            EventInfo ei = (mi as EventInfo);

            if (fi != null) 
                return fi.FieldType;
            
            if (pi != null) 
                return pi.PropertyType;
            
            if (ei != null) 
                return ei.EventHandlerType;

            return null;
        }

        internal static bool IsReadOnly(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return (((FieldInfo)member).Attributes & FieldAttributes.InitOnly) != 0;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)member;
                    return (!pi.CanWrite || pi.GetSetMethod() == null);
                default:
                    return true;
            }
        }
    }
}
