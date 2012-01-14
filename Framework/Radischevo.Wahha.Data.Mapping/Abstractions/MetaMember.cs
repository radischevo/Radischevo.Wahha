using System;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	/// <summary>
    /// Stores mapping metadata for the class member.
    /// </summary>
	public abstract class MetaMember
	{
		#region Instance Fields
		private MetaType _declaringType;
		private MemberInfo _member;
		private MetaAccessor _accessor;
		private Type _type;
		#endregion
		
		#region Constructors
		protected MetaMember (MetaType declaringType, MemberInfo member)
		{
			Precondition.Require(declaringType, () => Error.ArgumentNull("declaringType"));
			Precondition.Require(member, () => Error.ArgumentNull("member"));
			
			_declaringType = declaringType;
			_member = member;
			_type = member.GetMemberType();
			_accessor = new MetaAccessor(member);
		}
		#endregion
		
		#region Instance Properties
		/// <summary>
		/// Gets the mapped type member.
		/// </summary>
		protected MemberInfo Member 
		{
			get
			{
				return _member;
			}
		}
		
		/// <summary>
		/// Gets the meta type, which declares 
		/// the current member.
		/// </summary>
		public MetaType DeclaringType
		{
			get
			{
				return _declaringType;
			}
		}
		
		/// <summary>
		/// Gets the type of the current member.
		/// </summary>
		public virtual Type Type 
		{
			get
			{
				return _type;
			}
		}
		
		/// <summary>
		/// Gets the meta accessor for the current member.
		/// </summary>
		public virtual MetaAccessor Accessor
		{
			get
			{
				return _accessor;
			}
		}
		
		/// <summary>
        /// Gets the value, indicating 
        /// whether this member is a key field.
        /// </summary>
		public abstract bool IsKey
		{
			get;
		}
		
		/// <summary>
        /// Gets the value indicating whether the 
        /// current member value is persisted in the 
        /// backend database table.
        /// </summary>
		public abstract bool IsPersistent
		{
			get;
		}
		
		/// <summary>
        /// Gets the value indicating whether the 
        /// current member has an association.
        /// </summary>
		public abstract bool IsAssociation
		{
			get;
		}
		
		/// <summary>
        /// Gets the value, indicating whether a <c>DbNull.Value</c> 
        /// value can be stored in the database.
        /// </summary>
		public abstract bool IsNullable
		{
			get;
		}
		
		/// <summary>
        /// Gets the value indicating that the column 
        /// must be updated after each insert and update.
        /// </summary>
		public abstract bool AutoUpdate
		{
			get;
		}
		
		/// <summary>
        /// Gets the value, indicating whether this 
        /// field is autogenerated by the database on insert.
        /// </summary>
		public abstract bool AutoGenerated
		{
			get;
		}
		#endregion
		
		#region Instance Methods
		public abstract string GetMemberKey ();
		#endregion
	}
}