using System;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Provides an extended access to the column values within 
    /// each row for a DataReader, based on the 
    /// <see cref="System.Data.IDataRecord"/> interface.
    /// </summary>
    public interface IDbDataRecord : IDataRecord, IDbValueSet
    {
        #region Instance Methods
        /// <summary>
        /// Returns a value indicating whether the column 
        /// with the specified name exists in the current 
        /// value set.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        bool ContainsKey(string key);

        /// <summary>
        /// Determines whether the current record 
        /// contains at least one of the listed fields.
        /// </summary>
        /// <param name="keys">An array, containing 
        /// the names of the fields to find.</param>
        bool ContainsAny(params string[] keys);

        /// <summary>
        /// Determines whether the current record 
        /// contains all of the listed fields.
        /// </summary>
        /// <param name="keys">An array, containing 
        /// the names of the fields to find.</param>
        bool ContainsAll(params string[] keys);
        #endregion

        #region GetXxx By Column Name Methods
        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        bool GetBoolean(string name);
        
        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        byte GetByte(string name);
        
        /// <summary>
        /// Reads a stream of bytes from the specified column 
        /// offset into the buffer as an array, starting 
        /// at the given buffer offset.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <param name="fieldOffset">The index within the field from 
        /// which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to 
        /// read the stream of bytes.</param>
        /// <param name="bufferOffset">The index for buffer 
        /// to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        long GetBytes(string name, long fieldOffset, byte[] buffer, 
            int bufferOffset, int length);
        
        /// <summary>
        /// Gets the character value of the 
        /// specified column.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        char GetChar(string name);

        /// <summary>
        /// Return whether the specified field 
        /// is set to null.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        bool IsDBNull(string name);

        /// <summary>
        /// Reads a stream of characters from the specified 
        /// column offset into the buffer as an array, starting 
        /// at the given buffer offset.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <param name="fieldOffset">The index within the row 
        /// from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read 
        /// the stream of bytes.</param>
        /// <param name="bufferOffset">The index for buffer 
        /// to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        long GetChars(string name, long fieldOffset, char[] buffer, 
            int bufferOffset, int length);
        
        /// <summary>
        /// Returns an <see cref="System.Data.IDataReader"/> 
        /// for the specified column name.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        IDataReader GetData(string name);
        
        /// <summary>
        /// Gets the data type information for 
        /// the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        string GetDataTypeName(string name);
        
        /// <summary>
        /// Gets the date and time data value 
        /// of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        DateTime GetDateTime(string name);
        
        /// <summary>
        /// Gets the fixed-position numeric value 
        /// of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        decimal GetDecimal(string name);
        
        /// <summary>
        /// Gets the double-precision floating 
        /// point number of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        double GetDouble(string name);
        
        /// <summary>
        /// Gets the <see cref="T:System.Type"/> information corresponding 
        /// to the type of <see cref="T:System.Object"/> that would be 
        /// returned from <see cref="M:System.Data.IDataRecord.GetValue(System.String)"/>.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        Type GetFieldType(string name);
        
        /// <summary>
        /// Gets the single-precision floating point 
        /// number of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        float GetFloat(string name);
        
        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        Guid GetGuid(string name);
        
        /// <summary>
        /// Gets the 16-bit signed integer value 
        /// of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        short GetInt16(string name);
        
        /// <summary>
        /// Gets the 32-bit signed integer 
        /// value of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        int GetInt32(string name);
        
        /// <summary>
        /// Gets the 64-bit signed integer 
        /// value of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        long GetInt64(string name);
        
        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        string GetString(string name);

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        object GetValue(string name);
        #endregion
    }
}
