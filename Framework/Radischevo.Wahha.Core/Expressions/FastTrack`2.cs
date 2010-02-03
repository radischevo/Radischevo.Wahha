using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core.Expressions
{
    internal static class FastTrack<TClass, TMember>
    {
        #region Nested Types
        private sealed class ConstMemberLookupCache : ReaderWriterCache<MemberInfo, Func<object, TMember>>
        {
            #region Constructors
            public ConstMemberLookupCache()
                : base()
            {
            }
            #endregion

            #region Static Methods
            private static Func<object, TMember> CreateFunc(MemberInfo member)
            {
                ParameterExpression constParam = Expression.Parameter(typeof(object), "constValue");
                
                UnaryExpression cast = Expression.Convert(constParam, member.DeclaringType);
                MemberExpression memberExpr = Expression.MakeMemberAccess(cast, member);

                return Expression.Lambda<Func<object, TMember>>(memberExpr, constParam).Compile();
            }
            #endregion

            #region Instance Methods
            public Func<object, TMember> GetFunc(MemberInfo member)
            {
                return GetOrCreate(member, () => CreateFunc(member));
            }
            #endregion
        }

        private sealed class InstanceMemberLookupCache : ReaderWriterCache<MemberInfo, Func<TClass, TMember>>
        {
            #region Constructors
            public InstanceMemberLookupCache()
                : base()
            {
            }
            #endregion

            #region Static Methods
            private static Func<TClass, TMember> CreateFunc(MemberInfo member, bool isStatic)
            {
                ParameterExpression instanceParam = Expression.Parameter(typeof(TClass), "instance");
                MemberExpression memberExpr = Expression.MakeMemberAccess(
                    (isStatic) ? null : instanceParam, member);

                return Expression.Lambda<Func<TClass, TMember>>(memberExpr, instanceParam).Compile();
            }
            #endregion

            #region Instance Methods
            public Func<TClass, TMember> GetFunc(MemberInfo member, bool isStatic)
            {
                return GetOrCreate(member, () => CreateFunc(member, isStatic));
            }
            #endregion
        }
        #endregion

        #region Static Fields
        private static readonly ConstMemberLookupCache 
            _constMemberLookupCache = new ConstMemberLookupCache();
        private static readonly InstanceMemberLookupCache 
            _instanceMemberLookupCache = new InstanceMemberLookupCache();
        private static Func<TClass, TMember> _identityFunc;
        #endregion

        #region Static Methods
        private static Func<TClass, TMember> GetInstanceMemberLookupFunction(
            MemberInfo member, bool isStatic)
        {
            return _instanceMemberLookupCache.GetFunc(member, isStatic);
        }

        private static Func<TClass, TMember> GetConstMemberLookupFunction(
            MemberInfo member, object constValue)
        {
            Func<object, TMember> innerFunc = _constMemberLookupCache.GetFunc(member);
            return _ => innerFunc(constValue);
        }

        private static Func<TClass, TMember> GetIdentityFunction()
        {
            if (_identityFunc == null)
            {
                ParameterExpression instanceParameter =
                    Expression.Parameter(typeof(TClass), "instance");

                _identityFunc = Expression.Lambda<Func<TClass, TMember>>(
                    instanceParameter, instanceParameter).Compile();
            }
            return _identityFunc;
        }

        public static Func<TClass, TMember> GetFunction(
            ParameterExpression instanceParameter, Expression body)
        {
            if (instanceParameter == body)
                return GetIdentityFunction();
        
            ConstantExpression constantExpression = (body as ConstantExpression);
            if (constantExpression != null)
            {
                TMember value = (TMember)constantExpression.Value;
                return _ => value;
            }
            
            MemberExpression memberExpression = (body as MemberExpression);
            if (memberExpression != null)
            {
                if (memberExpression.Expression == null)
                    return GetInstanceMemberLookupFunction(memberExpression.Member, true);
                else if (memberExpression.Expression == instanceParameter)
                    return GetInstanceMemberLookupFunction(memberExpression.Member, false);
                else
                {
                    constantExpression = (memberExpression.Expression as ConstantExpression);
                    if (constantExpression != null)
                        return GetConstMemberLookupFunction(memberExpression.Member, constantExpression.Value);
                }
            }
            return null;
        }
        #endregion
    }
}
