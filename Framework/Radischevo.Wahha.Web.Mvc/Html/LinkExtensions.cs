using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.UI;
using Radischevo.Wahha.Web.Text;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public static class LinkExtensions
    {
        #region Helper Methods
        private static string LinkBuilder(HtmlControlHelper helper,
            string url, string text, IDictionary<string, object> attributes)
        {
            HtmlElementBuilder builder = new HtmlElementBuilder("a");
            
            if (attributes == null)
                attributes = new ValueDictionary();

            builder.Attributes.Merge(attributes);
            builder.Attributes.Merge("href", url, true);
            builder.InnerHtml = text;

            return builder.ToString();
        }
        #endregion

        #region Static Extension Methods
        public static string Link(this HtmlControlHelper helper,
            string url, string text)
        {
            return Link(helper, url, text, null);
        }

        public static string Link(this HtmlControlHelper helper,
            string url, string text, object attributes)
        {
            return Link(helper, url, text, new ValueDictionary(attributes));
        }

        public static string Link(this HtmlControlHelper helper,
            string url, string text, IDictionary<string, object> attributes)
        {
            return LinkBuilder(helper, url, text, attributes);
        }

        public static string Link<TController>(this HtmlControlHelper helper,
            Expression<Action<TController>> action, string text)
            where TController : IController
        {
            return Link(helper, action, text, null);
        }

        public static string Link<TController>(this HtmlControlHelper helper,
            Expression<Action<TController>> action, string text, object attributes)
            where TController : IController
        {
            return Link(helper, action, text, new ValueDictionary(attributes));
        }

        public static string Link<TController>(this HtmlControlHelper helper,
            Expression<Action<TController>> action, string text, 
            IDictionary<string, object> attributes)
            where TController : IController
        {
            UrlHelper url = new UrlHelper(helper.Context);
            return Link(helper, url.Route<TController>(action), text, attributes);
        }
        #endregion
    }
}
