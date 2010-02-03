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
using Radischevo.Wahha.Web.Mvc.Validation;
using Radischevo.Wahha.Web.Abstractions;
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
        if (String.Equals("element", title, StringComparison.OrdinalIgnoreCase))
            return RemoteValidationResult.Success();

        return RemoteValidationResult.Failure();
    }

    [HttpCompression, HttpCache(Duration = 600, 
        Cacheability = HttpCacheability.Public,
        Revalidation = HttpCacheRevalidation.None)]
    public ActionResult Messages(int page)
    {
        using (SmsRepository repository = new SmsRepository())
        {
            return View("Views/messages", repository.Select(page));
        }
    }

	public ActionResult Posts(int count)
	{
		/* Тестируем DataProvider
		using (DbDataProvider provider = DbDataProvider.Create<SqlDbDataProvider>(
			@"Initial Catalog=wahha.test;Data Source=iceberg\sqlexpress;Packet Size=4096;User ID=asp;Password=asp"))
		{
			int ic = provider.Execute("SELECT COUNT([id]) FROM [dbo].[wahha.items]").AsScalar<int>();

			int id1 = provider.Execute("INSERT INTO [dbo].[wahha.items] ([title], [description]) VALUES({0}, {1}); SELECT @@identity",
				new object[] { "Элемент 1", "First" }).AsScalar<int>();
			int id2 = provider.Execute("INSERT INTO [dbo].[wahha.items] ([title], [description]) VALUES({0}, {1}); SELECT @@identity",
				new object[] { "Элемент 2", "Second" }).AsScalar<int>();

			var list = provider.Execute("SELECT [id], [title], [description] FROM [dbo].[wahha.items]")
				.Using(CommandBehavior.CloseConnection)
				.AsDataReader().Select(r => new {
					Id = r.GetValue<int>("id"), Title = r.GetValue<string>("Title"), Description = r.GetValue<string>("Description")
				}).ToList();

			provider.Close();

			var list2 = provider.Execute("SELECT TOP 2 [id], [title], [description] FROM [dbo].[wahha.items]")
				.Using(CommandBehavior.CloseConnection)
				.AsDataReader(reader => {
					return reader.Select(r => new {
						Id = r.GetValue<int>("id"), Title = r.GetValue<string>("Title"), Description = r.GetValue<string>("Description")
					}).ToList();
				});

			var list3 = provider.Execute("SELECT TOP 3 [id], [title], [description] FROM [dbo].[wahha.items]")
				.Using(CommandBehavior.SingleRow)
				.AsDataReader(reader => {
					return reader.Select(r => new {
						Id = r.GetValue<int>("id"), Title = r.GetValue<string>("Title"), Description = r.GetValue<string>("Description")
					}).ToList();
				});
		}
		*/
		BlogPostRepository blogs = new BlogPostRepository();
		var list = blogs.Last(count);
		var item = blogs.Single(1352);
		int c1 = list.Total();
		
		return View("bar", list);
	}
}

public class MegaController : AsyncController
{
	public void MessagesAsync(int page)
	{
		MessageInfo mi = new MessageInfo() {
			Date = DateTime.Now, Direction = MessageDirection.Incoming, 
			From = "Ксю", To = "Я", Message = "Привет! :)"
		};

		ViewData.Model = mi.ToEnumerable();
	}

	public ActionResult MessagesCompleted()
	{
		return View("Views/messages");
	}

	public ActionResult SimpleAction()
	{
		MessageInfo mi = new MessageInfo() {
			Date = DateTime.Now, Direction = MessageDirection.Incoming,
			From = "Ксю", To = "Я", Message = "Привет! :)"
		};

		return View("Views/messages", mi.ToEnumerable());
	}
}
