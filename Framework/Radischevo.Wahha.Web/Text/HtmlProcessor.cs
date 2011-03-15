using System;
using System.IO;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    /// <summary>
    /// Transforms the provided HTML string using 
    /// the specified parsing rules and 
    /// adds typographic beautification.
    /// </summary>
    public class HtmlProcessor
    {
        #region Instance Fields
        private HtmlFilterSettings _filter;
        private HtmlTypographer _typographer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.Text.HtmlProcessor"/> class.
        /// </summary>
        public HtmlProcessor()
        {
            _filter = new HtmlFilterSettings(
                HtmlFilteringMode.AllowByDefault);
            _typographer = new HtmlTypographer();
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets an instance of the 
        /// <see cref="Radischevo.Wahha.Web.Text.HtmlFilterSettings"/> class 
        /// used by this instance. Use this property to set the filtering rules, 
        /// which will be applied to the input text.
        /// </summary>
        public HtmlFilterSettings Filter
        {
            get
            {
                return _filter;
            }
        }

        /// <summary>
        /// Gets an instance of the 
        /// <see cref="Radischevo.Wahha.Web.Text.HtmlTypographer"/> class 
        /// used by this instance. Use this property to set the typographic 
        /// and replacement rules, which will be applied to the input text.
        /// </summary>
        public HtmlTypographer Typographer
        {
            get
            {
                return _typographer;
            }
        }
        #endregion

        #region Instance Methods
		/// <summary>
		/// Processes an input string using the specified 
		/// parsing and typographic rules.
		/// </summary>
		public string Execute(string htmlString)
		{
			return Execute(htmlString, null);
		}

        /// <summary>
        /// Processes an input string using the specified 
        /// parsing and typographic rules.
        /// </summary>
		public string Execute(string htmlString, IValueSet parameters)
        {
            Precondition.Require(htmlString, () => Error.ArgumentNull("htmlString"));
            using (StringReader reader = new StringReader(
				"<html>".Append(htmlString).Append("</html>").ToString()))
            {
				return Execute(reader, parameters);
            }
        }

		/// <summary>
		/// Processes an input string using the specified 
		/// parsing and typographic rules.
		/// </summary>
		public string Execute(TextReader reader)
		{
			return Execute(reader, null);
		}

        /// <summary>
        /// Processes an input string using the specified 
        /// parsing and typographic rules.
        /// </summary>
		public virtual string Execute(TextReader reader, IValueSet parameters)
        {
            Precondition.Require(reader, () => Error.ArgumentNull("reader"));
			using (StringWriter writer = new StringWriter())
			{
				using (HtmlFilter parser = new HtmlFilter(Filter, reader, writer))
				{
					parser.Typographer = _typographer;
					parser.Parameters = parameters;
					parser.Execute();

					return writer.ToString();
				}
			}
        }
        #endregion
    }
}
