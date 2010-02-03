using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// Thrown if any errors occur while parsing the source.
    /// </summary>
    [Serializable]
    public class SgmlParseException : Exception
    {
        #region Instance Fields
        private string _entityContext;
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiates a new instance of SgmlParseException with no specific error information.
        /// </summary>
        public SgmlParseException()
        {
        }

        /// <summary>
        /// Instantiates a new instance of SgmlParseException with an error message describing the problem.
        /// </summary>
        /// <param name="message">A message describing the error that occurred</param>
        public SgmlParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Instantiates a new instance of SgmlParseException with an error message describing the problem.
        /// </summary>
        /// <param name="message">A message describing the error that occurred</param>
        /// <param name="e">The entity on which the error occurred.</param>
        public SgmlParseException(string message, Entity e)
            : base(message)
        {
            if (e != null)
                _entityContext = e.Context();
        }

        /// <summary>
        /// Instantiates a new instance of SgmlParseException with an error message describing the problem.
        /// </summary>
        /// <param name="message">A message describing the error that occurred</param>
        /// <param name="innerException">The original exception that caused the problem.</param>
        public SgmlParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SgmlParseException class with serialized data. 
        /// </summary>
        /// <param name="streamInfo">The object that holds the serialized object data.</param>
        /// <param name="streamCtx">The contextual information about the source or destination.</param>
        protected SgmlParseException(SerializationInfo streamInfo, StreamingContext streamCtx)
            : base(streamInfo, streamCtx)
        {
            if (streamInfo != null)
                _entityContext = streamInfo.GetString("entityContext");
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Contextual information detailing the entity on which the error occurred.
        /// </summary>
        public string EntityContext
        {
            get
            {
                return _entityContext;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Precondition.Require(info, Error.ArgumentNull("info"));

            info.AddValue("entityContext", _entityContext);
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
