using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization.Fields
{
	internal class MessageFieldPacked : MessageField
	{
		#region Instance Fields
		private MessageField _parent;
		#endregion

		#region Constructors
		public MessageFieldPacked(MessageField parent)
			: base(parent)
		{
			if (!parent.WireType.EqualsAny(WireType.Varint, 
				WireType.Fixed32, WireType.Fixed64))
				throw new NotSupportedException(); // TODO: Написать Error.NotSupported

			_parent = parent;
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
			_parent.AppendWriteField(generator);
		}

		public override void AppendFieldLength(ILGenerator generator)
		{
			_parent.AppendFieldLength(generator);
		}

		public override void AppendReadField(ILGenerator generator)
		{
			_parent.AppendReadField(generator);
		}

		public override void AppendGuard(ILGenerator generator,
			MethodInfo method, Label done)
		{
			_parent.AppendGuard(generator, method, done);
		}

		public override void AppendHeader(ILGenerator generator)
		{
			generator.Emit(OpCodes.Ldarg_1);
		}
		#endregion
	}
}
