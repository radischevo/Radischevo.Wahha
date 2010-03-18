using System;
using System.Security.Cryptography;

namespace Radischevo.Wahha.Core.Cryptography
{
	/// <summary>
	/// Provides the implementation of the Elf-32 hashing algorithm.
	/// </summary>
	public class Elf32 : HashAlgorithm
	{
		#region Instance Fields
		private uint _hash;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="T:Radischevo.Wahha.Core.Cryptography.Elf32"/> class.
		/// </summary>
		public Elf32()
		{
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
		private static uint CalculateHash(uint seed, byte[] buffer, int start, int size)
		{
			uint hash = seed;

			for (int i = start; i < size; i++)
				unchecked
				{
					hash = (hash << 4) + buffer[i];
					uint work = (hash & 0xf0000000);
					if (work != 0)
						hash ^= (work >> 24);
					hash &= ~work;
				}
			return hash;
		}

		/// <summary>
		/// Computes the hash value for the specified byte array.
		/// </summary>
		/// <param name="buffer">The input to compute the hash code for.</param>
		/// <returns>The computed hash code.</returns>
		public static uint Compute(uint polynomial, uint seed, byte[] buffer)
		{
			return CalculateHash(seed, buffer, 0, buffer.Length);
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
			_hash = CalculateHash(_hash, buffer, start, length);
		}

		/// <summary>
		/// When overridden in a derived class, finalizes the hash computation after
		/// the last data is processed by the cryptographic stream object.
		/// </summary>
		/// <returns>The computed hash code.</returns>
		protected override byte[] HashFinal()
		{
			byte[] hashBuffer = UInt32ToBigEndianBytes(_hash);
			this.HashValue = hashBuffer;
			return hashBuffer;
		}

		/// <summary>
		/// Initializes an implementation of the 
		/// <see cref="T:System.Security.Cryptography.HashAlgorithm"/> class.
		/// </summary>
		public override void Initialize()
		{
			_hash = 0;
		}
		#endregion
	}
}
