using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.UI;
using Radischevo.Wahha.Web.Text;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public static class FormExtensions
    {
        #region Helper Methods
        private static string InputBuilder(HtmlControlHelper helper, 
            string type, string name, object value, 
            IDictionary<string, object> attributes)
        {
            if (attributes == null)
                attributes = new ValueDictionary();

            if (value == null)
                attributes.Remove("value");

            HtmlElementBuilder htb = new HtmlElementBuilder("input");
            htb.Attributes.Merge(attributes, true);
            htb.Attributes.Merge("name", name, true);
            htb.Attributes.Merge("type", type, true);
            
            if(value != null)
                htb.Attributes.Merge("value", value, true);

            return htb.ToString();
        }

        private static string TextAreaBuilder(HtmlControlHelper helper,
            string name, string value, int rows, int columns, 
            IDictionary<string, object> attributes)
        {
            if (attributes == null)
                attributes = new ValueDictionary();

            HtmlElementBuilder htb = new HtmlElementBuilder("textarea");
            htb.Attributes.Merge(attributes, true);
            htb.Attributes.Merge("name", name, true);
            htb.Attributes.Merge("rows", rows, false);
            htb.Attributes.Merge("cols", columns, false);
            htb.InnerText = value ?? String.Empty;

            return htb.ToString();
        }

        private static string SelectBuilder(HtmlControlHelper helper, 
            string name, MultiSelectList dataSource, bool isMultiple, 
            IDictionary<string, object> attributes)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));
            
            if (attributes == null)
                attributes = new ValueDictionary();

            if (isMultiple)
                attributes["multiple"] = "multiple";
            else
                attributes.Remove("multiple");

            HtmlElementBuilder outer = new HtmlElementBuilder("select");
            outer.Attributes.Merge(attributes, true);
            outer.Attributes.Merge("name", name, true);

            if (dataSource != null)
            {
                StringBuilder builder = new StringBuilder();

                IEnumerable<ListItem> listItems = dataSource.GetListItems();
                string[] selectedValues = null;

                if (helper.DataSource.Keys.Any(k => k.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    selectedValues = helper.DataSource.GetValue<string>(name, String.Empty)
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                builder.AppendLine();

                foreach (ListItem item in listItems)
                {
                    if (selectedValues != null)
                        item.Selected = selectedValues.Any(s =>
                            String.Equals(item.Value, s, StringComparison.OrdinalIgnoreCase));

                    HtmlElementBuilder inner = new HtmlElementBuilder("option");
                    inner.InnerText = item.Text;

                    if (item.Value != null)
                        inner.Attributes.Merge("value", item.Value, true);

                    if (item.Selected)
                        inner.Attributes.Merge("selected", "selected");

                    builder.AppendLine(inner.ToString());
                }
                outer.InnerHtml = builder.ToString();
            }
            return outer.ToString();
        }

        private static MvcForm FormBuilder(HtmlControlHelper helper, 
            FormMethod method, string action, IDictionary<string, object> attributes)
        {
            HtmlElementBuilder builder = new HtmlElementBuilder("form");
            HttpResponseBase response = helper.Context.Context.Response;

            if (attributes == null)
                attributes = new ValueDictionary();

            builder.Attributes.Merge(attributes);
            builder.Attributes.Merge("action", action, true);
            builder.Attributes.Merge("method", GetFormMethod(method), true);

            response.Write(builder.ToString(HtmlElementRenderMode.StartTag));
            return new MvcForm(response, method);
        }

        private static string GetFormMethod(FormMethod method)
        {
            switch (method)
            {
                case FormMethod.Get:
                    return "get";
                case FormMethod.Post:
                    return "post";
            }
            return String.Empty;
        }
        #endregion

        #region Form
        public static MvcForm Form(this HtmlControlHelper helper,
            string actionUrl)
        {
            return Form(helper, FormMethod.Post, actionUrl, null);
        }

        public static MvcForm Form(this HtmlControlHelper helper,
            string actionUrl, object attributes)
        {
            return Form(helper, FormMethod.Post, actionUrl,
                new ValueDictionary(attributes));
        }

        public static MvcForm Form(this HtmlControlHelper helper,
            string actionUrl, IDictionary<string, object> attributes)
        {
            return FormBuilder(helper, FormMethod.Post, actionUrl, attributes);
        }

        public static MvcForm Form(this HtmlControlHelper helper,
            FormMethod method, string actionUrl)
        {
            return Form(helper, method, actionUrl, null);
        }

        public static MvcForm Form(this HtmlControlHelper helper,
            FormMethod method, string actionUrl, object attributes)
        {
            return Form(helper, method, actionUrl, 
                new ValueDictionary(attributes));
        }

        public static MvcForm Form(this HtmlControlHelper helper,
            FormMethod method, string actionUrl,
            IDictionary<string, object> attributes)
        {
            return FormBuilder(helper, method, actionUrl, attributes);
        }

        public static MvcForm Form<TController>(this HtmlControlHelper helper,
            Expression<Action<TController>> action)
            where TController : IController
        {
            return Form(helper, action, FormMethod.Post, null);
        }

        public static MvcForm Form<TController>(this HtmlControlHelper helper,
            Expression<Action<TController>> action, object attributes)
            where TController : IController
        {
            return Form(helper, action, FormMethod.Post, new ValueDictionary(attributes));
        }

        public static MvcForm Form<TController>(this HtmlControlHelper helper,
            Expression<Action<TController>> action, IDictionary<string, object> attributes)
            where TController : IController
        {
            return Form(helper, action, FormMethod.Post, attributes);
        }

        public static MvcForm Form<TController>(this HtmlControlHelper helper,
            Expression<Action<TController>> action, FormMethod method,
            object attributes)
            where TController : IController
        {
            return Form(helper, action, method, new ValueDictionary(attributes));
        }

        public static MvcForm Form<TController>(this HtmlControlHelper helper, 
            Expression<Action<TController>> action, FormMethod method, 
            IDictionary<string, object> attributes)
            where TController : IController
        {
            UrlHelper url = new UrlHelper(helper.Context);
            return FormBuilder(helper, method, url.Route<TController>(action), attributes);
        }
        #endregion

        #region CheckBox
		public static string CheckBox(this HtmlControlHelper helper, string name)
		{
			return CheckBox(helper, name, null, new ValueDictionary());
		}

        public static string CheckBox(this HtmlControlHelper helper, 
			string name, object value)
        {
            return CheckBox(helper, name, value, new ValueDictionary());
        }

        public static string CheckBox(this HtmlControlHelper helper, 
            string name, object value, bool isChecked)
        {
            return CheckBox(helper, name, value, isChecked, new ValueDictionary());
        }

        public static string CheckBox(this HtmlControlHelper helper,
            string name, object value, object attributes)
        {
            return CheckBox(helper, name, value, new ValueDictionary(attributes));
        }

        public static string CheckBox(this HtmlControlHelper helper,
            string name, object value, IDictionary<string, object> attributes)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));
            return CheckBox(helper, name, value, false, attributes);
        }

        public static string CheckBox(this HtmlControlHelper helper,
            string name, object value, bool isChecked, object attributes)
        {
            return CheckBox(helper, name, value, isChecked, new ValueDictionary(attributes));
        }

        public static string CheckBox(this HtmlControlHelper helper, 
            string name, object value, bool isChecked, IDictionary<string, object> attributes)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));

            if (attributes == null)
                attributes = new ValueDictionary();

            if (helper.DataSource.Keys.Any(k => k.Equals(name, 
                StringComparison.OrdinalIgnoreCase)) || helper.DataSource.Keys.Any())
                isChecked = helper.DataSource.GetValue<bool>(name);

            if (isChecked)
                attributes["checked"] = "checked";
            else
                attributes.Remove("checked");
            
            return InputBuilder(helper, "checkbox", name, value, attributes);
        }

        public static string CheckBox<TModel>(this HtmlControlHelper<TModel> helper,
            string name, object value, Expression<Func<TModel, bool>> isChecked)
            where TModel : class
        {
            return CheckBox(helper, name, value, isChecked, null);
        }

        public static string CheckBox<TModel>(this HtmlControlHelper<TModel> helper,
			string name, object value, Expression<Func<TModel, bool>> isChecked,
            object attributes)
            where TModel : class
        {
            return CheckBox(helper, name, value, isChecked, new ValueDictionary(attributes));
        }

        public static string CheckBox<TModel>(this HtmlControlHelper<TModel> helper,
			string name, object value, Expression<Func<TModel, bool>> isChecked, 
            IDictionary<string, object> attributes)
            where TModel: class
        {
            Precondition.Require(isChecked, () => Error.ArgumentNull("isChecked"));

            TModel model = (TModel)helper.Context.ViewData.Model;
            Func<bool> accessor = LinqHelper.WrapModelAccessor(isChecked, model);

            return CheckBox(helper, name, value, accessor(), attributes);
        }
        #endregion

        #region RadioButton
        public static string RadioButton(this HtmlControlHelper helper,
            string name, object value)
        {
            return RadioButton(helper, name, value, new ValueDictionary());
        }

        public static string RadioButton(this HtmlControlHelper helper,
            string name, object value, bool isChecked)
        {
            return RadioButton(helper, name, value, isChecked, new ValueDictionary());
        }

        public static string RadioButton(this HtmlControlHelper helper,
            string name, object value, IDictionary<string, object> attributes)
        {
            return RadioButton(helper, name, value, false, attributes);
        }

        public static string RadioButton(this HtmlControlHelper helper,
            string name, object value, object attributes)
        {
            return RadioButton(helper, name, value, false, new ValueDictionary(attributes));
        }

        public static string RadioButton(this HtmlControlHelper helper,
            string name, object value, bool isChecked, object attributes)
        {
            return RadioButton(helper, name, value, isChecked, new ValueDictionary(attributes));
        }

        public static string RadioButton(this HtmlControlHelper helper,
            string name, object value, bool isChecked, IDictionary<string, object> attributes)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));

            if (attributes == null)
                attributes = new ValueDictionary();

            if (helper.DataSource.Keys.Any(k => k.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                string providedValue = Convert.ToString(value, CultureInfo.InvariantCulture);
                string actualValue = helper.DataSource.GetValue<string>(name);

                isChecked = (!String.IsNullOrEmpty(actualValue) &&
                    String.Equals(providedValue, actualValue, StringComparison.OrdinalIgnoreCase));
            }

            if (isChecked)
                attributes["checked"] = "checked";
            else
                attributes.Remove("checked");

            return InputBuilder(helper, "radio", name, value, attributes);
        }

        public static string RadioButton<TModel>(this HtmlControlHelper<TModel> helper,
            string name, object value, Expression<Func<TModel, bool>> isChecked)
            where TModel : class
        {
            return RadioButton(helper, name, value, isChecked, null);
        }

        public static string RadioButton<TModel>(this HtmlControlHelper<TModel> helper,
            string name, object value, Expression<Func<TModel, bool>> isChecked,
            object attributes)
            where TModel : class
        {
            return RadioButton(helper, name, value, isChecked, new ValueDictionary(attributes));
        }

        public static string RadioButton<TModel>(this HtmlControlHelper<TModel> helper,
            string name, object value, Expression<Func<TModel, bool>> isChecked,
            IDictionary<string, object> attributes)
            where TModel : class
        {
            Precondition.Require(isChecked, () => Error.ArgumentNull("isChecked"));

            TModel model = (TModel)helper.Context.ViewData.Model;
            Func<bool> accessor = LinqHelper.WrapModelAccessor(isChecked, model);

            return RadioButton(helper, name, value, accessor(), attributes);
        }
        #endregion

        #region Hidden
        public static string Hidden(this HtmlControlHelper helper,
            string name, object value)
        {
            return Hidden(helper, name, value, new ValueDictionary());
        }

        public static string Hidden(this HtmlControlHelper helper,
            string name, IDictionary<string, object> attributes)
        {
            return Hidden(helper, name, null, attributes);
        }

        public static string Hidden(this HtmlControlHelper helper,
            string name, object value, object attributes)
        {
            return Hidden(helper, name, value, new ValueDictionary(attributes));
        }

        public static string Hidden(this HtmlControlHelper helper, 
            string name, object value, IDictionary<string, object> attributes)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));

            if (helper.DataSource.Keys.Any(k => k.Equals(name, StringComparison.OrdinalIgnoreCase)))
                value = helper.DataSource.GetValue<object>(name);

            return InputBuilder(helper, "hidden", name, value, attributes);
        }

        public static string Hidden<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value)
            where TModel : class
        {
            return Hidden(helper, name, value, null);
        }

        public static string Hidden<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value, object attributes)
            where TModel : class
        {
            return Hidden(helper, name, value, new ValueDictionary(attributes));
        }

        public static string Hidden<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value, 
            IDictionary<string, object> attributes)
            where TModel : class
        {
            Precondition.Require(value, () => Error.ArgumentNull("value"));

            TModel model = (TModel)helper.Context.ViewData.Model;
            Func<object> func = LinqHelper.WrapModelAccessor(value, model);

            return Hidden(helper, name, func(), attributes);
        }
        #endregion

        #region TextBox
        public static string TextBox(this HtmlControlHelper helper, string name)
        {
            return TextBox(helper, name, null, new ValueDictionary());
        }

        public static string TextBox(this HtmlControlHelper helper, 
            string name, object value)
        {
            return TextBox(helper, name, value, new ValueDictionary());
        }

        public static string TextBox(this HtmlControlHelper helper, 
            string name, object value, object attributes)
        {
            return TextBox(helper, name, value, new ValueDictionary(attributes));
        }

        public static string TextBox(this HtmlControlHelper helper, 
            string name, object value, IDictionary<string, object> attributes)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));
            if(helper.DataSource.Keys.Any(c => String.Equals(c, name, 
                StringComparison.InvariantCultureIgnoreCase)))
                value = helper.DataSource.GetValue<object>(name);

            return InputBuilder(helper, "text", name, value, attributes);
        }

        public static string TextBox<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value)
            where TModel : class
        {
            return TextBox(helper, name, value, null);
        }

        public static string TextBox<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value,
            object attributes)
            where TModel : class
        {
            return TextBox(helper, name, value, new ValueDictionary(attributes));
        }

        public static string TextBox<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value, 
            IDictionary<string, object> attributes)
            where TModel : class
        {
            Precondition.Require(value, () => Error.ArgumentNull("value"));

            TModel model = (TModel)helper.Context.ViewData.Model;
            Func<object> func = LinqHelper.WrapModelAccessor(value, model);
            
            return TextBox(helper, name, func(), attributes);
        }
        #endregion

        #region Password
        public static string Password(this HtmlControlHelper helper, string name)
        {
            return Password(helper, name, null, new ValueDictionary());
        }

        public static string Password(this HtmlControlHelper helper,
            string name, object value)
        {
            return Password(helper, name, value, new ValueDictionary());
        }

        public static string Password(this HtmlControlHelper helper,
            string name, object value, object attributes)
        {
            return Password(helper, name, value, new ValueDictionary(attributes));
        }

        public static string Password(this HtmlControlHelper helper,
            string name, object value, IDictionary<string, object> attributes)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));
            return InputBuilder(helper, "password", name, value, attributes);
        }

        public static string Password<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value)
            where TModel: class
        {
            return Password(helper, name, value, null);
        }

        public static string Password<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value, object attributes)
            where TModel : class
        {
            return Password(helper, name, value, new ValueDictionary(attributes));
        }

        public static string Password<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, object>> value, 
            IDictionary<string, object> attributes)
            where TModel : class
        {
            Precondition.Require(value, () => Error.ArgumentNull("value"));

            TModel model = (TModel)helper.Context.ViewData.Model;
            Func<object> func = LinqHelper.WrapModelAccessor(value, model);

            return Password(helper, name, func(), attributes);
        }
        #endregion

        #region TextArea
        public static string TextArea(this HtmlControlHelper helper, string name)
        {
            return TextArea(helper, name, null, 2, 20, new ValueDictionary());
        }

        public static string TextArea(this HtmlControlHelper helper,
            string name, object attributes)
        {
            return TextArea(helper, name, null, 2, 20, new ValueDictionary(attributes));
        }

        public static string TextArea(this HtmlControlHelper helper,
            string name, IDictionary<string, object> attributes)
        {
            return TextArea(helper, name, null, 2, 20, attributes);
        }

        public static string TextArea(this HtmlControlHelper helper,
            string name, string value)
        {
            return TextArea(helper, name, value, 2, 20, new ValueDictionary());
        }

        public static string TextArea(this HtmlControlHelper helper,
            string name, string value, object attributes)
        {
            return TextArea(helper, name, value, 2, 20, new ValueDictionary(attributes));
        }

        public static string TextArea(this HtmlControlHelper helper,
            string name, string value, IDictionary<string, object> attributes)
        {
            return TextArea(helper, name, value, 2, 20, attributes);
        }

        public static string TextArea(this HtmlControlHelper helper,
            string name, string value, int rows, int columns, object attributes)
        {
            return TextArea(helper, name, value, rows, columns, new ValueDictionary(attributes));
        }

        public static string TextArea(this HtmlControlHelper helper, 
            string name, string value, int rows, int columns, IDictionary<string, object> attributes)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));
            if (helper.DataSource.Keys.Any(k => k.Equals(name, StringComparison.OrdinalIgnoreCase)))
                value = helper.DataSource.GetValue<string>(name);

            return TextAreaBuilder(helper, name, value, rows, columns, attributes);
        }

        public static string TextArea<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, string>> value)
            where TModel : class
        {
            return TextArea(helper, name, value, 2, 20, null);
        }

        public static string TextArea<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, string>> value, object attributes)
            where TModel : class
        {
            return TextArea(helper, name, value, 2, 20, new ValueDictionary(attributes));
        }

        public static string TextArea<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, string>> value, 
            IDictionary<string, object> attributes)
            where TModel : class
        {
            return TextArea(helper, name, value, 2, 20, attributes);
        }

        public static string TextArea<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, string>> value,
            int rows, int columns, object attributes)
            where TModel : class
        {
            return TextArea(helper, name, value, rows, columns, new ValueDictionary(attributes));
        }

        public static string TextArea<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, string>> value, 
            int rows, int columns, IDictionary<string, object> attributes)
            where TModel : class
        {
            Precondition.Require(value, () => Error.ArgumentNull("value"));

            TModel model = (TModel)helper.Context.ViewData.Model;
            Func<string> func = LinqHelper.WrapModelAccessor(value, model);

            return TextArea(helper, name, func(), rows, columns, attributes);
        }
        #endregion

        #region DropDownList
        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list)
        {
            return DropDownList(helper, name, list, null, null, null, null);
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list, object attributes)
        {
            return DropDownList(helper, name, list, null, 
                null, null, new ValueDictionary(attributes));
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list, IDictionary<string, object> attributes)
        {
            return DropDownList(helper, name, list, null, null, null, attributes);
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField,
            object attributes)
        {
            return DropDownList(helper, name, list, dataTextField,
                null, null, new ValueDictionary(attributes));
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField, 
            IDictionary<string, object> attributes)
        {
            return DropDownList(helper, name, list, dataTextField,
                null, null, attributes);
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField,
            string dataValueField, object attributes)
        {
            return DropDownList(helper, name, list, dataTextField,
                dataValueField, null, new ValueDictionary(attributes));
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField,
            string dataValueField, IDictionary<string, object> attributes)
        {
            return DropDownList(helper, name, list, dataTextField, 
                dataValueField, null, attributes);
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField,
            string dataValueField, object selectedItem, object attributes)
        {
            return DropDownList(helper, name, list, dataTextField, 
                dataValueField, selectedItem, new ValueDictionary(attributes));
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField, 
            string dataValueField, object selectedItem, IDictionary<string, object> attributes)
        {
            SelectList selectList = new SelectList(list ?? new object[0], 
                dataTextField, dataValueField, selectedItem);

            return DropDownList(helper, name, selectList, attributes);
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, SelectList list)
        {
            return SelectBuilder(helper, name, list, false, null);
        }

        public static string DropDownList(this HtmlControlHelper helper,
            string name, SelectList list, object attributes)
        {
            return SelectBuilder(helper, name, list, false, new ValueDictionary(attributes));
        }

        public static string DropDownList(this HtmlControlHelper helper, 
            string name, SelectList list, IDictionary<string, object> attributes)
        {
            return SelectBuilder(helper, name, list, false, attributes);
        }
        #endregion

        #region ListBox
        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list)
        {
            return ListBox(helper, name, list,
                null, null, null, null);
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list, object attributes)
        {
            return ListBox(helper, name, list, 
                null, null, null, new ValueDictionary(attributes));
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list, IDictionary<string, object> attributes)
        {
            return ListBox(helper, name, list, null, null, null, attributes);
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField,
            object attributes)
        {
            return ListBox(helper, name, list, dataTextField, 
                null, null, new ValueDictionary(attributes));
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField, 
            IDictionary<string, object> attributes)
        {
            return ListBox(helper, name, list, dataTextField, null, null, attributes);
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField,
            string dataValueField, object attributes)
        {
            return ListBox(helper, name, list, dataTextField, 
                dataValueField, null, new ValueDictionary(attributes));
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField,
            string dataValueField, IDictionary<string, object> attributes)
        {
            return ListBox(helper, name, list, dataTextField, 
                dataValueField, null, attributes);
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField,
            string dataValueField, IEnumerable selectedItems,
            object attributes)
        {
            MultiSelectList selectList = new MultiSelectList(list,
                dataTextField, dataValueField, selectedItems);
            return ListBox(helper, name, selectList, new ValueDictionary(attributes));
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, IEnumerable list, string dataTextField, 
            string dataValueField, IEnumerable selectedItems, 
            IDictionary<string, object> attributes)
        {
            MultiSelectList selectList = new MultiSelectList(list,
                dataTextField, dataValueField, selectedItems);
            return ListBox(helper, name, selectList, attributes);
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, MultiSelectList list)
        {
            return ListBox(helper, name, list, null);
        }

        public static string ListBox(this HtmlControlHelper helper,
            string name, MultiSelectList list, object attributes)
        {
            return ListBox(helper, name, list, new ValueDictionary(attributes));
        }

        public static string ListBox(this HtmlControlHelper helper, 
            string name, MultiSelectList list, IDictionary<string, object> attributes)
        {
            return SelectBuilder(helper, name, list, true, attributes);
        }
        #endregion
    }
}