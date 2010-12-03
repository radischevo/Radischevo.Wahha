using System;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class UnknownFieldFixed32 : UnknownField
	{
		#region Constructors
		public UnknownFieldFixed32(MessageTag tag, MessageReader reader)
			: base(tag, reader.ReadFixedInt32())
		{
		}
		#endregion

		#region Instance Methods
		protected override void SerializeCore(MessageWriter writer)
		{
			writer.WriteFixed((int)Value);
		}
		#endregion
	}
}
