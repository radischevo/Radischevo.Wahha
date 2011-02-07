using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Expressions;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public static class BinaryHtmlExtensions
    {
        public static string Hidden(this HtmlControlHelper helper,
            string name, byte[] value)
        {
            return Hidden(helper, name, value, new ValueDictionary());
        }

        public static string Hidden(this HtmlControlHelper helper,
            string name, byte[] value, object attributes)
        {
            return Hidden(helper, name, value, new ValueDictionary(attributes));
        }

        public static string Hidden(this HtmlControlHelper helper,
            string name, byte[] value, IDictionary<string, object> attributes)
        {
            return FormExtensions.Hidden(helper, name, Convert.ToBase64String(value), attributes);
        }

        public static string Hidden<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, byte[]>> value)
            where TModel: class
        {
            return Hidden(helper, name, value, null);
        }

        public static string Hidden<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, byte[]>> value, object attributes)
            where TModel : class
        {
            return Hidden(helper, name, value, new ValueDictionary(attributes));
        }

        public static string Hidden<TModel>(this HtmlControlHelper<TModel> helper,
            string name, Expression<Func<TModel, byte[]>> value,
            IDictionary<string, object> attributes)
            where TModel : class
        {
            Precondition.Require(value, () => Error.ArgumentNull("value"));

            TModel model = (TModel)helper.Context.ViewData.Model;
            Func<byte[]> func = LinqHelper.WrapModelAccessor(value, model);

            return Hidden(helper, name, func(), attributes);
        }
    }
}
