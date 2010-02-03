using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Provides a means of reading one or more forward-only streams of result sets
    /// obtained by executing a command at a data source, and is implemented by .NET
    /// Framework data providers that access relational databases.
    /// </summary>
    public interface IDbDataReader : IDataReader, IDbDataRecord, 
        IEnumerable<IDbDataRecord>, IEnumerable
    {
    }
}
