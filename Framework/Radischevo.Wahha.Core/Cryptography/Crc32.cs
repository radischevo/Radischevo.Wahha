using System;
using System.Security.Cryptography;

namespace Radischevo.Wahha.Core.Cryptography
{
	/// <summary>
	/// Provides the implementation of the CRC32 hashing algorithm.
	/// </summary>
	public class Crc32 : HashAlgorithm
	{
		#region Constants
		public const uint DefaultPolynomial = 0xedb88320;
		public const uint DefaultSeed = 0xffffffff;
		#endregion

		#region Static Fields
		private static uint[] _defaultTable;
		#endregion

		#region Instance Fields
		private uint _hash;
		private uint _seed;
		private uint[] _table;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="T:Radischevo.Wahha.Core.Cryptography.Crc32"/> class.
		/// </summary>
		public Crc32()
		{
			_table = InitializeTable(DefaultPolynomial);
			_seed = DefaultSeed;

			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="T:Radischevo.Wahha.Core.Cryptography.Crc32"/> class.
		/// </summary>
		public Crc32(uint polynomial, uint seed)
		{
			_table = InitializeTable(polynomial);
			_seed = seed;

			Initialize();
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the size, in bits, of the computed hash code.
		/// </summary>
		public override int HashSize
		{
			get
			{
				return 32;
			}
		}
		#endregion

		#region Static Methods
		private static uint[] InitializeTable(uint polynomial)
		{
			if (polynomial == DefaultPolynomial && _defaultTable != null)
				return _defaultTable;

			uint[] createTable = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint entry = (uint)i;
				for (int j = 0; j < 8; j++)
					if ((entry & 1) == 1)
						entry = (entry >> 1) ^ polynomial;
					else
						entry = entry >> 1;
				createTable[i] = entry;
			}

			if (polynomial == DefaultPolynomial)
				_defaultTable = createTable;

			return createTable;
		}

		private static uint CalculateHash(uint[] table, uint seed, byte[] buffer, int start, int size)
		{
			uint crc = seed;
			for (int i = start; i < size; i++)
				unchecked
				{
					crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
				}
			return crc;
		}

		/// <summary>
		/// Computes the hash value for the specified byte array.
		/// </summary>
		/// <param name="buffer">The input to compute the hash code for.</param>
		/// <returns>The computed hash code.</returns>
		public static uint Compute(byte[] buffer)
		{
			return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Computes the hash value for the specified byte array.
		/// </summary>
		/// <param name="buffer">The input to compute the hash code for.</param>
		/// <returns>The computed hash code.</returns>
		public static uint Compute(uint seed, byte[] buffer)
		{
			return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Computes the hash value for the specified byte array.
		/// </summary>
		/// <param name="buffer">The input to compute the hash code for.</param>
		/// <returns>The computed hash code.</returns>
		public static uint Compute(uint polynomial, uint seed, byte[] buffer)
		{
			return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
		}
		#endregion

		#region Instance Methods
		private byte[] UInt32ToBigEndianBytes(uint x)
		{
			return new byte[] {
				(byte)((x >> 24) & 0xff),
				(byte)((x >> 16) & 0xff),
				(byte)((x >> 8) & 0xff),
				(byte)(x & 0xff)
			};
		}

		/// <summary>
		/// When overridden in a derived class, routes data written to the object into
		/// the hash algorithm for computing the hash.
		/// </summary>
		/// <param name="buffer">The input to compute the hash code for.</param>
		/// <param name="start">The offset into the byte array from which to begin using data.</param>
		/// <param name="length">The number of bytes in the byte array to use as data.</param>
		protected override void HashCore(byte[] buffer, int start, int length)
		{
			_hash = CalculateHash(_table, _hash, buffer, start, length);
		}

		/// <summary>
		/// When overridden in a derived class, finalizes the hash computation after
		/// the last data is processed by the cryptographic stream object.
		/// </summary>
		/// <returns>The computed hash code.</returns>
		protected override byte[] HashFinal()
		{
			byte[] hashBuffer = UInt32ToBigEndianBytes(~_hash);
			HashValue = hashBuffer;

			return hashBuffer;
		}

		/// <summary>
		/// Initializes an implementation of the 
		/// <see cref="T:System.Security.Cryptography.HashAlgorithm"/> class.
		/// </summary>
		public override void Initialize()
		{
			_hash = _seed;
		}
		#endregion
	}
}
