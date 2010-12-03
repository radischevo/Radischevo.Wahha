using System;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class MessageFieldInt16 : MessageField
	{
		#region Constructors
		public MessageFieldInt16(int tag, IFieldIO fieldIO)
			: base(tag, fieldIO)
		{
		}
		#endregion

		#region Instance Methods
		public override void AppendWriteField(ILGenerator generator)
		{
			generator.Emit(OpCodes.Conv_I4);
			generator.Call<MessageWriter>("WriteVarint", typeof(int));
		}

		public override void AppendReadField(ILGenerator generator)
		{
			generator.Call<MessageReader>("ReadInt32");
		}
		#endregion
	}
}
