using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using Radischevo.Wahha.Core.Serialization.Fields;

namespace Radischevo.Wahha.Core.Serialization
{
	public class MessageField
	{
		#region Instance Fields
		private int _tag;
		private IFieldIO _fieldIO;
		#endregion

		#region Constructors
		protected MessageField(int tag, IFieldIO fieldIO)
		{
			_tag = tag;
			_fieldIO = fieldIO;
		}

		protected MessageField(MessageField other) : 
			this(other._tag, other._fieldIO)
		{
		}
		#endregion

		#region Instance Properties
		public int Number
		{
			get
			{
				return _tag;
			}
		}

		public int Header
		{
			get
			{
				return (_tag << 3 | (int)WireType);
			}
		}

		public bool CanAppendWrite
		{
			get
			{
				return _fieldIO.CanCreateWriter;
			}
		}

		public bool CanAppendRead
		{
			get
			{
				return _fieldIO.CanCreateReader;
			}
		}

		public virtual WireType WireType
		{
			get
			{
				return WireType.Varint;
			}
		}

		protected Type FieldType
		{
			get
			{
				return _fieldIO.FieldType;
			}
		}
		#endregion

		#region Static Methods
		private static IFieldIO CreateFieldIO(PropertyInfo property)
		{
			IFieldIO fieldIO;
			if (RepeatedFieldIO.TryCreate(property, out fieldIO)
				|| NullableFieldIO.TryCreate(property, out fieldIO))
				return fieldIO;

			return new FieldIO(property);
		}

		private static MessageField Create(TagAttribute attr, IFieldIO io)
		{
			int tag = attr.Number;
			if (attr.Fixed)
				return CreateFixed(tag, io);

			if (attr.ZigZag)
				return CreateZigZag(tag, io);

			Type type = io.FieldType;
			if (type.EqualsAny(typeof(int), typeof(long), 
				typeof(bool), typeof(uint), typeof(ulong)))
				return new MessageFieldVarint(tag, io);

			if (type == typeof(string))
				return new MessageFieldString(tag, io);

			if (type == typeof(byte[]))
				return new MessageFieldBytes(tag, io);

			if (type == typeof(float))
				return new MessageFieldFixed(tag, io, WireType.Fixed32);

			if (type == typeof(double))
				return new MessageFieldFixed(tag, io, WireType.Fixed64);

			if (type.IsEnum)
				return new MessageFieldEnum(tag, io);

			if (type == typeof(DateTime) || type == typeof(Decimal))
				return new MessageField(tag, io);

			if (type == typeof(short) || type == typeof(ushort))
				return new MessageFieldInt16(tag, io);

			return new MessageFieldObject(tag, io);
		}

		private static MessageField CreateFixed(int tag, IFieldIO io)
		{
			Type type = io.FieldType;

			if (type == typeof(int) || type == typeof(uint))
				return new MessageFieldFixed(tag, io, WireType.Fixed32);

			if (type == typeof(long) || type == typeof(ulong))
				return new MessageFieldFixed(tag, io, WireType.Fixed64);

			throw new NotSupportedException(String.Format("Unsupported field type \"{0}\"", type));
		}

		private static MessageField CreateZigZag(int tag, IFieldIO io)
		{
			Type type = io.FieldType;

			if (type == typeof(int))
				return new MessageFieldZigZagInt32(tag, io);

			if (type == typeof(long))
				return new MessageFieldZigZagInt64(tag, io);

			if (type == typeof(short))
				return new MessageFieldZigZagInt16(tag, io);

			throw new NotSupportedException(String.Format("Unsupported field type \"{0}\"", type));
		}

		public static MessageField Create(TagAttribute attr, PropertyInfo property)
		{
			IFieldIO fieldIO;
			if (attr.Packed && PackedFieldIO.TryCreate(property, out fieldIO))
				return new MessageFieldPacked(Create(attr, fieldIO));

			return Create(attr, CreateFieldIO(property));
		}
		#endregion

		#region Instance Methods
		public virtual void AppendWriteField(ILGenerator generator)
		{
			generator.Call<MessageWriter>("Write" + FieldType.Name, FieldType);
		}

		public virtual void AppendFieldLength(ILGenerator generator)
		{
			generator.Call<MessageWriter>("Length", FieldType);
		}

		public virtual void AppendReadField(ILGenerator generator)
		{
			generator.Call<MessageReader>("Read" + FieldType.Name);
		}

		public virtual void AppendGuard(ILGenerator generator, 
			MethodInfo getMethod, Label done)
		{
		}

		public virtual void AppendHeader(ILGenerator generator)
		{
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldc_I4, Header);
			generator.Call<MessageWriter>("WriteVarint", typeof(uint));
		}

		public void AppendWriteBody(ILGenerator generator)
		{
			_fieldIO.AppendWrite(generator, this);
		}

		public FieldReader<T> GetFieldReader<T>()
		{
			FieldReader<T> reader;
			if (CanAppendRead && _fieldIO.CreateReader<T>(this, out reader))
				return reader;

			throw new NotSupportedException();
		}
		#endregion
    }
}
