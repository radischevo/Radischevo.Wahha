using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Serialization;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class JsonResult : ActionResult
    {
        #region Constants
        private const string _defaultContentType = "application/json";
        #endregion

        #region Instance Fields
        private object _data;
        private Encoding _contentEncoding;
        private string _contentType;
        private SerializationFormat _format;
        private List<JavaScriptConverter> _converters;
        #endregion

		#region Constructors
		public JsonResult()
			: this(null, null, null, SerializationFormat.Json, null)
		{
		}

		public JsonResult(object data)
			: this(data, null, null, SerializationFormat.Json, null)
		{
		}

		public JsonResult(object data, string contentType,
			Encoding contentEncoding, SerializationFormat format)
			: this(data, contentType, contentEncoding, format, null)
		{
		}

		public JsonResult(object data, string contentType,
			Encoding contentEncoding, SerializationFormat format, 
			IEnumerable<JavaScriptConverter> converters)
		{
			_converters = new List<JavaScriptConverter>(
				converters ?? Enumerable.Empty<JavaScriptConverter>());

			_data = data;
			_contentType = contentType;
			_contentEncoding = contentEncoding;
			_format = format;
		}
		#endregion

		#region Instance Properties
		public object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public Encoding ContentEncoding
        {
            get
            {
                return _contentEncoding;
            }
            set
            {
                _contentEncoding = value;
            }
        }

        public SerializationFormat Format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
            }
        }

        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        public ICollection<JavaScriptConverter> Converters
        {
            get
            {
                return _converters;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Executes an action result against the specified 
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ControllerContext"/></param>
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
			if (context.IsChild)
				throw Error.CannotExecuteResultInChildAction();

            HttpResponseBase response = context.Context.Response;
            string callbackFunction = context.Context.Request
				.Parameters.GetValue<string>("callback");

            string data = String.Empty;
            response.ContentType = (String.IsNullOrEmpty(_contentType)) 
				? _defaultContentType : _contentType;
            
            if (_contentEncoding != null) 
                response.ContentEncoding = _contentEncoding;

            if (_data != null)
            {
                #pragma warning disable 0618
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(_converters);
                data = serializer.Serialize(_data, _format);
                #pragma warning restore 0618
            }
            response.Write(String.IsNullOrEmpty(callbackFunction) ? data 
                : String.Format("if(typeof {0}==='function'){{0}({1});}", 
                    callbackFunction, data));
        }
        #endregion
    }
}
