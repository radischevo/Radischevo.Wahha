using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public class ByteArrayReader : IByteReader
	{
		#region Instance Fields
		private byte[] _bytes;
		private int _offset;
		private int _end;
		#endregion

		#region Constructors
		public ByteArrayReader(byte[] bytes, int offset, int length)
		{
			Precondition.Require(bytes, () =>
				Error.ArgumentNull("bytes"));
			Precondition.Require(offset > -1, () =>
				Error.ParameterMustBeGreaterThanOrEqual("offset", 0, offset));
			Precondition.Require(length > -1, () =>
				Error.ParameterMustBeGreaterThanOrEqual("length", 0, length));
			Precondition.Require(offset < bytes.Length, () =>
				Error.OffsetMustBeLessThanArrayLength("offset", offset));
			Precondition.Require(offset + length <= bytes.Length,
				() => Error.ArgumentOutOfRange("length", length));

			_bytes = bytes;
			_offset = offset;
			_end = offset + length;
		}
		#endregion

		#region Instance Propeties
		public bool EndOfStream
		{
			get
			{
				return (_offset == _end);
			}
		}
		#endregion

		#region Instance Methods
		public byte GetByte()
		{
			return _bytes[_offset++];
		}

		public float GetSingle()
		{
			float value = BitConverter.ToSingle(_bytes, _offset);
			_offset += sizeof(float);

			return value;
		}

		public ArraySegment<byte> GetBytes()
		{
			return GetBytes(_end - _offset);
		}

		public ArraySegment<byte> GetBytes(int count)
		{
			ArraySegment<byte> segment = new ArraySegment<byte>(
				_bytes, _offset, count);
			_offset += count;

			return segment;
		}

		public IByteReader GetReader(int length)
		{
			ByteArrayReader reader = new ByteArrayReader(
				_bytes, _offset, length);
			_offset += length;

			return reader;
		}
		#endregion
	}
}