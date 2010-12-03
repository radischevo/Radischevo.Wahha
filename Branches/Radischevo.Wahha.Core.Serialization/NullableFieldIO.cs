using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization
{
	public class NullableFieldIO : FieldIOBase
	{
		#region Constructors
		private NullableFieldIO(PropertyInfo property,
			bool skipNullabilityCheck)
			: base(property)
		{
			if (skipNullabilityCheck || IsNullable(property))
				return;

			throw new NotSupportedException();
		}

		public NullableFieldIO(PropertyInfo property)
			: this(property, false)
		{
		}
		#endregion

		#region Instance Properties
		public override Type FieldType
		{
			get
			{
				return Property.PropertyType.GetGenericArguments()[0];
			}
		}
		#endregion

		#region Static Methods
		private static bool IsNullable(PropertyInfo property)
		{
			Type type = property.PropertyType;
			return type.IsGenericType && 
				type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static bool TryCreate(PropertyInfo property, out IFieldIO io)
		{
			if (IsNullable(property))
			{
				io = new NullableFieldIO(property, true);
				return true;
			}
			io = null;
			return false;
		}
		#endregion

		#region Instance Methods
		public override void AppendRead(ILGenerator generator, MessageField field)
		{
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldarg_1);
			field.AppendReadField(generator);
			generator.Emit(OpCodes.Newobj, typeof(Nullable<>)
				.MakeGenericType(FieldType)
				.GetConstructor(new Type[] { FieldType }));

			generator.Emit(OpCodes.Call, Property.GetSetMethod());
		}

		public override void AppendWrite(ILGenerator generator, MessageField field)
		{
			var done = generator.DefineLabel();
			var tmp = generator.DeclareLocal(typeof(Nullable<>).MakeGenericType(FieldType));

			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Call, Property.GetGetMethod());
			generator.Emit(OpCodes.Stloc, tmp.LocalIndex);

			generator.Emit(OpCodes.Ldloca, tmp.LocalIndex);
			generator.Emit(OpCodes.Call, typeof(Nullable<>)
				.MakeGenericType(FieldType)
				.GetProperty("HasValue")
				.GetGetMethod());

			generator.Emit(OpCodes.Brfalse_S, done);

			field.AppendGuard(generator, Property.GetGetMethod(), done);
			field.AppendHeader(generator);

			generator.Emit(OpCodes.Ldloca, tmp.LocalIndex);
			generator.Emit(OpCodes.Call, typeof(Nullable<>)
				.MakeGenericType(FieldType)
				.GetProperty("Value")
				.GetGetMethod());

			field.AppendWriteField(generator);
			generator.Emit(OpCodes.Pop);
			generator.MarkLabel(done);
		}
		#endregion
	}
}
