using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class MessageFieldString : MessageField
	{
		#region Constructors
		public MessageFieldString(int tag, IFieldIO fieldIO)
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
		public override void AppendReadField(ILGenerator generator)
		{
			generator.Call<MessageReader>("ReadString");
		}

		public override void AppendGuard(ILGenerator generator, 
			MethodInfo method, Label done)
		{
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Call, method);
			generator.Emit(OpCodes.Brfalse, done);
		}

		public override void AppendWriteField(ILGenerator generator)
		{
			generator.Call<MessageWriter>("WriteString", typeof(string));
		}
		#endregion
	}
}
