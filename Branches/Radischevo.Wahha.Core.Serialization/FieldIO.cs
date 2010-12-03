using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization
{
	internal class FieldIO : FieldIOBase
	{
		#region Constructors
		public FieldIO(PropertyInfo property)
			: base(property)
		{
		}
		#endregion

		#region Instance Methods
		public override void AppendRead(ILGenerator generator, MessageField field)
		{
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldarg_1);
			field.AppendReadField(generator);

			generator.Emit(OpCodes.Call, Property.GetSetMethod());
		}

		public override void AppendWrite(ILGenerator generator, MessageField field)
		{
			var done = generator.DefineLabel();
			field.AppendGuard(generator, Property.GetGetMethod(), done);

			field.AppendHeader(generator);

			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Call, Property.GetGetMethod());
			field.AppendWriteField(generator);
			generator.Emit(OpCodes.Pop);
			generator.MarkLabel(done);
		}
		#endregion
	}
}
