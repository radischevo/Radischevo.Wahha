using System;
using System.IO;
using System.Text;

using Radischevo.Wahha.Core.Serialization.Fields;

namespace Radischevo.Wahha.Core.Serialization
{
	public class MessageWriter
	{
		#region Instance Fields
		private Stream _writer;
		private IObjectWriterStrategy _strategy;
		#endregion

		#region Constructors
		public MessageWriter(Stream output, IObjectWriterStrategy strategy)
		{
			_strategy = strategy;
			_writer = output;
		}

		public MessageWriter(Stream output)
			: this(output, new ObjectWriterStrategy())
		{
		}
		#endregion

		#region Static Methods
		public static byte[] Write<T>(T message)
		{
			using (MemoryStream output = new MemoryStream())
			{
				Serializer.Serialize(new MessageWriter(output), message);
				return output.ToArray();
			}
		}
		#endregion

		#region Helper Methods
		private void Write(byte value)
		{
			_writer.WriteByte(value);
		}

		private void Write(ushort value)
		{
			_writer.WriteByte((byte)(value & 0xff));
			_writer.WriteByte((byte)(value >> 8));
		}

		private MessageWriter WriteRaw(byte[] bytes)
		{
			_writer.Write(bytes, 0, bytes.Length);
			return this;
		}
		#endregion

		#region Instance Methods
		public int Length(int value)
		{
			return Length((long)value);
		}

		public int Length(long value)
		{
			return Length((ulong)value);
		}

		public int Length(ulong value)
		{
			int length = 0;
			do
			{
				++length;
				value >>= 7;
			}
			while (value != 0);
			return length;
		}

		public MessageWriter WriteVarint(int value)
		{
			if (value < 0)
				return WriteVarint((ulong)value);

			return WriteVarint((uint)value);
		}

		public MessageWriter WriteVarint(uint value)
		{
			if (value < 0x80)
				Write((byte)value);

			else if (value < (1 << 14))
			{
				uint low = value & 0x7f;
				uint hi = value & 0x3F80;
				Write((ushort)(hi << 1 | low | 0x80));
			}
			else
			{
				do
				{
					byte bits = (byte)(value & 0x7f);
					value >>= 7;

					if (value > 0)
						bits |= 0x80;

					Write(bits);
				} 
				while (value != 0);
			}
			return this;
		}

		public MessageWriter WriteVarint(long value)
		{
			return WriteVarint((ulong)value);
		}

		public MessageWriter WriteVarint(ulong value)
		{
			do
			{
				byte bits = (byte)(value & 0x7f);
				value >>= 7;
				
				if (value > 0)
					bits |= 0x80;

				Write(bits);
			} 
			while (value != 0);
			return this;
		}

		public MessageWriter WriteVarint(bool value)
		{
			return WriteVarint(Convert.ToInt32(value));
		}

		public MessageWriter WriteZigZag(int value)
		{
			return WriteVarint(value << 1 ^ value >> 31);
		}

		public MessageWriter WriteZigZag(long value)
		{
			return WriteVarint(value << 1 ^ value >> 63);
		}

		public MessageWriter WriteString(string value)
		{
			if (value == null)
				return this;

			return WriteBytes(Encoding.UTF8.GetBytes(value));
		}

		public MessageWriter WriteBytes(byte[] value)
		{
			return WriteBytes(value, value.Length);
		}

		public MessageWriter WriteBytes(byte[] value, int length)
		{
			WriteVarint(length);
			_writer.Write(value, 0, length);

			return this;
		}

		public MessageWriter WriteFixed(int value)
		{
			return WriteRaw(BitConverter.GetBytes(value));
		}

		public MessageWriter WriteFixed(uint value)
		{
			return WriteRaw(BitConverter.GetBytes(value));
		}

		public MessageWriter WriteFixed(float value)
		{
			return WriteRaw(BitConverter.GetBytes(value));
		}

		public MessageWriter WriteFixed(long value)
		{
			return WriteRaw(BitConverter.GetBytes(value));
		}

		public MessageWriter WriteFixed(ulong value)
		{
			return WriteRaw(BitConverter.GetBytes(value));
		}

		public MessageWriter WriteFixed(double value)
		{
			return WriteRaw(BitConverter.GetBytes(value));
		}

		public MessageWriter WriteDateTime(DateTime date)
		{
			return WriteZigZag(UnixTime.From(date));
		}

		public MessageWriter WriteDecimal(Decimal value)
		{
			return WriteVarint(Decimal.ToInt64(value * DecimalField.Factor));
		}

		public MessageWriter WriteObject<T>(T obj, int number) 
			where T : class
		{
			_strategy.Write<T>(this, number, obj);
			return this;
		}

		public MessageWriter WriteHeader(int tag, WireType wireType)
		{
			return WriteVarint(new MessageTag(tag, wireType).Value);
		}
		#endregion
	}
}