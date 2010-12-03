using System;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class UnknownFieldGroup : UnknownField
	{
		#region Constructors
		public UnknownFieldGroup(MessageTag tag, MessageReader reader)
			: base(tag, ReadGroup(tag, reader))
		{
		}
		#endregion

		#region Static Methods
		private static UnknownFieldCollection ReadGroup(MessageTag startTag, MessageReader reader)
		{
			UnknownFieldCollection group = new UnknownFieldCollection();
			for (int stop = startTag.WithWireType(WireType.EndGroup), 
				tag = reader.ReadInt32(); tag != stop; tag = reader.ReadInt32())
				group.Add(new MessageTag(tag), reader);

			return group;
		}
		#endregion

		#region Instance Methods
		protected override void SerializeCore(MessageWriter writer)
		{
			UnknownFieldCollection field = (Value as UnknownFieldCollection);
			field.Serialize(writer);
		}
		#endregion
	}
}
