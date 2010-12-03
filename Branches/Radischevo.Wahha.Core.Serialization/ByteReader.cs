using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Radischevo.Wahha.Core.Serialization
{
	public class ByteReader : IByteReader
	{
		#region Instance Fields
		private Stream _stream;
		private byte[] _bytes = new byte[128];
		#endregion

		#region Constructors
		public ByteReader(Stream stream)
		{
			Precondition.Require(stream, () => 
				Error.ArgumentNull("stream"));

			_stream = stream;
		}
		#endregion

		#region Instance Properties
		public bool EndOfStream
		{
			get
			{
				return (_stream.Position == _stream.Length);
			}
		}
		#endregion

		#region Instance Methods
		private byte[] AllocateBytes(int length)
		{
			if (length < _bytes.Length)
				return _bytes;

			return new byte[length];
		}

		public byte GetByte()
		{
			return (byte)_stream.ReadByte();
		}

		public float GetSingle()
		{
			_stream.Read(_bytes, 0, sizeof(float));
			return BitConverter.ToSingle(_bytes, 0);
		}

		public ArraySegment<byte> GetBytes()
		{
			int bytesLeft = (int)(_stream.Length - _stream.Position);
			byte[] bytes = AllocateBytes(bytesLeft);
			_stream.Read(bytes, 0, bytesLeft);

			return new ArraySegment<byte>(bytes, 0, bytesLeft);
		}

		public ArraySegment<byte> GetBytes(int count)
		{
			byte[] bytes = AllocateBytes(count);
			_stream.Read(bytes, 0, count);

			return new ArraySegment<byte>(bytes, 0, count);
		}

		public IByteReader GetReader(int length)
		{
			byte[] bytes = AllocateBytes(length);
			_stream.Read(bytes, 0, length);

			return new ByteArrayReader(bytes, 0, length);
		}
		#endregion
	}
}
