using System;
using System.IO;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Sends binary content to the response via a 
    /// <see cref="T:System.IO.Stream"/>.
    /// </summary>
    public class BinaryStreamResult : BinaryResult
    {
        #region Instance Fields
        private Stream _stream;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of 
        /// <see cref="T:Radischevo.Wahha.Web.Mvc.FileStreamResult"/>
        /// </summary>
        /// <param name="stream">The stream to send to the response</param>
        public BinaryStreamResult(Stream stream)
            : this(stream, null, null)
        {   }

        /// <summary>
        /// Initializes a new instance of 
        /// <see cref="T:Radischevo.Wahha.Web.Mvc.FileStreamResult"/>
        /// </summary>
        /// <param name="stream">The stream to send to the response</param>
        /// <param name="contentType">The content type to use for the response</param>
        public BinaryStreamResult(Stream stream, string contentType)
            : this(stream, contentType, null)
        {   }

        /// <summary>
        /// Initializes a new instance of 
        /// <see cref="T:Radischevo.Wahha.Web.Mvc.FileStreamResult"/>
        /// </summary>
        /// <param name="stream">The stream to send to the response</param>
        /// <param name="contentType">The content type to use for the response</param>
        /// <param name="fileName">The filename, which will appear in the download dialog</param>
        public BinaryStreamResult(Stream stream, string contentType, 
            string fileName) : base()
        {
            Precondition.Require(stream, () => Error.ArgumentNull("stream"));
            _stream = stream;
            base.FileName = fileName;
            base.ContentType = contentType;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the stream which will be sent to the response.
        /// </summary>
        public Stream Stream
        {
            get
            {
                return _stream;
            }
        }
        #endregion

        #region Instance Methods
        protected override void Write(HttpContextBase context)
        {
            Stream output = context.Response.OutputStream;
            using (_stream)
            {
                byte[] buffer = new byte[4096];
                int count = 0;

                while ((count = _stream.Read(buffer, 0, 4096)) > 0)
                    output.Write(buffer, 0, count);
            }
        }
        #endregion
    }
}
