using System;
using System.IO;
using System.Security.Permissions;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that provides access to 
    /// individual files that have been uploaded by a client.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpPostedFileWrapper : HttpPostedFileBase
    {
        #region Instance Fields
        private readonly HttpPostedFile _file;
        #endregion

        #region Constructors
        public HttpPostedFileWrapper(HttpPostedFile file)
        {
            Precondition.Require(file, Error.ArgumentNull("file"));
            _file = file;
        }
        #endregion

        #region Instance Properties
        public override int ContentLength
        {
            get
            {
                return _file.ContentLength;
            }
        }

        public override string ContentType
        {
            get
            {
                return _file.ContentType;
            }
        }

        public override string FileName
        {
            get
            {
                return _file.FileName;
            }
        }

        public override Stream InputStream
        {
            get
            {
                return _file.InputStream;
            }
        }
        #endregion

        #region Instance Methods
        public override void SaveAs(string filename)
        {
            _file.SaveAs(filename);
        }
        #endregion
    }
}
