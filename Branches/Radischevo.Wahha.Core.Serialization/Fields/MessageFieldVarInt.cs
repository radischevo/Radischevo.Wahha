using System;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class MessageFieldVarint : MessageField
	{
		#region Constructors
		public MessageFieldVarint(int tag, IFieldIO fieldIO)
			: base(tag, fieldIO)
		{
		}
		#endregion

		#region Instance Methods
		public override void AppendWriteField(ILGenerator generator)
		{
			generator.Call<MessageWriter>("WriteVarint", FieldType);
		}
		#endregion
	}
}
