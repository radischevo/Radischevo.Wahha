using System;
using System.Collections;
using System.IO;
using System.Security.Permissions;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that provides a collection 
    /// of application-scoped objects for the <see cref="T:System.Web.HttpApplicationState.StaticObjects" /> property.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpStaticObjectsCollectionWrapper : HttpStaticObjectsCollectionBase
    {
        #region Instance Fields
        private readonly HttpStaticObjectsCollection _collection;
        #endregion

        #region Constructors
        public HttpStaticObjectsCollectionWrapper(HttpStaticObjectsCollection collection)
        {
            Precondition.Require(collection, Error.ArgumentNull("collection"));
            _collection = collection;
        }
        #endregion

        #region Instance Properties
        public override int Count
        {
            get
            {
                return _collection.Count;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return _collection.IsReadOnly;
            }
        }

        public override bool IsSynchronized
        {
            get
            {
                return _collection.IsSynchronized;
            }
        }

        public override object this[string name]
        {
            get
            {
                return _collection[name];
            }
        }

        public override bool NeverAccessed
        {
            get
            {
                return _collection.NeverAccessed;
            }
        }

        public override object SyncRoot
        {
            get
            {
                return _collection.SyncRoot;
            }
        }
        #endregion

        #region Instance Methods
        public override void CopyTo(Array array, int index)
        {
            _collection.CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public override object GetObject(string name)
        {
            return _collection.GetObject(name);
        }

        public override void Serialize(BinaryWriter writer)
        {
            _collection.Serialize(writer);
        }
        #endregion
    }
}
