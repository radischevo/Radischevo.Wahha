﻿using System;

namespace Radischevo.Wahha.Web.Scripting.Serialization
{
    public abstract class JavaScriptTypeResolver
    {
        #region Constructors
        protected JavaScriptTypeResolver()
        {
        }
        #endregion

        #region Instance Methods
        public abstract Type ResolveType(string typeId);

        public abstract string ResolveTypeId(Type type);
        #endregion
    }
}
