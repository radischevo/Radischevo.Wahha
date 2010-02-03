using System;
using System.Globalization;
using System.Web;
using System.Web.Compilation;

namespace Radischevo.Wahha.Web.Mvc
{
    internal static class ResourceHelper
    {
        #region Helper Methods
        internal static string GetResourceString(string expression, 
            string virtualPath, object[] args)
        {
            ExpressionBuilderContext context = new ExpressionBuilderContext(virtualPath);
            ResourceExpressionBuilder builder = new ResourceExpressionBuilder();
            ResourceExpressionFields fields = (ResourceExpressionFields)builder.ParseExpression(
                expression, typeof(string), context);

            if (!String.IsNullOrEmpty(fields.ClassKey))
                return String.Format((string)HttpContext.GetGlobalResourceObject(fields.ClassKey, 
                    fields.ResourceKey, CultureInfo.CurrentUICulture), args);

            return String.Format((string)HttpContext.GetLocalResourceObject(virtualPath, 
                fields.ResourceKey, CultureInfo.CurrentUICulture), args);
        }
        #endregion
    }
}
