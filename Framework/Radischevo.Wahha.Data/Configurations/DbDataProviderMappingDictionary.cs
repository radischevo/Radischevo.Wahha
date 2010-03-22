using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Data.Configurations
{
    public class DbDataProviderMappingDictionary : Dictionary<string, Type>
    {
        #region Instance Fields
        private Type _default;
        #endregion

        #region Constructors
        public DbDataProviderMappingDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
        #endregion

        #region Instance Properties
        public Type Default
        {
            get
            {
                if (_default == null)
                    _default = Values.FirstOrDefault();

                return _default;
            }
            set
            {
                _default = value;
            }
        }
        #endregion
    }
}
