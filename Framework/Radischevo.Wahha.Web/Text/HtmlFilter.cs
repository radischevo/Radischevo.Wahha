using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text.Sgml;

namespace Radischevo.Wahha.Web.Text
{
    /// <summary>
    /// Transforms the provided HTML string using 
    /// the specified parsing rules and 
    /// adds typographic beautification.
    /// </summary>
    public class HtmlFilter
    {
        #region Instance Fields
        private HtmlStringParser _parser;
        private HtmlStringTypographer _typographer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.Text.HtmlFilter"/> class.
        /// </summary>
        public HtmlFilter()
        {
            _parser = new HtmlStringParser(
                HtmlFilteringMode.AllowByDefault);
            _typographer = new HtmlStringTypographer();
            
            _parser.Typographer = _typographer;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets an instance of the 
        /// <see cref="Radischevo.Wahha.Web.Text.HtmlStringParser"/> class 
        /// used by this instance. Use this property to set the parsing rules, 
        /// which will be applied to the input text.
        /// </summary>
        public HtmlStringParser Parser
        {
            get
            {
                return _parser;
            }
        }

        /// <summary>
        /// Gets an instance of the 
        /// <see cref="Radischevo.Wahha.Web.Text.HtmlStringTypographer"/> class 
        /// used by this instance. Use this property to set the typographic 
        /// and replacement rules, which will be applied to the input text.
        /// </summary>
        public HtmlStringTypographer Typographer
        {
            get
            {
                return _typographer;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Creates an <see cref="System.Xml.XmlReader"/> for 
        /// reading the input HTML text.
        /// </summary>
        protected virtual XmlReader CreateReader(TextReader reader)
        {
            SgmlReader sgml = new SgmlReader();
            sgml.DocType = "html";
            sgml.WhitespaceHandling = (_parser.PreserveWhitespace) ?
                WhitespaceHandling.All : WhitespaceHandling.None;
            sgml.CaseFolding = CaseFolding.ToLower;
            sgml.InputStream = reader;

            return sgml;
        }

        /// <summary>
        /// Processes an input string using the specified 
        /// parsing and typographic rules.
        /// </summary>
        public string Parse(string htmlString)
        {
            Precondition.Require(htmlString, () => Error.ArgumentNull("htmlString"));
            using (StringReader reader = new StringReader(
				"<html>".Append(htmlString).Append("</html>").ToString()))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Processes an input string using the specified 
        /// parsing and typographic rules.
        /// </summary>
        public virtual string Parse(TextReader reader)
        {
            Precondition.Require(reader, () => Error.ArgumentNull("reader"));

            XmlDocument document = new XmlDocument();
            document.PreserveWhitespace = _parser.PreserveWhitespace;

            using (XmlReader xmlReader = CreateReader(reader))
            {
                document.Load(xmlReader);
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(sw))
                    {
                        _parser.ProcessDocument(document.DocumentElement, writer);
                    }
                    return sw.ToString();
                }
            }
        }
        #endregion
    }
}
