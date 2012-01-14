using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	public class AttributedMetaType : MetaType
	{
		#region Instance Fields
		private readonly string _table;
		private readonly string _schema;
		private readonly string _projection;
		private readonly IEnumerable<MetaMember> _keys;
		private readonly IKeyedEnumerable<string, MetaMember> _members;
		private readonly Type _serializerType;
		private IDbSerializer _serializer;
		#endregion
		
		#region Constructors
		public AttributedMetaType (Type type)
			: base(type)
		{
			_table = type.Name;
			
			TableAttribute[] attrs = (TableAttribute[])type
				.GetCustomAttributes(typeof(TableAttribute), true);
			
			if (attrs != null && attrs.Length > 0)
			{
				TableAttribute attr = attrs[0];
				
				_schema = attr.Schema;
				_table = attr.Name;
				_projection = attr.Projection;
				_serializerType = attr.Serializer;
			}
			_members = GetMembers(type);
			_keys = _members.Where(a => a.IsKey).AsReadOnly();
		}
		#endregion
		
		#region Instance Properties
		public override IDbSerializer Serializer
		{
			get 
			{
				if (_serializer == null) 
					_serializer = CreateSerializer(_serializerType) ?? base.Serializer;
				
				return _serializer;
			}
		}
		
		public override string Schema 
		{
			get 
			{
				return _schema;
			}
		}

		public override string Table 
		{
			get 
			{
				return _table;
			}
		}

		public override string Projection 
		{
			get 
			{
				return _projection;
			}
		}

		public override IEnumerable<MetaMember> Keys
		{
			get 
			{
				return _keys;
			}
		}

		public override IKeyedEnumerable<string, MetaMember> Members 
		{
			get 
			{
				return _members;
			}
		}
		#endregion
		
		#region Instance Methods
		private IKeyedEnumerable<string, MetaMember> GetMembers(Type type)
		{
			MetaMemberCollection members = new MetaMemberCollection();
			foreach (MemberInfo member in type.GetMembers(BindingFlags.Instance | 
				BindingFlags.Public | BindingFlags.NonPublic)) 
			{
				ColumnAttribute column = (ColumnAttribute)Attribute
					.GetCustomAttribute(member, typeof(ColumnAttribute));				
				AssociationAttribute association = (AssociationAttribute)Attribute
					.GetCustomAttribute(member, typeof (AssociationAttribute));
				
				if (column != null) 
				{
					if (association != null)
						throw Error.InvalidMemberAttributes (member);
					
					members.Add(new AttributedMetaColumn(this, member, column));
				}
				if (association != null)
					members.Add(new AttributedMetaAssociation(this, member, association));
			}
			return members;
		}
		#endregion
	}
}

