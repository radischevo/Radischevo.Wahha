using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization
{
	internal class PackedFieldIO : RepeatedFieldIO
	{
		#region Constructors
		protected PackedFieldIO(PropertyInfo property, MethodInfo add)
			: base(property, add)
		{
		}
		#endregion

		#region Static Methods
		public static new bool TryCreate(PropertyInfo property, out IFieldIO io)
		{
			return TryCreate(property, add => new PackedFieldIO(property, add), out io);
		}
		#endregion

		#region Instance Methods
		protected override void AppendMessageHeaderCore(
			ILGenerator generator, ForEachLoop loop, MessageField field)
		{
			LocalBuilder length = generator.DeclareLocal(typeof(int));
			loop.Create(body => {
				body.Emit(OpCodes.Ldarg_1);
				loop.LoadCurrentAs(FieldType);
				field.AppendFieldLength(generator);
				body.Emit(OpCodes.Ldloc, length.LocalIndex);
				body.Emit(OpCodes.Add);
				body.Emit(OpCodes.Stloc, length.LocalIndex);
			});

			generator.Emit(OpCodes.Ldloc, length.LocalIndex);
			Label done = generator.DefineLabel();
			generator.Emit(OpCodes.Brfalse, done);

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldc_I4, MessageTag.AsInt(field.Number, WireType.String));
			generator.Call<MessageWriter>("WriteVarint", typeof(uint));
			generator.Emit(OpCodes.Ldloc, length.LocalIndex);
			generator.Call<MessageWriter>("WriteVarint", typeof(uint)).Pop();
			generator.MarkLabel(done);
		}

		public override void AppendRead(ILGenerator generator, MessageField field)
		{
			LocalBuilder count = generator.DeclareLocal(typeof(uint));
			Label top = generator.DefineLabel();
			Label next = generator.DefineLabel();

			generator.Emit(OpCodes.Br_S, next);
			generator.Emit(OpCodes.Nop);
			generator.MarkLabel(top);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Call, Property.GetGetMethod());
			generator.Emit(OpCodes.Ldarg_1);
			field.AppendReadField(generator);
			generator.Emit(OpCodes.Callvirt, AddMethod);
			generator.MarkLabel(next);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Call, typeof(MessageReader)
				.GetProperty("EndOfStream")
				.GetGetMethod());

			generator.Emit(OpCodes.Brfalse_S, top);
		}
		#endregion
	}
}
