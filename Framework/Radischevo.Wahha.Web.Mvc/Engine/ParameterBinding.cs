using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class ParameterBinding
    {
        #region Constructors
        protected ParameterBinding()
        {   }
        #endregion

        #region Instance Properties
        public virtual IModelBinder Binder
        {
            get
            {
                return null;
            }
        }

        public virtual string Name
        {
            get
            {
                return null;
            }
        }

        public virtual ParameterSource Source
        {
            get
            {
                return ParameterSource.Default;
            }
        }

        public virtual object DefaultValue
        {
            get
            {
                return null;
            }
        }

        public virtual IEnumerable<string> Include
        {
            get
            {
                return new string[0];
            }
        }

        public virtual IEnumerable<string> Exclude
        {
            get
            {
                return new string[0];
            }
        }
        #endregion

        #region Instance Methods
        public virtual Predicate<string> GetMemberFilter()
        {
            return null;
        }
        #endregion
    }
}
