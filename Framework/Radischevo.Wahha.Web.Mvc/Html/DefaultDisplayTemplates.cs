using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc.Html
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

		public static string Collection(HtmlHelper html)
		{
			object model = html.Context.ViewData.Model;
			if (model == null)
				return string.Empty;

			IEnumerable collection = (model as IEnumerable);
			if (collection == null)
				throw Error.CollectionTypeMustBeEnumerable(model.GetType());

			Type elementType = collection.GetType().GetSequenceElementType() ?? typeof(string);
			elementType = elementType.MakeNonNullableType();
			string oldPrefix = html.Context.ViewData.Template.Prefix;

			try
			{
				html.Context.ViewData.Template.Prefix = string.Empty;

				StringBuilder result = new StringBuilder();
				int index = 0;

				foreach (object item in collection)
				{
					Type itemType = elementType;
					if (item != null)
						itemType = item.GetType().MakeNonNullableType();
					
					ModelMetadata metadata = Configuration.Instance.Models
						.MetadataProvider.GetMetadata(itemType);

					string fieldName = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", oldPrefix, index++);
					result.Append(html.Templates.Render(DataBoundControlMode.ReadOnly, metadata, null, fieldName, item));
				}
				return result.ToString();
			}
			finally
			{
				html.Context.ViewData.Template.Prefix = oldPrefix;
			}
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
