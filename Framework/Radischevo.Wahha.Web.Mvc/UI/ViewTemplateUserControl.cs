using System;

namespace Radischevo.Wahha.Web.Mvc.UI
{
    public class ViewTemplateUserControl<TModel> : ViewUserControl<TModel> 
        where TModel : class
    {
        protected string FormattedValue
        {
            get 
            { 
                return ViewData.Template.Value.ToString(); 
            }
        }
    }

    public class ViewTemplateUserControl : ViewTemplateUserControl<object> 
    {
    }
}
