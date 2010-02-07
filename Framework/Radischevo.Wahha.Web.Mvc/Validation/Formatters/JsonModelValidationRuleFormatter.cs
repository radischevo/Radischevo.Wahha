using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class JsonModelValidationRuleFormatter : IModelValidationRuleFormatter
    {
        #region Constructors
        public JsonModelValidationRuleFormatter()
        {
        }
        #endregion

        #region Instance Methods
        public string Format(string selector, 
            IEnumerable<ClientModelValidationRule> rules)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            SortedDictionary<string, object> dict = new SortedDictionary<string, object>();

            dict.Add("Fields", rules.ToLookup(k => k.Field, StringComparer.OrdinalIgnoreCase)
                .Select(g => new { Field = g.Key, Rules = g.ToList() }));
            dict.Add("Selector", selector);

            return serializer.Serialize(dict);
        }
        #endregion
    }
}
