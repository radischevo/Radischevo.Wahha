using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.UI.WebControls;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Configurations;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public class TemplateHelper
    {
        #region Nested Types
        private class ActionCacheItem
        {
            #region Instance Fields
            private Func<HtmlHelper, ViewDataDictionary, string> _action;
            #endregion

            #region Constructors
            public ActionCacheItem(string viewName)
            {
                _action = (html, viewData) => {
                    ViewEngineResult result = Configuration.Instance
                        .Views.ViewEngines.FindView(html.Context, viewName);                    

                    result.View.Render(new ViewContext(html.Context, result.View, 
                        viewData, html.Context.TempData), html.Context.Context.Response.Output);
                    return String.Empty;
                };
            }
            
            public ActionCacheItem(Func<HtmlHelper, string> action)
            {
                _action = (html, viewData) => {
                    HtmlHelper helper = new HtmlHelper(new ViewContext(html.Context,
                        html.Context.View, viewData, html.Context.TempData));
                    helper.Controls.DataSource = html.Controls.DataSource;

                    return action(helper);
                };
            }
            #endregion

            #region Instance Methods
            public string Execute(HtmlHelper helper, 
                ViewDataDictionary viewData)
            {
                return _action(helper, viewData);
            }
            #endregion
        }
        #endregion

        #region Static Fields
        static string _cacheItemId = Guid.NewGuid().ToString();
        private static HashSet<Type> _convertibleToString = new HashSet<Type>();
        private static Dictionary<DataBoundControlMode, string> _modeViewPaths = 
            new Dictionary<DataBoundControlMode, string>();
        private static Dictionary<string, Func<HtmlHelper, string>> _displayActions =
            new Dictionary<string, Func<HtmlHelper, string>>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Func<HtmlHelper, string>> _editorActions =
            new Dictionary<string, Func<HtmlHelper, string>>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region Instance Fields
        private HtmlHelper _helper;
        #endregion

        #region Constructors
        static TemplateHelper()
        {
            _convertibleToString.Add(typeof(float));
            _convertibleToString.Add(typeof(double));
            _convertibleToString.Add(typeof(decimal)); 
            _convertibleToString.Add(typeof(short)); 
            _convertibleToString.Add(typeof(byte)); 
            _convertibleToString.Add(typeof(long));
            _convertibleToString.Add(typeof(int)); 
            _convertibleToString.Add(typeof(char)); 
            _convertibleToString.Add(typeof(Guid)); 
            _convertibleToString.Add(typeof(DateTime));
            _convertibleToString.Add(typeof(DateTimeOffset));
            _convertibleToString.Add(typeof(TimeSpan));

            _modeViewPaths.Add(DataBoundControlMode.ReadOnly, "views/templates/display");
            _modeViewPaths.Add(DataBoundControlMode.Edit, "views/templates/edit");

            _displayActions.Add("EmailAddress", DefaultDisplayTemplates.EmailAddress);
			_displayActions.Add("Collection", DefaultDisplayTemplates.Collection);
            _displayActions.Add("HiddenInput", DefaultDisplayTemplates.HiddenInput);
            _displayActions.Add("Html", DefaultDisplayTemplates.HtmlString);
            _displayActions.Add("Text", DefaultDisplayTemplates.String);
            _displayActions.Add("Url", DefaultDisplayTemplates.Hyperlink);
            _displayActions.Add(typeof(bool).Name, DefaultDisplayTemplates.Boolean);
			_displayActions.Add(typeof(decimal).Name, DefaultDisplayTemplates.Decimal);
            _displayActions.Add(typeof(string).Name, DefaultDisplayTemplates.String);
            _displayActions.Add(typeof(object).Name, DefaultDisplayTemplates.Object);

            _editorActions.Add("HiddenInput", DefaultEditorTemplates.Hidden);
            _editorActions.Add("Html", DefaultEditorTemplates.TextArea);
			_editorActions.Add("Collection", DefaultEditorTemplates.Collection);
            _editorActions.Add("MultilineText", DefaultEditorTemplates.TextArea);
            _editorActions.Add("Password", DefaultEditorTemplates.Password);
            _editorActions.Add("Text", DefaultEditorTemplates.String);
			_editorActions.Add("Enum", DefaultEditorTemplates.Enumeration);
            _editorActions.Add(typeof(bool).Name, DefaultEditorTemplates.Boolean);
			_editorActions.Add(typeof(decimal).Name, DefaultEditorTemplates.Decimal);
            _editorActions.Add(typeof(string).Name, DefaultEditorTemplates.String);
            _editorActions.Add(typeof(object).Name, DefaultEditorTemplates.Object);
        }

        public TemplateHelper(HtmlHelper helper)
        {
            Precondition.Require(helper, () => Error.ArgumentNull("helper"));
            _helper = helper;
        }
        #endregion

        #region Instance Properties
        public ViewContext Context
        {
            get
            {
                return _helper.Context;
            }
        }

        private Dictionary<string, ActionCacheItem> ActionCache
        {
            get
            {
                HttpContextBase context = Context.Context;
                Dictionary<string, ActionCacheItem> result;

                if (!context.Items.Contains(_cacheItemId))
                {
                    result = new Dictionary<string, ActionCacheItem>();
                    context.Items[_cacheItemId] = result;
                }
                else
                    result = (Dictionary<string, ActionCacheItem>)context.Items[_cacheItemId];
                
                return result;
            }
        }
        #endregion

        #region Static Methods
        private static IEnumerable<string> GetTemplateNames(ModelMetadata metadata, string templateName)
        {
            if (!String.IsNullOrEmpty(templateName))
                yield return templateName;

            if (!String.IsNullOrEmpty(metadata.TemplateHint))
                yield return metadata.TemplateHint;

            Type type = metadata.Type.MakeNonNullableType();
            yield return type.Name;

			if (type.IsEnum)
				yield return "Enum";

            if (_convertibleToString.Contains(type))
			{
                yield return "String";
			}
			else if (type.IsInterface)
			{
				if (typeof(IEnumerable).IsAssignableFrom(type))
					yield return "Collection";

				yield return "Object";
			}
			else
			{
				bool isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);

				while (true)
				{
					type = type.BaseType;
					if (type == null)
						break;

					if (isEnumerable && type == typeof(Object))
						yield return "Collection";

					yield return type.Name;
				}
			}
        }

        internal static ModelMetadata GetMetadata(Type containerType, Type modelType, string propertyName)
        {
            return (containerType != null && !String.IsNullOrEmpty(propertyName))
                ? Configuration.Instance.Models.MetadataProvider.GetMetadata(containerType)
					.Properties.Single(p => String.Equals(p.PropertyName, propertyName, 
                        StringComparison.InvariantCultureIgnoreCase))
                : Configuration.Instance.Models.MetadataProvider.GetMetadata(modelType);
        }
        #endregion

        #region Instance Methods
        internal string Render(DataBoundControlMode mode, string templateName,
            string expression, Type parentModelType, Type modelType,
            string propertyName, object value)
        {
            ModelMetadata metadata = GetMetadata(parentModelType, modelType, propertyName);
            return Render(mode, metadata, templateName, expression, value);
        }

        internal string Render(DataBoundControlMode mode, ModelMetadata metadata,
            string templateName, string expression, object value)
        {
            object visitedObject = value ?? metadata.Type;
            if (_helper.Context.ViewData.Template.VisitedObjects
                .Contains(visitedObject))
                return String.Empty;

            object formattedValue = value;
            string formatString = (mode == DataBoundControlMode.ReadOnly) ? 
                metadata.DisplayFormat : metadata.EditFormat;

            if (metadata.ConvertEmptyStringToNull && String.Empty.Equals(value))
                value = null;

            if (value == null)
                formattedValue = metadata.NullDisplayText;
            else if (!String.IsNullOrEmpty(formatString))
                formattedValue = String.Format(
                    CultureInfo.InvariantCulture, formatString, value);
            
            Dictionary<string, Func<HtmlHelper, string>> defaultActions = 
                (mode == DataBoundControlMode.Edit) ? _editorActions : _displayActions;

            string viewPath = _modeViewPaths[mode];
            Type underlyingType = Nullable.GetUnderlyingType(metadata.Type);

            ViewDataDictionary viewData = new ViewDataDictionary(Context.ViewData);
            
            viewData.Model = value;
            viewData.Metadata = metadata;
            viewData.Template = new TemplateDescriptor(metadata.Type, formattedValue);
            viewData.Template.Prefix = Context.ViewData.Template.GetHtmlElementName(expression);
            viewData.Template.IsNullableType = (underlyingType != null);
            viewData.Template.VisitedObjects = new HashSet<object>(_helper.Context.ViewData.Template.VisitedObjects);

            viewData.Template.VisitedObjects.Add(visitedObject);

            foreach (string viewName in GetTemplateNames(metadata, templateName))
            {
                string fullViewName = viewPath + "/" + viewName;
                ActionCacheItem cacheItem;

                if (ActionCache.TryGetValue(fullViewName, out cacheItem))
                {
                    if (cacheItem != null)
                    {
                        return cacheItem.Execute(_helper, viewData);
                    }
                }
                else
                {
                    ViewEngineResult result = Configuration.Instance
                        .Views.ViewEngines.FindView(Context, fullViewName);

                    if (result != null && result.View != null)
                    {
                        ActionCache[fullViewName] = new ActionCacheItem(fullViewName);
                        StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
                        result.View.Render(new ViewContext(Context, result.View, viewData, 
                            Context.TempData), writer);

                        return writer.ToString();
                    }

                    Func<HtmlHelper, string> defaultAction;
                    if (defaultActions.TryGetValue(viewName, out defaultAction))
                    {
                        ActionCache[fullViewName] = new ActionCacheItem(defaultAction);
                        HtmlHelper helper = new HtmlHelper(new ViewContext(
                            Context, Context.View, viewData, Context.TempData));
                        helper.Controls.DataSource = _helper.Controls.DataSource;

                        return defaultAction(helper);
                    }
                    ActionCache[fullViewName] = null;
                }
            }
            return String.Empty;
        }

        protected string Template(string expression, string templateName, 
            string htmlElementName, DataBoundControlMode mode)
        {
            Type containerType = null;
            Type modelType = null;
            object modelValue = null;
            string propertyName = null;
 
            ViewDataInfo vdi = Context.ViewData.GetViewDataInfo(expression);

            if (vdi != null)
            {
                containerType = vdi.Container.GetType();

                if (vdi.Descriptor != null)
                {
                    propertyName = vdi.Descriptor.Name;
                    modelType = vdi.Descriptor.PropertyType;
                }

                modelValue = vdi.Value;
                if (modelValue != null)
                    modelType = modelValue.GetType();
            }

            return Render(mode, templateName, htmlElementName ?? expression, 
                containerType, modelType ?? typeof(string), propertyName, modelValue);
        }

        protected string Template<TModel, TValue>(Expression<Func<TModel, TValue>> expression, 
            string templateName, string htmlElementName, DataBoundControlMode mode) 
            where TModel : class
        {
            object modelValue = LinqHelper.WrapModelAccessor(
                expression, (TModel)Context.ViewData.Model)();

            Type modelType = typeof(TValue);
            Type parentModelType = null;
            string propertyName = null;

            switch (expression.Body.NodeType)
            {
				case ExpressionType.ArrayIndex:
					break;

                case ExpressionType.Parameter:
                    break;

				case ExpressionType.Call:
					if (!LinqHelper.IsSingleArgumentIndexer(expression.Body))
						throw Error.TemplateExpressionLimitations();
					break;

                case ExpressionType.MemberAccess:
                    MemberExpression memberExpression = (MemberExpression)expression.Body;
                    propertyName = (memberExpression.Member is PropertyInfo) ? memberExpression.Member.Name : null;
                    parentModelType = memberExpression.Member.DeclaringType;
                    break;

                default:
                    throw Error.TemplateExpressionLimitations();
            }
            return Render(mode, templateName, htmlElementName ?? LinqHelper.GetExpressionText(expression), 
                parentModelType, modelType, propertyName, modelValue);
        }
        #endregion

        #region Display Methods
        public string Display(string expression)
        {
            return Display(expression, null, null);
        }

        public string Display(string expression, string templateName)
        {
            return Display(expression, templateName, null);
        }

        public string Display(string expression, string templateName, 
            string htmlElementName)
        {
            return Template(expression, templateName, htmlElementName, 
                DataBoundControlMode.ReadOnly);
        }
        #endregion

        #region Editor Methods
        public string Editor(string expression)
        {
            return Editor(expression, null, null);
        }

        public string Editor(string expression, 
            string templateName)
        {
            return Editor(expression, templateName, null);
        }

        public string Editor(string expression, 
            string templateName, string htmlElementName)
        {
            return Template(expression, templateName, htmlElementName, 
                DataBoundControlMode.Edit);
        }
        #endregion
    }

    public class TemplateHelper<TModel> : TemplateHelper
        where TModel : class
    {
        #region Constructors
        public TemplateHelper(HtmlHelper<TModel> helper)
            : base(helper)
        {
        }
        #endregion

        #region Display Methods
        public string Display<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return Display(expression, null, null);
        }

        public string Display<TValue>(Expression<Func<TModel, TValue>> expression,
            string templateName)
        {
            return Display(expression, templateName, null);
        }

        public string Display<TValue>(Expression<Func<TModel, TValue>> expression,
            string templateName, string htmlElementName)
        {
            return Template(expression, templateName, htmlElementName, DataBoundControlMode.ReadOnly);
        }
        #endregion

        #region Editor Methods
        public string Editor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return Editor(expression, null, null);
        }

        public string Editor<TValue>(Expression<Func<TModel, TValue>> expression,
            string templateName)
        {
            return Editor(expression, templateName, null);
        }

        public string Editor<TValue>(Expression<Func<TModel, TValue>> expression,
            string templateName, string htmlElementName)
        {
            return Template(expression, templateName, htmlElementName,
                DataBoundControlMode.Edit);
        }
        #endregion
    }
}
