using System;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	/// <summary>
    /// Stores metadata for table column mapping.
    /// </summary>
	public abstract class MetaColumn : MetaMember
	{
		#region Constructors
		protected MetaColumn (MetaType declaringType, MemberInfo member)
			: base(declaringType, member)
		{
		}
		#endregion
		
		#region Instance Properties
		/// <summary>
        /// Gets the name of the database column 
        /// that the current member is mapped to.
        /// </summary>
		public abstract string Name
		{
			get;
		}
		
		/// <summary>
        /// Gets the value indicating whether the 
        /// current member value is persisted in the 
        /// backend database table.
        /// </summary>
		public override sealed bool IsPersistent 
		{
			get 
			{
				return true;
			}
		}
		
		/// <summary>
        /// Gets the value indicating whether the 
        /// current member has an association.
        /// </summary>
		public override sealed bool IsAssociation 
		{
			get 
			{
				return false;
			}
		}
		
		public abstract IDbColumnBinder Binder
		{
			get;
		}
		#endregion
		
		#region Static Methods
		protected static IDbColumnBinder CreateBinder (Type type)
		{
			if (type == null)
				return null;
			
			if (!typeof(IDbColumnBinder).IsAssignableFrom(type))
				throw new InvalidOperationException(); // TODO: Create Error.InvalidColumnBinderType()

			return (IDbColumnBinder)ServiceLocator.Instance.GetService(type);
		}
		#endregion
		
		#region Instance Methods
		public override string GetMemberKey ()
		{
			return Name;
		} 
		#endregion
	}
}

