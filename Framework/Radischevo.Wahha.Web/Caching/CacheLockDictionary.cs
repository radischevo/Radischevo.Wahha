using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Caching
{
    internal class CacheLockDictionary : ReaderWriterCache<string, object>
    {
        #region Constructors
        public CacheLockDictionary()
            : base()
        {   }
        #endregion

        #region Instance Methods
        internal object Get(string key)
        {
            return base.GetOrCreate(key, () => new object());
        }
        #endregion
    }
}
