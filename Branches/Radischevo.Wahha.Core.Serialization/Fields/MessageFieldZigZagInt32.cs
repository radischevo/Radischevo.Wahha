using System;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class MessageFieldZigZagInt32 : MessageField
	{
		#region Constructors
		public MessageFieldZigZagInt32(int tag, IFieldIO fieldIO)
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
			generator.Call<MessageReader>("ReadZigZag32");
		}
		#endregion
	}
}
