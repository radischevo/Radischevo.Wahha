using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Permissions;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Serves as the base class for classes that enables information 
    /// to be shared across multiple sessions and requests within an ASP.NET application. 
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpApplicationStateBase : NameObjectCollectionBase, ICollection
    {
        #region Constructors
        /// <summary>
        /// Initializes the class for use by an inherited class 
        /// instance. This constructor can only be called by 
        /// an inherited class.
        /// </summary>
        protected HttpApplicationStateBase()
        {
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// When overridden in a derived class, gets the 
        /// access keys for the objects in the collection.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual IEnumerable<string> AllKeys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a reference 
        /// to the <see cref="T:Radischevo.Wahha.Web.Abstractions.HttpApplicationStateBase" /> object. 
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual HttpApplicationStateBase Contents
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets 
        /// the number of objects in the collection.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public override int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value 
        /// that indicates whether access to the collection is thread-safe.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual bool IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, 
        /// gets the value of a state object by name.
        /// </summary>
        /// <param name="name">The name of the object in the collection.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
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

        /// <summary>
        /// When overridden in a derived class, 
        /// gets a state object by index.
        /// </summary>
        /// <param name="index">The index of the object in the collection.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets all objects 
        /// that are declared by an object element where the scope 
        /// is set to "Application" within the ASP.NET application. 
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual HttpStaticObjectsCollectionBase StaticObjects
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets an 
        /// object that can be used to synchronize 
        /// access to the collection.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// When overridden in a derived class, adds 
        /// a new object to the collection. 
        /// </summary>
        /// <param name="name">The name of the object 
        /// to add to the collection.</param>
        /// <param name="value">The value of the object.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void Add(string name, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, 
        /// removes all objects from the collection.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, copies the 
        /// elements of the collection to an array, 
        /// starting at the specified index in the array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination 
        /// for the elements that are copied from the collection. 
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which to begin copying.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, 
        /// gets a state object by index. 
        /// </summary>
        /// <param name="index">The index of the 
        /// application state object.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual object Get(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, gets a 
        /// state object by name. 
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual object Get(string name)
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
            return Converter.ChangeType<TValue>(Get(key), defaultValue);
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
            return Converter.ChangeType<TValue>(Get(index), defaultValue);
        }

        /// <summary>
        /// When overridden in a derived class, returns an 
        /// enumerator that can be used to 
        /// iterate through the collection.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, 
        /// gets the name of a state object by index. 
        /// </summary>
        /// <param name="index">The index of the application state object.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual string GetKey(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, 
        /// locks access to objects in the collection 
        /// in order to enable synchronized access.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void Lock()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, removes 
        /// the named object from the collection. 
        /// </summary>
        /// <param name="name">The name of the object 
        /// to remove from the collection.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void Remove(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, 
        /// removes all objects from the collection.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void RemoveAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, removes a 
        /// state object from the collection by index. 
        /// </summary>
        /// <param name="index">The position in the 
        /// collection of the item to remove. </param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, updates 
        /// the value of an object in the collection. 
        /// </summary>
        /// <param name="name">The name of the object to update.</param>
        /// <param name="value">The updated value of the object.</param>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void Set(string name, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, unlocks 
        /// access to objects in the collection to 
        /// enable synchronized access.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException" condition="Always." />
        public virtual void UnLock()
        {
            throw new NotImplementedException();
        }                       
        #endregion
    }
}
