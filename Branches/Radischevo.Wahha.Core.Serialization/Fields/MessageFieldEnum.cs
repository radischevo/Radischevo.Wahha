using System;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	class MessageFieldEnum : MessageField
	{
		#region Constructors
		public MessageFieldEnum(int tag, IFieldIO fieldIO)
			: base(tag, fieldIO)
		{
		}
		#endregion

		#region Instance Methods
		public override void AppendReadField(ILGenerator generator)
		{
			generator.Emit(OpCodes.Ldtoken, FieldType);
			generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
			generator.Call<MessageReader>("ReadEnum", typeof(Type));
		}

		public override void AppendWriteField(ILGenerator generator)
		{
			generator.Call<MessageWriter>("WriteVarint", typeof(int));
		}
		#endregion
	}
}
