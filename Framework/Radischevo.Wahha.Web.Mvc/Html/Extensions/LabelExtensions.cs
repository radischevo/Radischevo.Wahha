using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public static class LabelExtensions
    {
        #region Helper Methods
        internal static string Render(ViewContext context, string expression, ModelMetadata metadata)
        {
            string labelText = metadata.DisplayName ?? expression ?? metadata.Type.Name;
            string elementName = context.ViewData.Template
                .GetHtmlElementName(expression);

            if (String.IsNullOrEmpty(labelText))
                return String.Empty;

            HtmlElementBuilder builder = new HtmlElementBuilder("label",
                new { @for = elementName }, labelText);

            return builder.ToString();
        }

        internal static string Render(ViewContext context, string expression,
            Type containerType, string propertyName, Type modelType)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            ModelMetadata metadata = TemplateHelper.GetMetadata(containerType, 
                modelType, propertyName);

            return Render(context, expression, metadata);
        }
        #endregion

        #region Extension Methods
        public static string Label(this HtmlControlHelper helper, string expression)
        {
            Precondition.Defined(expression, () => Error.ArgumentNull("expression"));

            Type containerType = null;
            Type modelType = null;
            string propertyName = null;
            ViewDataInfo vdi = helper.Context.ViewData.GetViewDataInfo(expression);

            if (vdi != null)
            {
                containerType = vdi.Container.GetType();

                if (vdi.Descriptor != null)
                {
                    propertyName = vdi.Descriptor.Name;
                    modelType = vdi.Descriptor.PropertyType;
                }

                if (vdi.Value != null)
                    modelType = vdi.Value.GetType();
            }
            return Render(helper.Context, expression, containerType, propertyName, modelType);
        }

        public static string Label<TModel, TValue>(this HtmlControlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression)
            where TModel : class
        {
            Precondition.Require(expression.Body.NodeType == ExpressionType.MemberAccess,
				() => Error.TemplateExpressionLimitations());

            MemberExpression me = (MemberExpression)expression.Body;
            PropertyInfo propertyInfo = (me.Member as PropertyInfo);

            return Render(helper.Context, LinqHelper.GetExpressionText(expression),
                   me.Member.DeclaringType, (propertyInfo == null) ? null :
                   propertyInfo.Name, typeof(TValue));
        }
        #endregion
    }
}
