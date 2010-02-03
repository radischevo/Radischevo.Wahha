using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Mvc.Html;

namespace Radischevo.Wahha.Web.Mvc.UI
{
    internal static class DefaultDisplayTemplates
    {
        #region Static Methods
        public static string Boolean(HtmlHelper html)
        {
            string elementName = html.Context.ViewData.Template
                .GetHtmlElementName(System.String.Empty);
            bool model = false;

            if (html.Context.ViewData.Model != null)
                model = Convert.ToBoolean(html.Context.ViewData.Model, 
                    CultureInfo.InvariantCulture);

            if (html.Context.ViewData.Metadata.IsNullableValueType)
            {
                bool isNull = (html.Context.ViewData.Model == null);
                return html.Controls.DropDownList(elementName,
                    DefaultEditorTemplates.TriStateValues(isNull, model), 
                    new { @id = elementName, @disabled = "disabled" });
            }
            
            return html.Controls.CheckBox(elementName, model,
                new { @id = elementName, @disabled = "disabled" });
        }

		public static string Decimal(HtmlHelper html)
		{
			if (html.Context.ViewData.Template.Value == html.Context.ViewData.Model)
				html.Context.ViewData.Template.Value = string.Format(CultureInfo.CurrentCulture, 
					"{0:0.00}", html.Context.ViewData.Model);

			return String(html);
		}

        public static string EmailAddress(HtmlHelper html)
        {
            return html.Controls.Tag("a", new { HRef = System.String.Format(
                CultureInfo.InvariantCulture, "mailto:{0}", html.Context.ViewData.Model) }, 
                Convert.ToString(html.Context.ViewData.Template.Value, CultureInfo.InvariantCulture));
        }

        public static string HiddenInput(HtmlHelper html)
        {
            if (html.Context.ViewData.Metadata.HideSurroundingChrome)
                return System.String.Empty;
            
            return String(html);
        }

        public static string Hyperlink(HtmlHelper html)
        {
            return html.Controls.Tag("a", new { HRef = Convert.ToString(
                html.Context.ViewData.Model, CultureInfo.InvariantCulture) },
                Convert.ToString(html.Context.ViewData.Template.Value, CultureInfo.InvariantCulture));
        }

        public static string Object(HtmlHelper html)
        {
            ModelMetadata metadata = html.Context.ViewData.Metadata;
            object model = html.Context.ViewData.Model;
            Type type = html.Context.ViewData.Template.Type;

            if (model == null)
                return metadata.NullDisplayText;

            if (html.Context.ViewData.Template.Depth > 1)
                return metadata.GetDisplayText(model);

            StringBuilder builder = new StringBuilder();
            foreach (ModelMetadata property in metadata.Properties
				.Where(pm => pm.Visible).OrderBy(pm => pm.FieldOrder))
            {
                object propertyValue = (model == null) ? null : property.GetModel(model);
                if(!metadata.HideSurroundingChrome)
                    builder.Append(LabelExtensions.Render(html.Context, property.PropertyName, property));

                builder.Append(html.Templates.Render(DataBoundControlMode.ReadOnly, 
                    property, null, property.PropertyName, propertyValue));
            }
            return builder.ToString();
        }

        public static string String(HtmlHelper html)
        {
            return html.Encode(html.Context.ViewData.Template.Value);
        }

        public static string HtmlString(HtmlHelper html)
        {
            return html.Context.ViewData.Template.Value.ToString();
        }
        #endregion
    }
}
