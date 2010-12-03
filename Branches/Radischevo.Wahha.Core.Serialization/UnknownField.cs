using System;
using Radischevo.Wahha.Core.Serialization.Fields;

namespace Radischevo.Wahha.Core.Serialization
{
	public abstract class UnknownField
	{
		#region Instance Fields
		private MessageTag _tag;
		private object _value;
		#endregion

		#region Constructors
		protected UnknownField(MessageTag tag, object value)
		{
			_tag = tag;
			_value = value;
		}
		#endregion

		#region Instance Properties
		public int Number
		{
			get
			{
				return _tag.Number;
			}
		}

		public WireType WireType
		{
			get
			{
				return _tag.WireType;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
		}
		#endregion

		#region Static Methods
		public static UnknownField Create(MessageTag tag, MessageReader reader)
		{
			switch (tag.WireType)
			{
				case WireType.Fixed64:
					return new UnknownFieldFixed64(tag, reader);
				case WireType.String:
					return new UnknownFieldString(tag, reader);
				case WireType.StartGroup:
					return new UnknownFieldGroup(tag, reader);
				case WireType.Fixed32:
					return new UnknownFieldFixed32(tag, reader);
			}
			return new UnknownFieldVarint(tag, reader.ReadInt64());
		}
		#endregion

		#region Instance Methods
		public void Serialize(MessageWriter writer)
		{
			SerializeCore(writer.WriteVarint(_tag.Value));
		}

		protected abstract void SerializeCore(MessageWriter writer);
		#endregion
	}
}
