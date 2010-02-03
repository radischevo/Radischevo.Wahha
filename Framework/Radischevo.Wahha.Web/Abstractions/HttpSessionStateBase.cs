using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Permissions;
using System.Web;
using System.Web.SessionState;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Serves as the base class for classes that provides access to 
    /// session-state values, session-level settings, and lifetime 
    /// management methods.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpSessionStateBase : ICollection, IEnumerable
    {
        #region Constructors
        /// <summary>
        /// Initializes the class for use by an inherited class instance. 
        /// This constructor can only be called by an inherited class. 
        /// </summary>
        protected HttpSessionStateBase()
        {
        }
        #endregion

        #region Instance Properties
        public virtual int CodePage
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpSessionStateBase Contents
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpCookieMode CookieMode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsCookieless
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsNewSession
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
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual NameObjectCollectionBase.KeysCollection Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual int LCID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual SessionStateMode Mode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string SessionID
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpStaticObjectsCollectionBase StaticObjects
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

        public virtual int Timeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Instance Methods
        public virtual void Abandon()
        {
            throw new NotImplementedException();
        }

        public virtual void Add(string name, object value)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the typed value with the 
        /// specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="key">The key to find.</param>
        public TValue GetValue<TValue>(string key)
        {
            return GetValue<TValue>(key, default(TValue));
        }

        /// <summary>
        /// Gets the typed value with the 
        /// specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="key">The key to find.</param>
        /// <param name="defaultValue">The default value of the variable.</param>
        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            return Converter.ChangeType<TValue>(this[key], defaultValue);
        }

        /// <summary>
        /// Gets the typed value with the specified index.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="key">The index of the element to find.</param>
        public TValue GetValue<TValue>(int index)
        {
            return GetValue<TValue>(index, default(TValue));
        }

        /// <summary>
        /// Gets the typed value with the specified index.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="key">The index of the element to find.</param>
        /// <param name="defaultValue">The default value of the variable.</param>
        public TValue GetValue<TValue>(int index, TValue defaultValue)
        {
            return Converter.ChangeType<TValue>(this[index], defaultValue);
        }

        public virtual void Remove(string name)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
