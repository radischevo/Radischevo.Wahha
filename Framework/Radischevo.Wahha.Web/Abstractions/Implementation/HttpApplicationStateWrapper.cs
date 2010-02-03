using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that enables information to be 
    /// shared across multiple requests and sessions within an ASP.NET application.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpApplicationStateWrapper : HttpApplicationStateBase
    {
        #region Instance Fields
        private readonly HttpApplicationState _application;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:Radischevo.Wahha.Web.Abstractions.HttpApplicationStateWrapper" /> class. 
        /// </summary>
        /// <param name="application">The object that this wrapper class provides access to.</param>
        public HttpApplicationStateWrapper(HttpApplicationState application)
        {
            Precondition.Require(application, Error.ArgumentNull("application"));
            _application = application;
        }
        #endregion

        #region Instance Properties
        public override IEnumerable<string> AllKeys
        {
            get
            {
                return _application.AllKeys;
            }
        }

        public override HttpApplicationStateBase Contents
        {
            get
            {
                return this;
            }
        }

        public override int Count
        {
            get
            {
                return _application.Count;
            }
        }

        public override bool IsSynchronized
        {
            get
            {
                return ((ICollection)_application).IsSynchronized;
            }
        }

        public override object this[int index]
        {
            get
            {
                return _application[index];
            }
        }

        public override object this[string name]
        {
            get
            {
                return _application[name];
            }
            set
            {
                _application[name] = value;
            }
        }

        public override NameObjectCollectionBase.KeysCollection Keys
        {
            get
            {
                return _application.Keys;
            }
        }

        public override HttpStaticObjectsCollectionBase StaticObjects
        {
            get
            {
                return new HttpStaticObjectsCollectionWrapper(_application.StaticObjects);
            }
        }

        public override object SyncRoot
        {
            get
            {
                return ((ICollection)_application).SyncRoot;
            }
        }
        #endregion

        #region Instance Methods
        public override void Add(string name, object value)
        {
            _application.Add(name, value);
        }

        public override void Clear()
        {
            _application.Clear();
        }

        public override void CopyTo(Array array, int index)
        {
            ((ICollection)_application).CopyTo(array, index);
        }

        public override object Get(int index)
        {
            return _application.Get(index);
        }

        public override object Get(string name)
        {
            return _application.Get(name);
        }

        public override IEnumerator GetEnumerator()
        {
            return _application.GetEnumerator();
        }

        public override string GetKey(int index)
        {
            return _application.GetKey(index);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _application.GetObjectData(info, context);
        }

        public override void Lock()
        {
            _application.Lock();
        }

        public override void OnDeserialization(object sender)
        {
            _application.OnDeserialization(sender);
        }

        public override void Remove(string name)
        {
            _application.Remove(name);
        }

        public override void RemoveAll()
        {
            _application.RemoveAll();
        }

        public override void RemoveAt(int index)
        {
            _application.RemoveAt(index);
        }

        public override void Set(string name, object value)
        {
            _application.Set(name, value);
        }

        public override void UnLock()
        {
            _application.UnLock();
        }
        #endregion       
    }
}
