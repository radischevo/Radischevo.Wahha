using System;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class UnknownFieldVarint : UnknownField
	{
		#region Constructors
		public UnknownFieldVarint(MessageTag tag, MessageReader reader)
			: base(tag, reader.ReadInt64())
		{
		}
		public UnknownFieldVarint(MessageTag tag, long value)
			: base(tag, value)
		{
		}
		#endregion

		#region Instance Methods
		protected override void SerializeCore(MessageWriter writer)
		{
			writer.WriteVarint((long)Value);
		}
		#endregion
	}
}
