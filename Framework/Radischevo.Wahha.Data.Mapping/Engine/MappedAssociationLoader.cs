using System;
using System.Collections.Generic;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Mapping.Configurations;

namespace Radischevo.Wahha.Data.Mapping
{
	internal static class MappedAssociationLoader
	{
		#region Static Methods		
		public static IAssociationLoader<TAssociation> Scalar<TAssociation>(MetaColumn column, IValueSet source)
			where TAssociation : class
		{
			MetaType type = column.DeclaringType;
			
			// TODO: Билдер уйдет в DbQueryGenerator.
			StringBuilder builder = new StringBuilder();
			builder.Append("SELECT [")
				.Append(column.Name)
				.Append("] FROM ");
			
			if (!String.IsNullOrEmpty(type.Schema))
				builder.Append('[').Append(type.Schema).Append("].");
			
			builder.Append('[').Append(type.Table)
				.Append("] WHERE ");
			
			ValueDictionary arguments = new ValueDictionary();
			foreach (MetaMember key in type.Keys)
			{
				string memberKey = key.GetMemberKey();
				builder.Append('[').Append(memberKey).AppendFormat("] = @{0}", memberKey).Append(" AND ");
				arguments[memberKey] = source.GetValue<object>(memberKey);
			}
			builder.Length -= 5;
			
			return new ScalarMappedAssociationLoader<TAssociation>(
				new DbCommandDescriptor(builder.ToString(), arguments), 
				column.Binder);
		}
		
		public static IAssociationLoader<TAssociation> Single<TAssociation>(MetaAssociation association, IValueSet source)
		{
			// Gonna move to DbQueryGenerator			
			MetaType otherType = Configuration.Instance.Factory.CreateMapping(typeof(TAssociation));
			
			StringBuilder builder = new StringBuilder();
			builder.Append("SELECT ");
			
			foreach (MetaMember member in otherType.Members)
				builder.Append('[').Append(member.GetMemberKey()).Append("],");
			
			builder.Length--;
			builder.Append(" FROM ");
			
			if (!String.IsNullOrEmpty(otherType.Schema))
				builder.Append('[').Append(otherType.Schema).Append("].");
			
			builder.Append('[').Append(otherType.Table).Append("] WHERE [")
				.Append(association.OtherKey).Append("]=@").Append(association.OtherKey);
			
			ValueDictionary arguments = new ValueDictionary();
			arguments.Add(association.OtherKey, source.GetValue<object>(association.ThisKey));
			
			return new SingleMappedAssociationLoader<TAssociation>(new DbCommandDescriptor(builder.ToString(), arguments));
		}
		
		public static IAssociationLoader<IEnumerable<TAssociation>> Multiple<TAssociation>(MetaAssociation association, IValueSet source)
		{
			// Gonna move to DbQueryGenerator			
			MetaType otherType = Configuration.Instance.Factory.CreateMapping(typeof(TAssociation));
			
			StringBuilder builder = new StringBuilder();
			builder.Append("SELECT ");
			
			foreach (MetaMember member in otherType.Members)
				builder.Append('[').Append(member.GetMemberKey()).Append("],");
			
			builder.Length--;
			builder.Append(" FROM ");
			
			if (!String.IsNullOrEmpty(otherType.Schema))
				builder.Append('[').Append(otherType.Schema).Append("].");
			
			builder.Append('[').Append(otherType.Table).Append("] WHERE [")
				.Append(association.OtherKey).Append("]=@").Append(association.OtherKey);
			
			ValueDictionary arguments = new ValueDictionary();
			arguments.Add(association.OtherKey, source.GetValue<object>(association.ThisKey));
			
			return new MultipleMappedAssociationLoader<TAssociation>(new DbCommandDescriptor(builder.ToString(), arguments));
		}
		#endregion
	}
}

