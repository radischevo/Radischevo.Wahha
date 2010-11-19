using System;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Provides additional extension methods 
    /// for the <see cref="System.Data.IDataRecord"/> and 
    /// <see cref="Radischevo.Wahha.Data.IDbDataRecord"/> interface 
    /// implementations.
    /// </summary>
    public static class DataRecordExtensions
    {
        #region Extended Conversion
        /// <summary>
        /// Converts the <see cref="System.Data.IDataRecord"/> to a case-sensitive dictionary.
        /// </summary>
        /// <param name="record">The record to convert.</param>
        public static IDictionary<string, object> ToDictionary(this IDataRecord record)
        {
            return ToDictionary(record, StringComparer.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="System.Data.IDataRecord"/> to a dictionary 
        /// with the specified key comparer.
        /// </summary>
        /// <param name="record">The record to convert.</param>
        /// <param name="comparer">The comparer used for comparing keys.</param>
        public static IDictionary<string, object> ToDictionary(
            this IDataRecord record, IEqualityComparer<string> comparer)
        {
            Precondition.Require(record, () => Error.ArgumentNull("record"));

            Dictionary<string, object> dictionary = 
                new Dictionary<string, object>(record.FieldCount, comparer);

            for (int i = 0; i < record.FieldCount; ++i)
            {
                string key = record.GetName(i);
                if (dictionary.ContainsKey(key))
                    throw Error.AmbiguousColumnName(key);

                dictionary.Add(key, record.GetValue(i));
            }
            return dictionary;
        }

        /// <summary>
        /// Converts the <see cref="System.Data.IDataRecord"/> to 
        /// an instance of the class that implements the 
        /// <see cref="Radischevo.Wahha.Data.IDbDataRecord"/> interface.
        /// </summary>
        public static IDbDataRecord ToDataRecord(this IDataRecord record)
        {
            return new DbDataRecord(record);
        }
        #endregion

        #region Generic GetValue extensions
        /// <summary>
        /// Gets the strongly-typed 
        /// value of the specified column. 
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="index">The zero-based column ordinal.</param>
        public static TValue GetValue<TValue>(this IDataRecord record, int index)
        {
            object value = record.GetValue(index);
            return Converter.ChangeType<TValue>(value, default(TValue));
        }

        /// <summary>
        /// Gets the strongly-typed 
        /// value of the specified column, or the specified 
        /// default value, if no column found or no conversion possible.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to return.</param>
        public static TValue GetValue<TValue>(this IDataRecord record,
            int index, TValue defaultValue)
        {
            object value = record.GetValue(index);
            return Converter.ChangeType<TValue>(value, defaultValue);
        }
        #endregion

        #region GetXxx for nullable value types by column index
        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Boolean}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Boolean}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Boolean"/> if possible.
        /// </remarks>
        public static bool? GetNullableBoolean(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetBoolean(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Byte}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Byte}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Byte"/> if possible.
        /// </remarks>
        public static byte? GetNullableByte(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetByte(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Char}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Char}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Char"/> if possible.
        /// </remarks>
        public static char? GetNullableChar(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetChar(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{DateTime}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{DateTime}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="DateTime"/> if possible.
        /// </remarks>
        public static DateTime? GetNullableDateTime(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetDateTime(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Decimal}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Decimal}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Decimal"/> if possible.
        /// </remarks>
        public static decimal? GetNullableDecimal(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetDecimal(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Double}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Double}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Double"/> if possible.
        /// </remarks>
        public static double? GetNullableDouble(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetDouble(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Single}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Single}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Single"/> if possible.
        /// </remarks>
        public static float? GetNullableFloat(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetFloat(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Guid}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Guid}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Guid"/> if possible.
        /// </remarks>
        public static Guid? GetNullableGuid(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetGuid(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Int16}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Int16}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Int16"/> if possible.
        /// </remarks>
        public static short? GetNullableInt16(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetInt16(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Int32}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Int32}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Int32"/> if possible.
        /// </remarks>
        public static int? GetNullableInt32(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetInt32(index);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Int64}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Int64}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Int64"/> if possible.
        /// </remarks>
        public static long? GetNullableInt64(this IDbDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetInt64(index);
        }
        #endregion

        #region GetXxx for nullable value types by column name
        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Boolean}"/>. 
        /// </summary>
        /// <param name="record">The data record.</param>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Boolean}"/>.</returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Boolean"/> if possible.
        /// </remarks>
        public static bool? GetNullableBoolean(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetBoolean(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Byte}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Byte}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Byte"/> if possible.
        /// </remarks>
        public static byte? GetNullableByte(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetByte(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Char}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Char}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Char"/> if possible.
        /// </remarks>
        public static char? GetNullableChar(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetChar(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{DateTime}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{DateTime}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="DateTime"/> if possible.
        /// </remarks>
        public static DateTime? GetNullableDateTime(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetDateTime(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Decimal}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Decimal}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Decimal"/> if possible.
        /// </remarks>
        public static decimal? GetNullableDecimal(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetDecimal(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Double}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Double}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Double"/> if possible.
        /// </remarks>
        public static double? GetNullableDouble(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetDouble(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Single}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Single}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Single"/> if possible.
        /// </remarks>
        public static float? GetNullableFloat(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetFloat(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Guid}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Guid}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Guid"/> if possible.
        /// </remarks>
        public static Guid? GetNullableGuid(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetGuid(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Int16}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Int16}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Int16"/> if possible.
        /// </remarks>
        public static short? GetNullableInt16(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetInt16(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Int32}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Int32}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Int32"/> if possible.
        /// </remarks>
        public static int? GetNullableInt32(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetInt32(name);
        }

        /// <summary>
        /// Returns the value in the given column converted to <see cref="Nullable{Int64}"/> 
        /// </summary>
        /// <param name="record">The data record</param>
        /// <param name="name">The name of the column</param>
        /// <returns>The value in the given column converted to <see cref="Nullable{Int64}"/> </returns>
        /// <remarks>
        /// If the value in the column is <see cref="DBNull"/>, <c>null</c> is returned. Otherwise the value is 
        /// converted to a <see cref="Int64"/> if possible.
        /// </remarks>
        public static long? GetNullableInt64(this IDbDataRecord record, string name)
        {
            if (record.IsDBNull(name))
                return null;

            return record.GetInt64(name);
        }
        #endregion
    }
}
