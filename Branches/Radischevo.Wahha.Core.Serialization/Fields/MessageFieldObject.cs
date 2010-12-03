using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class MessageFieldObject : MessageField
	{
		#region Constructors
		public MessageFieldObject(int tag, IFieldIO fieldIO)
			: base(tag, fieldIO)
		{
		}
		#endregion

		#region Instance Properties
		public override WireType WireType
		{
			get
			{
				return WireType.String;
			}
		}
		#endregion

		#region Instance Methods
		public override void AppendWriteField(ILGenerator generator)
		{
			generator.Emit(OpCodes.Ldc_I4, Number);
			generator.Emit(OpCodes.Call, typeof(MessageWriter)
				.GetMethod("WriteObject").MakeGenericMethod(FieldType));
		}

		public override void AppendReadField(ILGenerator generator)
		{
			generator.Emit(OpCodes.Call, typeof(MessageReader).GetMethod("Read", 
				BindingFlags.NonPublic | BindingFlags.Instance)
				.MakeGenericMethod(FieldType));
		}

		public override void AppendHeader(ILGenerator generator)
		{
			generator.Emit(OpCodes.Ldarg_1);
		}
		#endregion
	}
}
