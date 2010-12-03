using System;
using System.Text;

namespace Radischevo.Wahha.Core
{
	public static class StringBuilderExtensions
	{
		#region Helper Methods
		private static StringBuilder CreateBuilder(string value)
		{
			StringBuilder builder = new StringBuilder(value);
			builder.Append(value);
			
			return builder;
		}
		#endregion

		#region String Extension Methods
		/// <summary>
		/// Appends the string representation of a specified 
		/// Boolean value to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The Boolean value to append.</param>
		public static StringBuilder Append(this string argument, bool value)
		{
			return CreateBuilder(argument).Append(value);
		}

		/// <summary>
		/// Appends the string representation of a specified 
		/// 8-bit unsigned integer to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, byte value)
		{
			return CreateBuilder(argument).Append(value);
		}

		/// <summary>
		/// Appends the string representation of a specified Unicode 
		/// character to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The Unicode character to append.</param>
		public static StringBuilder Append(this string argument, char value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of the Unicode characters 
		/// in a specified array to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The array of characters to append.</param>
		public static StringBuilder Append(this string argument, char[] value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified 
		/// decimal number to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, decimal value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified double-precision 
		/// floating-point number to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, double value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified single-precision 
		/// floating-point number to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, float value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified 32-bit 
		/// signed integer to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, int value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified 64-bit 
		/// signed integer to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, long value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified object 
		/// to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The object to append.</param>
		public static StringBuilder Append(this string argument, object value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified 8-bit 
		/// signed integer to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, sbyte value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified 16-bit signed 
		/// integer to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, short value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends a copy of the specified string to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The <see cref="System.String"/> to append.</param>
		public static StringBuilder Append(this string argument, string value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified 32-bit 
		/// unsigned integer to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, uint value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified 64-bit 
		/// unsigned integer to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, ulong value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends the string representation of a specified 16-bit 
		/// unsigned integer to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder Append(this string argument, ushort value)
		{
			return CreateBuilder(argument).Append(value);
		}
		
		/// <summary>
		/// Appends a specified number of copies of the string representation 
		/// of a Unicode character to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The character to append.</param>
		/// <param name="repeatCount">The number of times to append value.</param>
		public static StringBuilder Append(this string argument, char value, int repeatCount)
		{
			return CreateBuilder(argument).Append(value, repeatCount);
		}
		
		/// <summary>
		/// Appends the string representation of a specified subarray of 
		/// Unicode characters to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">A character array.</param>
		/// <param name="startIndex">The starting position in value.</param>
		/// <param name="charCount">The number of characters to append.</param>
		public static StringBuilder Append(this string argument, char[] value, 
			int startIndex, int charCount)
		{
			return CreateBuilder(argument).Append(value, startIndex, charCount);
		}
		
		/// <summary>
		/// Appends a copy of a specified substring to the end of this instance.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="value">The System.String that contains the substring to append.</param>
		/// <param name="startIndex">The starting position of the substring within value.</param>
		/// <param name="count">The number of characters in value to append.</param>
		public static StringBuilder Append(this string argument, string value, 
			int startIndex, int count)
		{
			return CreateBuilder(argument).Append(value, startIndex, count);
		}

		/// <summary>
		/// Appends a formatted string, which contains zero or more format specifications, 
		/// to this instance. Each format specification is replaced by the string representation 
		/// of a corresponding object argument.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">An object to format.</param>
		public static StringBuilder AppendFormat(this string argument, string format, object arg0)
		{
			return CreateBuilder(argument).AppendFormat(format, arg0);
		}
		
		/// <summary>
		/// Appends a formatted string, which contains zero or more format specifications, 
		/// to this instance. Each format specification is replaced by the string representation 
		/// of a corresponding object argument.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An array of objects to format.</param>
		public static StringBuilder AppendFormat(this string argument, string format, params object[] args)
		{
			return CreateBuilder(argument).AppendFormat(format, args);
		}
	
		/// <summary>
		/// Appends a formatted string, which contains zero or more format specifications, 
		/// to this instance. Each format specification is replaced by the string representation 
		/// of a corresponding object argument.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies 
		/// culture-specific formatting information.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An array of objects to format.</param>
		public static StringBuilder AppendFormat(this string argument, IFormatProvider provider, 
			string format, params object[] args)
		{
			return CreateBuilder(argument).AppendFormat(provider, format, args);
		}
		
		/// <summary>
		/// Appends a formatted string, which contains zero or more format specifications, 
		/// to this instance. Each format specification is replaced by the string representation 
		/// of a corresponding object argument.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">The first object to format.</param>
		/// <param name="arg1">The second object to format.</param>
		public static StringBuilder AppendFormat(this string argument, string format, 
			object arg0, object arg1)
		{
			return CreateBuilder(argument).AppendFormat(format, arg0, arg1);
		}
		
		/// <summary>
		/// Appends a formatted string, which contains zero or more format specifications, 
		/// to this instance. Each format specification is replaced by the string representation 
		/// of a corresponding object argument.
		/// </summary>
		/// <param name="argument">The string argument to create 
		/// <see cref="System.Text.StringBuilder"/> from.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">The first object to format.</param>
		/// <param name="arg1">The second object to format.</param>
		/// <param name="arg2">The third object to format.</param>
		public static StringBuilder AppendFormat(this string argument, string format,
			object arg0, object arg1, object arg2)
		{
			return CreateBuilder(argument).AppendFormat(format, arg0, arg1, arg2);
		}
		#endregion
	}
}
