using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Caching
{
    internal sealed class CacheLockDictionary : ReaderWriterCache<string, object>
    {
        #region Constructors
        public CacheLockDictionary()
            : base()
        {   }
        #endregion

        #region Instance Methods
        public object Get(string key)
        {
            return base.GetOrCreate(key, () => new object());
        }
        #endregion
    }
}
