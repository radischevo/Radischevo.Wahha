using System;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;
using Radischevo.Wahha.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Radischevo.Wahha.Web.Scripting.Templates;
using Radischevo.Wahha.Web.Mvc.Async;
using System.Collections.Generic;
using Radischevo.Wahha.Data.Provider;

/// <summary>
/// Summary description for MainController
/// </summary>
public class OtherController : Controller
{
    #region Nested Types
    public class RegistrationForm
    {
        [Required(ErrorMessage = "login-empty")]
        [RegularExpression(@"^[a-zA-Z_]+[a-zA-Z0-9\-_]{2,19}$",
            ErrorMessage = "login-invalid")]
        public string Login { get; set; }
        [Required(ErrorMessage = "email-empty")]
        [RegularExpression(@"^(\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)$",
            ErrorMessage = "email-invalid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "password-empty")]
        public string Password { get; set; }
        public string Confirmation { get; set; }
    }
    #endregion

    public OtherController()
    {
    }

    [AcceptHttpVerbs(Radischevo.Wahha.Web.HttpMethod.Get)]
	[HttpCompression]
    public ActionResult Register()
    {
        /*Test passed
        string str = "Wahha 3.0";
        byte[] enc = System.Text.Encoding.UTF8.GetBytes(str);
        var pair = Converter.ToBase16String(enc);
        var lpair = Converter.ToBase16String(enc, true);

        byte[] bytes = Converter.FromBase16String(lpair);
        string res = System.Text.Encoding.UTF8.GetString(bytes);
        */

		Include<OtherController>(c => c.Bar());
        return View("Views/Users/register", new RegistrationForm());
    }

    [AcceptHttpVerbs(Radischevo.Wahha.Web.HttpMethod.Post)]
    public ActionResult Register(RegistrationForm form)
    {
        return View("Views/Users/register", form);
    }

	public ActionResult Bar()
	{
		var ctx = Context.Parent;
		return this.View("Bar", ctx.RouteData.GetValue<string>("controller"));
	}

    public ActionResult Foo(int id)
    {
        ViewData["Bunch"] = Person.GetTempBunch(id);
        return this.View("Foo");        
    }

    public ActionResult Gallery()
    {
        Response.Write("Gallery");
        return this.View("Foo");
    }

    public ActionResult CheckItemTitle([Bind(Name = "item-title")]string title)
    {
		return Content("true");
    }
}

public class MegaController : AsyncController
{
}
