using System;
using System.IO;
using System.Security.Permissions;
using System.Web;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Serves as the base class for classes that provide access 
    /// to individual files that have been uploaded by a client.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class HttpPostedFileBase
    {
        #region Constructors
        /// <summary>
        /// Initializes the class for use by an inherited class instance. 
        /// This constructor can only be called by an inherited class. 
        /// </summary>
        protected HttpPostedFileBase()
        {
        }
        #endregion

        #region Instance Properties
        public virtual int ContentLength
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string ContentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string FileName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Stream InputStream
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Instance Methods
        public virtual void SaveAs(string filename)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
