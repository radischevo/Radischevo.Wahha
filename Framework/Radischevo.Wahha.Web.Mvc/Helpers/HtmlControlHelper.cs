using System;
using System.IO;
using System.Globalization;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text;

namespace Radischevo.Wahha.Web.Mvc.UI
{
    /// <summary>
    /// Class containing convenience methods for use in 
    /// rendering HTML controls within a view.
    /// </summary>
    public class HtmlControlHelper : IHideObjectMembers
    {
        #region Instance Fields
        private ViewContext _context;
        private IValueSet _dataSource;
        #endregion

        #region Constructors
        public HtmlControlHelper(ViewContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            _context = context;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/>
        /// </summary>
        public ViewContext Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Gets or sets the data source, which 
        /// is used in the form binding scenarios. 
        /// </summary>
        /// <remarks>By default, ViewContext.Parameters collection 
        /// is used as the data source.</remarks>
        public IValueSet DataSource
        {
            get
            {
                if (_dataSource == null)
                    _dataSource = _context.Parameters;

                return _dataSource;
            }
            set
            {
                _dataSource = value;
            }
        }
        #endregion

        #region Instance Methods
        public string Tag(string tagName)
        {
            return Tag(tagName, null);
        }

        public string Tag(string tagName, object attributes)
        {
            HtmlElementBuilder builder = new HtmlElementBuilder(tagName, attributes, null);
            return builder.ToString();
        }
        
        public string Tag(string tagName, params string[] content)
        {
            HtmlElementBuilder builder = new HtmlElementBuilder(tagName, null,
                (content == null) ? null : String.Join("", content));
            return builder.ToString();
        }

        public string Tag(string tagName, object attributes, params string[] content)
        {
            HtmlElementBuilder builder = new HtmlElementBuilder(tagName, attributes, 
                (content == null) ? null : String.Join("", content));
            return builder.ToString();
        }

        /// <summary>
        /// Returns the string for a hidden input containing a 
        /// token used to prevent CSRF attacks.
        /// </summary>
        public string Token()
        {
            return Token(null);
        }

        /// <summary>
        /// Returns the string for a hidden input containing a 
        /// token used to prevent CSRF attacks.
        /// </summary>
        /// <param name="value">An extended value to store in the token.</param>
        public string Token(string value)
        {
            RequestValidationToken token = RequestValidationToken.Create(Context.Context);

            HtmlElementBuilder builder = new HtmlElementBuilder("input");
            builder.Attributes.Add("type", "hidden");
            builder.Attributes.Add("name", ValidateRequestTokenAttribute.ValidationFieldName);
            builder.Attributes.Add("value", token.Serialize());

            return builder.ToString();
        }
        #endregion
    }

    /// <summary>
    /// Class containing convenience methods for use in 
    /// rendering HTML controls within a strongly-typed view.
    /// </summary>
    public class HtmlControlHelper<TModel> : HtmlControlHelper
    {
        #region Constructors
        public HtmlControlHelper(ViewContext context)
            : base(context)
        {
        }
        #endregion
    }
}
