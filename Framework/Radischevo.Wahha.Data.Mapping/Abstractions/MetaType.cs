using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	/// <summary>
    /// Stores information related to table-to-type mapping
    /// </summary>
	public abstract class MetaType
	{
		#region Instance Fields
		private readonly object _lock = new object();
		private readonly Type _type;
		private IDbSerializer _serializer;
        #endregion
		
		#region Constructors
		protected MetaType (Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			_type = type;
		}
		#endregion
		
		#region Instance Properties
		public Type Type
		{
			get
			{
				return _type;
			}
		}
		
		public virtual IDbSerializer Serializer
		{
			get
			{
				if (_serializer == null) 
				{
					lock(_lock) 
					{
						if (_serializer == null)
							_serializer = MappedDbSerializerBuilder.Build (this);
					}
				}
				return _serializer;
			}
		}
		
		public abstract string Schema
		{
			get;
		}
		
		public abstract string Table
		{
			get;
		}
		
		public abstract string Projection
		{
			get;
		}
		
		public abstract IEnumerable<MetaMember> Keys
		{
			get;
		}
		
		public abstract IKeyedEnumerable<string, MetaMember> Members
		{
			get;
		}
		#endregion
		
		#region Static Methods
		protected static IDbSerializer CreateSerializer (Type type)
		{
			if (type == null)
				return null;
			
			if (!typeof(IDbSerializer).IsAssignableFrom(type))
				throw Error.InvalidSerializerType(type);

			return (IDbSerializer)ServiceLocator.Instance.GetService(type);
		}
		#endregion
	}
}

