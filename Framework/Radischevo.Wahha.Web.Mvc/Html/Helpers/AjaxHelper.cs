using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    /// <summary>
    /// Class containing convenience methods for use in 
    /// rendering HTML for use in Ajax scenarios within a view.
    /// </summary>
    public class AjaxHelper : IHideObjectMembers
    {
        #region Constants
        private const string _storageKey = "Radischevo.Wahha.Web.Mvc.Ajax.ScriptManager.Context";
        #endregion

        #region Instance Fields
        private ViewContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of 
        /// <see cref="Radischevo.Wahha.Web.Mvc.UI.AjaxHelper"/>
        /// </summary>
        /// <param name="context">The current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/></param>
        public AjaxHelper(ViewContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            _context = context;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/>.
        /// </summary>
        public ViewContext Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Gets a <see cref="Radischevo.Wahha.Web.Mvc.Ajax.ScriptManager"/> 
        /// instance for the current request.
        /// </summary>
        public ScriptManager Scripts
        {
            get
            {
                ScriptManager manager = (_context.Context.Items[_storageKey] as ScriptManager);
                if (manager == null)
                {
                    manager = new ScriptManager(_context);
                    _context.Context.Items[_storageKey] = manager;
                }
                return manager;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Encodes the specified string to a 
        /// valid JavaScript string.
        /// </summary>
        /// <param name="data">The string to encode.</param>
        public string Encode(string data)
        {
            if (String.IsNullOrEmpty(data))
                return data;

            StringBuilder sb = new StringBuilder();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.Serialize(data, sb);

            return sb.ToString(1, sb.Length - 2);
        }

        /// <summary>
        /// Converts an object to a JSON string.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        public string Serialize(object data)
        {
            return Serialize(data, SerializationFormat.Json, -1);
        }

        /// <summary>
        /// Converts an object to a JSON string.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <param name="format">Specifies the JSON formatting options.</param>
        public string Serialize(object data, SerializationFormat format)
        {
            return Serialize(data, format, -1);
        }

        /// <summary>
        /// Converts an object to a JSON string.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <param name="recursionLimit">Sets the limit for constraining the 
        /// number of object levels to process.</param>
        public string Serialize(object data, int recursionLimit)
        {
            return Serialize(data, SerializationFormat.Json, recursionLimit);
        }

        /// <summary>
        /// Converts an object to a JSON string.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <param name="format">Specifies the JSON formatting options.</param>
        /// <param name="recursionLimit">Sets the limit for constraining the 
        /// number of object levels to process.</param>
        public virtual string Serialize(object data, 
            SerializationFormat format, int recursionLimit)
        {
            string result = String.Empty;

            if (data != null)
            {
                #pragma warning disable 0618
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                
                if(recursionLimit > -1)
                    serializer.RecursionLimit = recursionLimit;

                result = serializer.Serialize(data, format);
                #pragma warning restore 0618
            }
            return result;
        }

        /// <summary>
        /// Converts the specified JSON string 
        /// to an object graph.
        /// </summary>
        /// <param name="json">The JSON string to be deserialized.</param>
        public virtual object Deserialize(string json)
        {
            if (String.IsNullOrEmpty(json))
                return null;

            object obj = null;
            #pragma warning disable 0618
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            obj = serializer.DeserializeObject(json);
            #pragma warning restore 0618

            return obj;
        }

        /// <summary>
        /// Converts the specified JSON string 
        /// to an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="json">The JSON string to be deserialized.</param>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        public virtual T Deserialize<T>(string json)
        {
            if (String.IsNullOrEmpty(json))
                return default(T);

            T obj = default(T);
            #pragma warning disable 0618
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            obj = serializer.Deserialize<T>(json);
            #pragma warning restore 0618

            return obj;
        }
        #endregion
    }
}
