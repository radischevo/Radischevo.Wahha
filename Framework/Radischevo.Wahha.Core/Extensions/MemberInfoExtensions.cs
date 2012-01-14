using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class MemberInfoExtensions
    {
        #region Static Extensions Methods
        /// <summary>
        /// Helper method used to find attributes of type <typeparamref name="TAttribute"/>, 
        /// associated with the specified member.
        /// </summary>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo member)
        {
            return GetCustomAttributes<TAttribute>(member, false);
        }

        /// <summary>
        /// Helper method used to find attributes of type <typeparamref name="TAttribute"/>, 
        /// associated with the specified member.
        /// </summary>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo member, 
            bool inherit)
        {
			Precondition.Require(member, () => Error.ArgumentNull("member"));
            return member.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>();
        }

        /// <summary>
        /// Helper method used to find attributes 
        /// associated with the specified member.
        /// </summary>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo member, 
            bool inherit, params Type[] types)
        {
			Precondition.Require(member, () => Error.ArgumentNull("member"));
            return member.GetCustomAttributes(inherit).Cast<Attribute>().Where(m => types.Contains(m.GetType()));
        }
		
		public static Type GetMemberType (this MemberInfo member)
		{
			Precondition.Require(member, () => Error.ArgumentNull("member"));
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
				
				case MemberTypes.Method:
					return ((MethodInfo)member).ReturnType;
				
				case MemberTypes.Event:
					return ((EventInfo)member).EventHandlerType;
				
				case MemberTypes.Constructor:
					return typeof(void);
			}
			return null;
		}
        #endregion
    }
}
