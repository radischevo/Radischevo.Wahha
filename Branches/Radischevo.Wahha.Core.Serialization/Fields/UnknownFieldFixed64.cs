using System;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class UnknownFieldFixed64 : UnknownField
	{
		#region Constructors
		public UnknownFieldFixed64(MessageTag tag, MessageReader reader)
			: base(tag, reader.ReadFixedInt64())
		{
		}
		#endregion

		#region Instance Methods
		protected override void SerializeCore(MessageWriter writer)
		{
			writer.WriteFixed((long)Value);
		}
		#endregion
	}
}
