using System;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	public class MetaAccessor : IDynamicAccessor
	{
		#region Instance Fields
		private readonly IDynamicAccessor _accessor;
		private readonly Type _type;
		private readonly bool _isDeferred;
		#endregion
		
		#region Constructors
		public MetaAccessor (MemberInfo member)
		{
			Precondition.Require(member, () => Error.ArgumentNull("member"));
			
			Type memberType = member.GetMemberType();
			Type deferredType = GetDeferredType(memberType);
			
			_accessor = CreateAccessor(member);
			_isDeferred = (deferredType != null);
			_type = deferredType ?? memberType;
		}
		#endregion
		
		#region Instance Properties
		public bool IsDeferred
		{
			get
			{
				return _isDeferred;
			}
		}
		
		public Type Type 
		{
			get
			{
				return _type;
			}
		}
		#endregion
		
		#region Static Methods
		private static IDynamicAccessor CreateAccessor (MemberInfo member)
		{
			switch (member.MemberType) 
			{
				case MemberTypes.Property:
					return ((PropertyInfo)member).CreateAccessor();
				
				case MemberTypes.Field:
					return ((FieldInfo)member).CreateAccessor();
			}
			throw Error.UnsupportedMemberType(member, "member");
		}
		
		private static Type GetDeferredType (Type type)
		{
			Type linkType = type.GetGenericInterface(typeof(ILink<>));
			if (linkType == null)
				return null;
			
			return linkType.GetGenericArguments()[0];
		}
		#endregion
		
		#region Instance Methods
		public object GetValue (object instance)
		{
			return _accessor.GetValue(instance);
		}

		public void SetValue (object instance, object value)
		{
			_accessor.SetValue(instance, value);
		}
		#endregion
	}
}

