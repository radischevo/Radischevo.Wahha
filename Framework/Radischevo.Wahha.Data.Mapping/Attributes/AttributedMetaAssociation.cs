using System;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	public class AttributedMetaAssociation : MetaAssociation
	{
		#region Instance Fields
		private readonly bool _isNullable;
		private readonly bool _isProjected;
		private readonly bool _isPersistent;
		private readonly string _thisKey;
		private readonly string _otherKey;
		private readonly string _prefix;
		#endregion
		
		#region Constructors
		public AttributedMetaAssociation (MetaType declaringType, 
			MemberInfo member, AssociationAttribute attribute)
			: base(declaringType, member)
		{
			Precondition.Require(attribute, () => Error.ArgumentNull("attribute"));
			
			_isNullable = attribute.CanBeNull;
			_isProjected = attribute.Projected;
			_isPersistent = attribute.Persistent;
			_thisKey = attribute.ThisKey ?? ((_isPersistent) ? member.Name : null);
			_otherKey = attribute.OtherKey;
			_prefix = attribute.Prefix ?? String.Concat(member.Name, ".");
		}
		#endregion
		
		#region Instance Properties
		public override bool AutoUpdate 
		{
			get 
			{
				return false;
			}
		}

		public override bool IsNullable 
		{
			get 
			{
				return _isNullable;
			}
		}

		public override bool IsPersistent 
		{
			get 
			{
				return _isPersistent;
			}
		}
		
		public override string ThisKey
		{
			get 
			{
				return _thisKey;
			}
		}
		
		public override string OtherKey 
		{
			get 
			{
				return _otherKey;
			}
		}
		
		public override bool IsProjected 
		{
			get 
			{
				return _isProjected;
			}
		}
		
		public override string Prefix 
		{
			get 
			{
				return _prefix;
			}
		}
		#endregion
	}
}

