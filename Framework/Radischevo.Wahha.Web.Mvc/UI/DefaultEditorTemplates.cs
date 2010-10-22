using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using W = System.Web.UI.WebControls;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Mvc.Html;
using Radischevo.Wahha.Web.Text;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc.UI
{
	internal static class DefaultEditorTemplates
	{
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
					TriStateValues(isNull, model), new {
						@id = elementName
					});
			}
			return html.Controls.CheckBox(elementName,
					model, new {
						@id = elementName
					});
		}

		public static string Enumeration(HtmlHelper html)
		{
			string elementName = html.Context.ViewData.Template
				.GetHtmlElementName(System.String.Empty);

			Type enumType = html.Context.ViewData.Metadata.Type;

			if (!enumType.IsEnum) // handle enums only
				return Object(html);

			return html.Controls.DropDownList(elementName,
				EnumerationValues(enumType, html.Context.ViewData
					.Metadata.IsNullableValueType, 
					html.Context.ViewData.Model),
				new {
					@id = elementName
				});
		}

		public static string Decimal(HtmlHelper html)
		{
			if (html.Context.ViewData.Template.Value == html.Context.ViewData.Model)
				html.Context.ViewData.Template.Value = string.Format(
					CultureInfo.CurrentCulture, "{0:0.00}", html.Context.ViewData.Model);

			return String(html);
		}

		public static string Hidden(HtmlHelper html)
		{
			string elementName = html.Context.ViewData.Template
				.GetHtmlElementName(System.String.Empty);

			object model = html.Context.ViewData.Model;
			byte[] modelAsByteArray = (model as byte[]);
			if (modelAsByteArray != null)
				model = Convert.ToBase64String(modelAsByteArray);

			return html.Controls.Hidden(elementName,
				model, new {
					@id = elementName
				});
		}

		public static string TextArea(HtmlHelper html)
		{
			string elementName = html.Context.ViewData.Template
				.GetHtmlElementName(System.String.Empty);

			return html.Controls.TextArea(elementName,
				(html.Context.ViewData.Template.Value ?? "").ToString(),
				5, 40, new {
					@id = elementName
				});
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
					result.Append(html.Templates.Render(W.DataBoundControlMode.Edit, metadata, null, fieldName, item));
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
			ValidationHelper validation = new ValidationHelper(html.Context);

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
				string elementName = html.Context.ViewData.Template.GetHtmlElementName(property.PropertyName);

				if (!property.HideSurroundingChrome)
					builder.Append(LabelExtensions.Render(html.Context, property.PropertyName, property));

				builder.Append(html.Templates.Render(W.DataBoundControlMode.Edit,
					property, null, property.PropertyName, propertyValue));

				validation.Messages(elementName, (errors) => {
					builder.Append(new HtmlElementBuilder("ul", 
						new { @class = "validation-error" }, 
						string.Join(Environment.NewLine, errors.Select(
							error => new HtmlElementBuilder("li", null, error.Message).ToString()
						).ToArray())
					));
				});
			}
			return builder.ToString();
		}

		public static string String(HtmlHelper html)
		{
			string elementName = html.Context.ViewData.Template
				.GetHtmlElementName(System.String.Empty);

			return html.Controls.TextBox(elementName,
				html.Context.ViewData.Template.Value, new {
					@id = elementName
				});
		}

		public static string Password(HtmlHelper html)
		{
			string elementName = html.Context.ViewData.Template
				.GetHtmlElementName(System.String.Empty);

			return html.Controls.Password(elementName,
				html.Context.ViewData.Template.Value, new {
					@id = elementName
				});
		}

		public static SelectList TriStateValues(bool isNull, bool value)
		{
			return new SelectList(new ListItem[] {
                new ListItem { Text = Resources.Resources.String_TriState_NotSet, 
                    Value = "", Selected = isNull },
                new ListItem { Text = Resources.Resources.String_TriState_Yes, 
                    Value = "true", Selected = (!isNull && value) },
                new ListItem { Text = Resources.Resources.String_TriState_No, 
                    Value = "false", Selected = (!isNull && !value) },
            });
		}

		public static SelectList EnumerationValues(Type enumType, 
			bool allowNullValue, object model)
		{
			string[] names = Enum.GetNames(enumType);
			string selectedValue = (model as string);

			return new SelectList(names.Select(name => new ListItem() {
				Value = name, Text = name, 
				Selected = string.Equals(selectedValue, 
					name, StringComparison.OrdinalIgnoreCase)
			}));
		}
	}
}
