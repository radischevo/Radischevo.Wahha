using System;
using System.Reflection;

namespace Radischevo.Wahha.Data.Mapping
{
	internal static class Error
	{
		internal static Exception ArgumentNull(string argumentName)
        {
            return new ArgumentNullException(argumentName);
        }
		
		internal static Exception UnsupportedMemberType (MemberInfo member, string parameterName) 
		{
			return new ArgumentException(String.Format("Member '{0}' has invalid type. MetaAccessor can only be created for field or property.", member.Name), parameterName);
		}

		internal static Exception InvalidSerializerType (Type type)
		{
			return new InvalidOperationException(
                String.Format("Type '{0}' must implement the IDbSerializer interface", type.FullName));
		}
		
		internal static Exception InvalidColumnBinderType (Type type)
		{
			return new InvalidOperationException(
                String.Format("Type '{0}' must implement the IDbColumnBinder interface", type.FullName));
		}

		internal static Exception InvalidMemberAttributes (MemberInfo member)
		{
			return new InvalidOperationException(String.Format(
				"Invalid member '{0}'. One member cannot be marked with both ColumnAttribute and AssociationAttribute at once.", member.Name));
		}
	}
}

