using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Permissions;
using System.Web;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Serves as the base class for classes that provide 
    /// access to files that were uploaded by a client.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpFileCollectionBase : NameObjectCollectionBase, ICollection, IEnumerable
    {
        #region Constructors
        /// <summary>
        /// Initializes the class for use by an inherited 
        /// class instance. This constructor can only be 
        /// called by an inherited class. 
        /// </summary>
        protected HttpFileCollectionBase()
        {
        }
        #endregion

        #region Instance Properties
        public virtual IEnumerable<string> AllKeys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Count
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

        public virtual HttpPostedFileBase this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual HttpPostedFileBase this[string name]
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
        public virtual void CopyTo(Array dest, int index)
        {
            throw new NotImplementedException();
        }

        public virtual HttpPostedFileBase Get(int index)
        {
            throw new NotImplementedException();
        }

        public virtual HttpPostedFileBase Get(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public virtual string GetKey(int index)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
