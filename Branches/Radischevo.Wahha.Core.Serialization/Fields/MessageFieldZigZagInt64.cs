using System;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class MessageFieldZigZagInt64 : MessageField
	{
		#region Constructors
		public MessageFieldZigZagInt64(int tag, IFieldIO fieldIO)
			: base(tag, fieldIO)
		{
		}
		#endregion

		#region Instance Methods
		public override void AppendWriteField(ILGenerator generator)
		{
			generator.Call<MessageWriter>("WriteZigZag", typeof(int));
		}

		public override void AppendReadField(ILGenerator generator)
		{
			generator.Call<MessageReader>("ReadZigZag64");
		}
		#endregion
	}
}
