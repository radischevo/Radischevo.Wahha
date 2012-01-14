using System;
using System.Collections.ObjectModel;

namespace Radischevo.Wahha.Data
{
	[Serializable]
    public class DbParameterCollection : Collection<DbParameterDescriptor>
    {
        #region Constructors
        public DbParameterCollection()
            : base()
        {
        }
        #endregion
	}
}
