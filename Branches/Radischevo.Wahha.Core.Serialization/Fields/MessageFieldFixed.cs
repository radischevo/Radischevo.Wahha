using System;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class MessageFieldFixed : MessageField
	{
		#region Instance Fields
		private readonly WireType _wireType;
		#endregion

		#region Constructors
		public MessageFieldFixed(int tag, IFieldIO fieldIO, WireType wireType)
			: base(tag, fieldIO)
		{
			_wireType = wireType;
		}
		#endregion

		#region Instance Properties
		public override WireType WireType
		{
			get
			{
				return _wireType;
			}
		}
		#endregion

		#region Instance Methods
		public override void AppendWriteField(ILGenerator generator)
		{
			generator.Call<MessageWriter>("WriteFixed", FieldType);
		}

		public override void AppendReadField(ILGenerator generator)
		{
			generator.Call<MessageReader>("ReadFixed" + FieldType.Name);
		}

		public override void AppendFieldLength(ILGenerator generator)
		{
			generator.Pop().Pop().Emit(OpCodes.Ldc_I4, 
				(WireType == WireType.Fixed32) ? 4 : 8);
		}
		#endregion
	}
}
