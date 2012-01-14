using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;
using Radischevo.Wahha.Data.Mapping;

namespace ConsoleTester
{
	public class Materializer : MappedDbSerializer<Comment>
	{
		#region Constructors
		public Materializer (IMetaMappingFactory factory)
			: base(factory)
		{
		}
		#endregion
		
		#region Instance Methods
		public static IAssociationLoader<Topic> Single(MetaAssociation column, IValueSet source)
		{
			return null;
		}
		
		protected override Comment Deserialize (Comment entity, IDbValueSet source)
		{
			return Deserialize(MetaType, entity, source);
		}
		
		private static Comment Deserialize(MetaType type, Comment entity, IDbValueSet source)
		{
			MetaMember member;

			member = type.Members["topic"];
			ILink<Topic> attachment_link = (ILink<Topic>)member.Accessor.GetValue(entity);
			
			if (attachment_link == null)
				attachment_link = new Link<Topic>();
			
			attachment_link.Source = Materializer.Single((MetaAssociation)member, source).Load;
			
			/*if (source.ContainsAny("topic")) 
			{
				IValueSet topicValue = source.GetValue<IValueSet>("topic");
				attachment_link.Value = 
			}*/
			member.Accessor.SetValue(entity, attachment_link);
			
			return entity;
		}
		#endregion
	}
}

