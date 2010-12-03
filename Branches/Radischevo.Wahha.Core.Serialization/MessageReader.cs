using System;
using System.IO;
using System.Text;

using Radischevo.Wahha.Core.Serialization.Fields;

namespace Radischevo.Wahha.Core.Serialization
{
	public class MessageReader
	{
		#region Instance Fields
		private IByteReader _bytes;
		#endregion

		#region Constructors
		public MessageReader(Stream stream)
			: this(new ByteReader(stream))
		{
		}

		public MessageReader(IByteReader bytes)
		{
			Precondition.Require(bytes, () => 
				Error.ArgumentNull("bytes"));

			_bytes = bytes;
		}

		public MessageReader(params byte[] message)
			: this(new ByteArrayReader(message, 0, message.Length))
		{
		}
		#endregion

		#region Instance Properties
		public bool EndOfStream
		{
			get
			{
				return _bytes.EndOfStream;
			}
		}
		#endregion

		#region Static Methods
		public static T Read<T>(byte[] message) 
			where T : class, new()
		{
			return new Serializer<T>().Deserialize(
				new MessageReader(message), 
				new T(), new UnknownFieldCollection()
			);
		}
		#endregion

		#region Instance Methods
		internal MessageReader CreateSubReader()
		{
			int length = ReadInt32();
			return new MessageReader(_bytes.GetReader(length));
		}

		internal T Read<T>() 
			where T : class, new()
		{
			return new Serializer<T>().Deserialize(
				this, new T(), 
				new UnknownFieldCollection()
			);
		}

		public bool ReadBoolean()
		{
			return (ReadInt32() != 0);
		}

		public int ReadInt32()
		{
			int bits, value = _bytes.GetByte();
			if (value < 0x80)
				return value;

			int shiftBits = 7;
			value &= 0x7f;
			do
			{
				bits = _bytes.GetByte();
				if (shiftBits < 31)
				{
					value |= (bits & 0x7F) << shiftBits;
					shiftBits += 7;
				}
			} 
			while (bits > 0x7F);
			return value;
		}

		public uint ReadUInt32()
		{
			return (uint)ReadInt32();
		}

		public long ReadInt64()
		{
			long value = 0L;
			int shiftBits = 0;
			long bits;
			do
			{
				bits = _bytes.GetByte();
				value |= (bits & 0x7F) << shiftBits;
				shiftBits += 7;
			} 
			while (bits > 0x7F && shiftBits < 64);
			return value;
		}

		public ulong ReadUInt64()
		{
			return (ulong)ReadInt64();
		}

		public int ReadZigZag32()
		{
			uint value = (uint)ReadInt32();
			uint mask = 0 - (value & 1);

			return (int)(value >> 1 ^ mask);
		}

		public Int64 ReadZigZag64()
		{
			ulong value = (ulong)ReadInt64();
			ulong mask = (0L - (value & 1));

			return (long)(value >> 1 ^ mask);
		}

		public int ReadFixedInt32()
		{
			return _bytes.GetByte()
				| _bytes.GetByte() << 8
				| _bytes.GetByte() << 16
				| _bytes.GetByte() << 24;
		}

		public uint ReadFixedUInt32()
		{
			return (uint)ReadFixedInt32();
		}

		public Int64 ReadFixedInt64()
		{
			long lo = (long)ReadFixedUInt32();
			long hi = (long)ReadFixedUInt32();

			return lo | (hi << 32);
		}

		public UInt64 ReadFixedUInt64()
		{
			return (UInt64)ReadFixedInt64();
		}

		public float ReadFixedSingle()
		{
			return _bytes.GetSingle();
		}

		public double ReadFixedDouble()
		{
			return BitConverter.Int64BitsToDouble(ReadFixedInt64());
		}

		public string ReadString()
		{
			ArraySegment<byte> bytes = _bytes.GetBytes();
			return Encoding.UTF8.GetString(bytes.Array, 
				bytes.Offset, bytes.Count);
		}

		public byte[] ReadBytes()
		{
			ArraySegment<byte> bytes = _bytes.GetBytes(ReadInt32());
			byte[] value = new byte[bytes.Count];
			Array.Copy(bytes.Array, bytes.Offset, value, 0, bytes.Count);

			return value;
		}

		public DateTime ReadDateTime()
		{
			return UnixTime.ToDateTime(ReadZigZag64());
		}

		public Decimal ReadDecimal()
		{
			return new Decimal(ReadInt64()) / DecimalField.Factor;
		}

		public MessageTag ReadMessageTag()
		{
			return new MessageTag(ReadInt32());
		}

		public bool TryReadMessageTag(ref MessageTag target)
		{
			if (_bytes.EndOfStream)
				return false;

			target = ReadMessageTag();
			return true;
		}

		public int ReadEnum(Type enumType)
		{
			int value = ReadInt32();
			if (!Enum.IsDefined(enumType, value))
				throw new UnknownEnumException(value);

			return value;
		}
		#endregion
	}
}
