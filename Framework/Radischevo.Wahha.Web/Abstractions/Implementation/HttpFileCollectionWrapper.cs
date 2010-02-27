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
    /// Encapsulates the HTTP intrinsic object that provides access 
    /// to files that were uploaded by a client.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpFileCollectionWrapper : HttpFileCollectionBase
    {
        #region Instance Fields
        private readonly HttpFileCollection _collection;
        #endregion

        #region Constructors
        public HttpFileCollectionWrapper(HttpFileCollection collection)
        {
            Precondition.Require(collection, () => Error.ArgumentNull("collection"));
            _collection = collection;
        }
        #endregion

        #region Instance Properties
        public override IEnumerable<string> AllKeys
        {
            get
            {
                return _collection.AllKeys;
            }
        }

        public override int Count
        {
            get
            {
                return _collection.Count;
            }
        }

        public override bool IsSynchronized
        {
            get
            {
                return ((ICollection)_collection).IsSynchronized;
            }
        }

        public override HttpPostedFileBase this[string name]
        {
            get
            {
                HttpPostedFile file = _collection[name];
                if (file == null)
                    return null;
                
                return new HttpPostedFileWrapper(file);
            }
        }

        public override HttpPostedFileBase this[int index]
        {
            get
            {
                HttpPostedFile file = _collection[index];
                if (file == null)
                    return null;
                
                return new HttpPostedFileWrapper(file);
            }
        }

        public override NameObjectCollectionBase.KeysCollection Keys
        {
            get
            {
                return _collection.Keys;
            }
        }

        public override object SyncRoot
        {
            get
            {
                return ((ICollection)_collection).SyncRoot;
            }
        }
        #endregion

        #region Instance Methods
        public override void CopyTo(Array dest, int index)
        {
            _collection.CopyTo(dest, index);
        }

        public override HttpPostedFileBase Get(int index)
        {
            HttpPostedFile file = _collection.Get(index);
            if (file == null)
                return null;
            
            return new HttpPostedFileWrapper(file);
        }

        public override HttpPostedFileBase Get(string name)
        {
            HttpPostedFile file = _collection.Get(name);
            if (file == null)
                return null;
            
            return new HttpPostedFileWrapper(file);
        }

        public override IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public override string GetKey(int index)
        {
            return _collection.GetKey(index);
        }

        public override void GetObjectData(SerializationInfo info, 
            StreamingContext context)
        {
            _collection.GetObjectData(info, context);
        }

        public override void OnDeserialization(object sender)
        {
            _collection.OnDeserialization(sender);
        }
        #endregion
    }
}
