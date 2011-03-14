using System;

namespace Radischevo.Wahha.Web.Text
{
    public interface IFluentAttributeRule : IRuleBuilder
    {
        IFluentAttributeRule As(HtmlAttributeOptions options);

        IFluentAttributeRule Convert(HtmlAttributeConverter converter);

        IFluentAttributeRule Default(string defaultValue);

        IFluentAttributeRule Default(object defaultValue);

        IFluentAttributeRule Validate(string pattern);

		IFluentAttributeRule Validate(params string[] values);
    }
}
