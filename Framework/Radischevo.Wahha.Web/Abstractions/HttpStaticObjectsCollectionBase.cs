using System;
using System.Collections;
using System.IO;
using System.Security.Permissions;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Serves as the base class for classes that provide a collection of application-scoped objects for the 
    /// <see cref="P:System.Web.HttpApplicationState.StaticObjects"/> property.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpStaticObjectsCollectionBase : ICollection, IEnumerable
    {
        #region Constructors
        /// <summary>
        /// Initializes the class for use by an inherited class instance. 
        /// This constructor can only be called by an inherited class.
        /// </summary>
        protected HttpStaticObjectsCollectionBase()
        {
        }
        #endregion

        #region Instance Properties
        public virtual int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual object this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool NeverAccessed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Instance Methods
        public virtual void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public virtual object GetObject(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the typed value with the 
        /// specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="key">The key to find.</param>
        public TValue GetObject<TValue>(string key)
        {
            return GetObject<TValue>(key, default(TValue));
        }

        /// <summary>
        /// Gets the typed value with the 
        /// specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="key">The key to find.</param>
        /// <param name="defaultValue">The default value of the variable.</param>
        public TValue GetObject<TValue>(string key, TValue defaultValue)
        {
            return Converter.ChangeType<TValue>(GetObject(key), defaultValue);
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
