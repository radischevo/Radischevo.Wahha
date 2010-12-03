using System;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class UnknownFieldString : UnknownField
	{
		#region Constructors
		public UnknownFieldString(MessageTag tag, MessageReader reader)
			: base(tag, reader.CreateSubReader().ReadString())
		{
		}
		#endregion

		#region Instance Methods
		protected override void SerializeCore(MessageWriter writer)
		{
			writer.WriteString(Value as string);
		}
		#endregion
	}
}
