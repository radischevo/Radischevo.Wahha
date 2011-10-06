using System;
using System.Data;
using System.Security;
using System.Threading;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Provides common interface methods 
	/// for <see cref="Radischevo.Wahha.Data.IDbDataRecord"/> 
	/// implementations.
	/// </summary>
	[Serializable]
	public abstract class DbDataRecordBase
	{
		#region Constructors
		protected DbDataRecordBase()
		{
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the boxed value of the 
		/// column which has the specified name.
		/// </summary>
		/// <param name="name">The name of the 
		/// column to find.</param>
		public object this[string name]
		{
			get
			{
				return GetValue(name);
			}
		}

		/// <summary>
		/// Gets the boxed value of the column 
		/// which has the specified ordinal.
		/// </summary>
		/// <param name="ordinal">The zero-based 
		/// column ordinal.</param>
		public object this[int ordinal]
		{
			get
			{
				return GetValue(ordinal);
			}
		}
		#endregion

		#region Static Methods
		private static bool IsCatchableExceptionType(Exception exception)
		{
			return !(exception is StackOverflowException ||
				exception is OutOfMemoryException ||
				exception is ThreadAbortException ||
				exception is NullReferenceException ||
				exception is AccessViolationException ||
				exception is SecurityException);
		}

		private static long GetArray<T>(T[] value, long fieldOffset,
			T[] buffer, int bufferOffset, int length)
		{
			T[] sourceArray = value;
			int maxLength = sourceArray.Length;
			if (fieldOffset > 0x7fffffffL)
				throw Error.InvalidSourceBufferIndex(maxLength, fieldOffset, "fieldOffset");

			int sourceIndex = (int)fieldOffset;
			if (buffer != null)
			{
				try
				{
					if (sourceIndex < maxLength)
					{
						if (sourceIndex + length > maxLength)
							maxLength -= sourceIndex;
						else
							maxLength = length;
					}
					Array.Copy(sourceArray, sourceIndex, buffer, bufferOffset, maxLength);
				}
				catch (Exception exception)
				{
					if (IsCatchableExceptionType(exception))
					{
						maxLength = sourceArray.Length;
						if (length < 0)
							throw Error.InvalidDataLength(length);

						if (bufferOffset < 0 || bufferOffset >= buffer.Length)
							throw Error.InvalidDestinationBufferIndex(length, bufferOffset, "bufferOffset");

						if (fieldOffset < 0 || fieldOffset >= maxLength)
							throw Error.InvalidSourceBufferIndex(length, fieldOffset, "fieldOffset");

						if (maxLength + bufferOffset > buffer.Length)
							throw Error.InvalidBufferSizeOrIndex(maxLength, bufferOffset);
					}
					throw;
				}
			}
			return maxLength;
		}
		#endregion

		#region GetXxx By Name Methods
		/// <summary>
		/// Gets the value of the specified column 
		/// as a <see cref="System.Boolean"/>.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public bool GetBoolean(string name)
		{
			return (bool)GetValue(name);
		}

		/// <summary>
		/// Gets the 8-bit unsigned integer value of the specified column.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public byte GetByte(string name)
		{
			return (byte)GetValue(name);
		}

		/// <summary>
		/// Gets the characher value of the specified column.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public char GetChar(string name)
		{
			return (char)GetValue(name);
		}

		/// <summary>
		/// Gets the date and time data value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public DateTime GetDateTime(string name)
		{
			return (DateTime)GetValue(name);
		}

		/// <summary>
		/// Gets the fixed-position numeric 
		/// value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public decimal GetDecimal(string name)
		{
			return (decimal)GetValue(name);
		}

		/// <summary>
		/// Gets the double-precision floating point number 
		/// value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public double GetDouble(string name)
		{
			return (double)GetValue(name);
		}

		/// <summary>
		/// Gets the single-precision floating point number 
		/// value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public float GetFloat(string name)
		{
			return (float)GetValue(name);
		}

		/// <summary>
		/// Gets the GUID value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public Guid GetGuid(string name)
		{
			return (Guid)GetValue(name);
		}

		/// <summary>
		/// Gets the 16-bit signed integer 
		/// value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public short GetInt16(string name)
		{
			return (short)GetValue(name);
		}

		/// <summary>
		/// Gets the 32-bit signed integer 
		/// value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public int GetInt32(string name)
		{
			return (int)GetValue(name);
		}

		/// <summary>
		/// Gets the 64-bit signed integer 
		/// value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public long GetInt64(string name)
		{
			return (long)GetValue(name);
		}

		/// <summary>
		/// Gets the string  
		/// value of the specified column. 
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public string GetString(string name)
		{
			return (string)GetValue(name);
		}

		/// <summary>
		/// Reads a stream of bytes from the specified column offset 
		/// into the buffer as an array, starting at the given buffer offset.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <param name="fieldOffset">The index within the field from which 
		/// to start the read operation.</param>
		/// <param name="buffer">The buffer infto which to read the stream of bytes.</param>
		/// <param name="bufferOffset">The index for buffer to start the read operation.</param>
		/// <param name="length">The number of bytes to read.</param>
		public long GetBytes(string name, long fieldOffset,
			byte[] buffer, int bufferOffset, int length)
		{
			return GetArray<byte>((byte[])GetValue(name),
				fieldOffset, buffer, bufferOffset, length);
		}

		/// <summary>
		/// Reads a stream of characters from the specified column offset 
		/// into the buffer as an array, starting at the given buffer offset.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <param name="fieldOffset">The index within the field from which 
		/// to start the read operation.</param>
		/// <param name="buffer">The buffer infto which to read the stream of characters.</param>
		/// <param name="bufferOffset">The index for buffer to start the read operation.</param>
		/// <param name="length">The number of characters to read.</param>
		public long GetChars(string name, long fieldOffset,
			char[] buffer, int bufferOffset, int length)
		{
			return GetArray<char>(GetString(name).ToCharArray(),
				fieldOffset, buffer, bufferOffset, length);
		}

		/// <summary>
		/// When overridden in a derived class, gets the boxed value 
		/// of the result column which has the specified name.
		/// </summary>
		/// <param name="key">The name of the column to find.</param>
		public abstract object GetValue(string name);
		#endregion

		#region GetXxx By Ordinal Methods
		/// <summary>
		/// Gets the value of the specified column 
		/// as a <see cref="System.Boolean"/>.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public bool GetBoolean(int ordinal)
		{
			return (bool)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the 8-bit unsigned integer value of the specified column.
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public byte GetByte(int ordinal)
		{
			return (byte)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the characher value of the specified column.
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public char GetChar(int ordinal)
		{
			return (char)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the date and time data value of the specified column. 
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public DateTime GetDateTime(int ordinal)
		{
			return (DateTime)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the fixed-position numeric 
		/// value of the specified column. 
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public decimal GetDecimal(int ordinal)
		{
			return (decimal)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the double-precision floating point number 
		/// value of the specified column. 
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public double GetDouble(int ordinal)
		{
			return (double)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the single-precision floating point number 
		/// value of the specified column. 
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public float GetFloat(int ordinal)
		{
			return (float)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the GUID value of the specified column. 
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public Guid GetGuid(int ordinal)
		{
			return (Guid)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the 16-bit signed integer 
		/// value of the specified column. 
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public short GetInt16(int ordinal)
		{
			return (short)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the 32-bit signed integer 
		/// value of the specified column.
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public int GetInt32(int ordinal)
		{
			return (int)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the 64-bit signed integer 
		/// value of the specified column. 
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public long GetInt64(int ordinal)
		{
			return (long)GetValue(ordinal);
		}

		/// <summary>
		/// Gets the string  
		/// value of the specified column. 
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public string GetString(int ordinal)
		{
			return (string)GetValue(ordinal);
		}

		/// <summary>
		/// Reads a stream of bytes from the specified column offset 
		/// into the buffer as an array, starting at the given buffer offset.
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		/// <param name="fieldOffset">The index within the field from which 
		/// to start the read operation.</param>
		/// <param name="buffer">The buffer infto which to read the stream of bytes.</param>
		/// <param name="bufferOffset">The index for buffer to start the read operation.</param>
		/// <param name="length">The number of bytes to read.</param>
		public long GetBytes(int ordinal, long fieldOffset,
			byte[] buffer, int bufferOffset, int length)
		{
			return GetArray<byte>((byte[])GetValue(ordinal), 
				fieldOffset, buffer, bufferOffset, length);
		}

		/// <summary>
		/// Reads a stream of characters from the specified column offset 
		/// into the buffer as an array, starting at the given buffer offset.
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		/// <param name="fieldOffset">The index within the field from which 
		/// to start the read operation.</param>
		/// <param name="buffer">The buffer infto which to read the stream of characters.</param>
		/// <param name="bufferOffset">The index for buffer to start the read operation.</param>
		/// <param name="length">The number of characters to read.</param>
		public long GetChars(int ordinal, long fieldOffset,
			char[] buffer, int bufferOffset, int length)
		{
			return GetArray<char>(GetString(ordinal).ToCharArray(),
				fieldOffset, buffer, bufferOffset, length);
		}

		/// <summary>
		/// When overridden in a derived class, gets the boxed 
		/// value of the result column which has the specified ordinal.
		/// </summary>
		/// <param name="index">The zero-based column ordinal.</param>
		public abstract object GetValue(int ordinal);
		#endregion
	}
}
