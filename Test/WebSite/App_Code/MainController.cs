using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Radischevo.Wahha.Web.Mvc;
using Radischevo.Wahha.Web.Routing;
using System.Text.RegularExpressions;
using Radischevo.Wahha.Web.Text;
using System.Collections.Specialized;
using System.Collections.Generic;
using Radischevo.Wahha.Web;
using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text.Sgml;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using System.Diagnostics;
using System.Linq.Expressions;
using Radischevo.Wahha.Data;
using Radischevo.Wahha.Core.Expressions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Radischevo.Wahha.Web.Mvc.Validation;
using Radischevo.Wahha.Web.Abstractions;
using System.Text;
using Radischevo.Wahha.Web.Scripting.Serialization;

public enum Status
{
	Active = 0, 
	Blocked = 1,
	Moderated = 2
}

[Flags]
public enum AccessModes : int
{
	None = 0,
	Read = 1,
	Edit = 2,
	Delete = 4,
	ChangePermissions = 8
}

[AttributeUsage(AttributeTargets.Method)]
public class MazaAttribute : ActionFilterAttribute
{
	public override void OnExecuting(ActionExecutionContext context)
	{
		context.Context.Parameters["maza"] = "Ksu";
		base.OnExecuting(context);
	}
}

[DisplayColumn("ID")]
public class TemplatedItem : IDataErrorInfo
{
	private List<int> _indices = new List<int>();

    [HiddenInput(DisplayValue = false)]
    public int ID { get; set; }
    [DisplayName("Элемент активен")]
	[FieldOrder(2)]
    public bool IsActive { get; set; }
    [DisplayName("Заголовок")]
	[FieldOrder(1)]
    [Required(ErrorMessage = "Надо заполнить")]
    public string Title { get; set; }
    [DisplayName("Статус просмотра")]
    public bool? IsViewed { get; set; }
    [DisplayName("Число комментариев")]
    [Range(10, 50, ErrorMessage = "Pizdec! Out of range!")]
    public int Count { get; set; }
    [DisplayName("Дата публикации")]
    public DateTime Date { get; set; }
    [DisplayName("Сообщение")]
    [UIHint("Html")]
    [Required(ErrorMessage = "Pizdec! Message is required")]
    [StringLength(50, ErrorMessage = "Pizdec! Too long string!")]
    public string Message { get; set; }
    [DisplayName("Вложенный элемент")]
    [DisplayFormat(NullDisplayText = "(Not set)")]
    public TemplatedItem Inner { get; set; }
	[DisplayName("Номера")]
	public List<int> Indices
	{
		get
		{
			return _indices;
		}
	}

	public Status Status
	{
		get;
		set;
	}

	#region IDataErrorInfo Members
	public string Error
	{
		get
		{
			return null;
		}
	}

	public string this[string columnName]
	{
		get
		{
			if (columnName == "Title")
				return "Zhopa is an uncorrect title.";

			return null;
		}
	}
	#endregion
}

/// <summary>
/// Summary description for MainController
/// </summary>
//[OutputMessage(Message = "<h3>Превед, я MainController</h3>")]
[HttpCompression]
public class MainController : Controller
{
    public class Maza
    {
        private Link<Section> _section;
        private EnumerableLink<string> _collection;

        public Maza()
        {
            _section = new Link<Section>();
            _collection = new EnumerableLink<string>();
        }

        public Link<Section> Section
        {
            get
            {
                return _section;
            }
        }

        public EnumerableLink<string> Collection
        {
            get
            {
                return _collection;
            }
        }
    }

    public ActionResult SampleComponent(Section section)
    {
        return Content(section.Name);
    }

    public MainController()
    {
    }

	public ActionResult Database()
	{
		return EmptyResult.Instance;
	}

    //[ActionCache(Duration = 15)]
    public ActionResult Default()
    {
        // Тестируем lazy-link...
        Maza m = new Maza();
        m.Section.Value = new Section("MazaFaza");
        m.Section.Source = () => new Section("Maza!");

        m.Section.Load(); 
        Section s = m.Section.Value;

        // Тестируем lazy-enumerable...
        /*m.Collection.Source = () => {
            using (DbDataProvider provider = DbDataProvider.Resolve("sql"))
            {
                using (IDbDataReader reader = provider.Execute("Select * From Items_1").AsDataReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32("id");
                        string t = reader.GetString("title");

                        var unassignedFields = reader.Keys.Except(reader.AccessedFields, 
                            StringComparer.OrdinalIgnoreCase);
                    }
                    return reader.Select(r => r.GetValue<string>("title"));
                }
            }
        };
        m.Collection.Count = 50;

        var lst = m.Collection.ToList();*/

		Include<MainController>(c => c.List(10));

        Maza maza1 = new Maza();
        maza1.Section.Value = new Section("maza1");

        Maza maza2 = new Maza();
        maza2.Section.Value = new Section("maza2");

        Func<Maza,string> func1 = CachedExpressionCompiler.Compile<Maza, string>(p => p.Section.Value.Name);
        Func<Maza, string> func2 = CachedExpressionCompiler.Compile<Maza, string>(p => p.Section.Value.Name);

        string maza1Name = func1(maza1);
        string maza2Name = func1(maza2);
        string maza3Name = func2(maza1);

        // Тестируем Interval
        Interval<int> ints = new Interval<int>(5, 10);
        ints.From = 35;
        ints.To = 25;

        ints.Normalize();

        //Response.ContentType = "application/xhtml+xml";

        return View("Default", new TemplatedItem() { Count = 10, Date = DateTime.Now, 
            IsActive = true, IsViewed = null, Message = "Йоу, товарищи!" });
    }

    public void List(int page)
    {
		//throw new HttpException(404, "Not found");
        Response.Write(page);
    }

    [AcceptHttpVerbs(HttpMethod.Get | HttpMethod.Head)]
    public ActionResult TestSgml()
    {
        return View("Test");
    }

    [AcceptHttpVerbs(HttpMethod.Get)]
    public ActionResult TemplatedItemTest()
    {
        Radischevo.Wahha.Web.Mvc.Configurations.Configuration
            .Instance.Models.MetadataProviders
            .Default = new DataAnnotationsMetadataProvider();

		Radischevo.Wahha.Web.Mvc.Configurations.Configuration
            .Instance.Models.ValidatorProviders
            .Default = new DataAnnotationsValidatorProvider();

		var ti = new TemplatedItem() {
			ID = 500,
			Count = 10,
			Date = DateTime.Now,
			IsActive = true,
			IsViewed = null,
			Message = "Йоу, товарищи!",
			Inner = new TemplatedItem() {
				ID = 5561,
				Count = 17,
				Date = DateTime.Now,
				IsActive = true,
				IsViewed = true,
				Message = "Вложения приветствуются",
				Inner = new TemplatedItem() {
					ID = 12449,
					Count = 28,
					Date = DateTime.Now,
					IsActive = false,
					IsViewed = true,
					Message = "А вот эти уже нет"
				}
			}
		};
		ti.Indices.Add(5);
		ti.Indices.Add(6);
		ti.Indices.Add(7);
		ti.Indices.Add(9);

        return View("template", ti);
    }

    [AcceptHttpVerbs(HttpMethod.Post)]
    public ActionResult TemplatedItemTest([Bind(Name="item-id")]int id)
    {
		TemplatedItem item = new TemplatedItem();
		item = BindModel(item, "item");
		Errors.Add("item-title", "Заголовок слишком короткий");
		Errors.Add("item-title", "Текст чересчур тупой");

        return View("template", item);
    }

    [AcceptHttpVerbs(HttpMethod.Post)]
    public ActionResult TestSgml(HtmlProcessor text)
    {
        HtmlElementFlags allowFlags = HtmlElementFlags.Allowed | HtmlElementFlags.Recursive;
        text.Parser.ProcessingMode = HtmlProcessingMode.DenyByDefault;
        text.Parser.DefaultElementFlags = HtmlElementFlags.AllowContent | HtmlElementFlags.UseTypography;
        
        text.Parser.Add(a => a.Attributes("xmlns", "ns").As(HtmlAttributeFlags.Denied));
        text.Parser.Add(e => e.Elements("i", "b", "u", "em", "strong", "pre", "acronym", "h1", "h2", "h3", "h4", "h5", "h6")
                .As(allowFlags | HtmlElementFlags.Text | HtmlElementFlags.UseTypography))
            .With(e => e.Element("script").As(HtmlElementFlags.Denied | HtmlElementFlags.Recursive))
            .With(e => e.Element("a")
                .As(allowFlags | HtmlElementFlags.Text)
                .Convert(elem => { 
                    if(elem.InnerText.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && 
                        elem.InnerText.Length > 30) { 
                        XmlAttribute title = elem.GetAttributeNode("title") ?? 
                            elem.Attributes.Append(elem.OwnerDocument.CreateAttribute("title"));
                        title.Value = elem.InnerText;
                        elem.InnerXml = elem.InnerText.Substring(0, 30) + "...";
                    }
                    return elem; 
                })
                .With(a => a.Attribute("href").Validate("#url")
                    .Convert(attr => {
                        Uri uri = new Uri(attr.Value, UriKind.RelativeOrAbsolute);
                        if (uri.IsAbsoluteUri && uri.Host != "localhost")
                            attr.Value = String.Format("http://localhost/redirect?url={0}", Uri.EscapeDataString(attr.Value));

                        return attr;
                    }))
                .With(a => a.Attribute("title"))
                .With(a => a.Attribute("rel").As(HtmlAttributeFlags.Denied | HtmlAttributeFlags.Required).Default("nofollow"))
                .With(a => a.Attribute("target").As(HtmlAttributeFlags.Denied | HtmlAttributeFlags.Required).Default("_blank"))
                )
            .With(e => e.Element("img")
                .As(allowFlags | HtmlElementFlags.SelfClosing)
                .Convert(elem => {
                    if (elem.HasAttribute("alt")) {
                        XmlAttribute title = elem.GetAttributeNode("title") ?? elem.Attributes.Append(elem.OwnerDocument.CreateAttribute("title"));
                        title.Value = elem.GetAttribute("alt");
                    }
                    return elem;
                })
                .With(a => a.Attribute("src").Validate("#url"))
                .With(a => a.Attribute("width").As(HtmlAttributeFlags.Required).Validate("#int").Default(100))
                .With(a => a.Attribute("height").Validate("#int"))
                .With(a => a.Attribute("title")))
            .With(e => e.Element("p").As(allowFlags | HtmlElementFlags.AllowContent | HtmlElementFlags.UseTypography))
            .With(e => e.Element("nobr").As(allowFlags | HtmlElementFlags.Text | HtmlElementFlags.UseTypography))
            .With(e => e.Element("ul").As(allowFlags | HtmlElementFlags.Container)
                .With(l => l.Element("li").As(HtmlElementFlags.Allowed | HtmlElementFlags.Text | HtmlElementFlags.UseTypography)))
            .With(e => e.Element("code").As(allowFlags | HtmlElementFlags.Preformatted))
            .With(e => e.Element("br").As(allowFlags | HtmlElementFlags.SelfClosing))
            .With(e => e.Element("habracut").As(allowFlags | HtmlElementFlags.SelfClosing)
                .Convert(elem => {
                    XmlElement link = elem.OwnerDocument.CreateElement("a");
                    link.Attributes.Append(elem.OwnerDocument.CreateAttribute("href"))
                        .Value = "http://localhost/blog/news/21.html";
                    string title = (elem.HasAttribute("title")) ? String.Format("{0} →",
                        elem.Attributes["title"].Value) : "читать далее →";
                    link.AppendChild(elem.OwnerDocument.CreateTextNode(title));

                    return link;
                }));

        text.Typographer.EncodeSpecialSymbols = true;
        //text.Typographer.ExtractLinks = false;
        text.Typographer.Replace("(c)", "&copy;")
            .Replace("(r)", "&reg;")
            .Replace("(tm)", "&trade;")
            .Replace("+/-", "&plusmn;")
            .Replace("+-", "&plusmn;")
            .Replace(@"(?'number'\d+)\s*\^\s*(?'power'\-?\d+((\.|\,)\d+)?)", 
                "${number}<sup>${power}</sup>", StringReplacementMode.Regex)
            .Replace(@"(?'before'\d+)\s*x\s*(?'after'\-?\d+)", 
                "${before}&times;${after}", StringReplacementMode.Regex)
            .Replace(@"(?'before'\d+)\s*\*\s*(?'after'\-?\d+)",
                "${before}&times;${after}", StringReplacementMode.Regex);

        ViewData["Output"] = text.ToString();
        
        return View("Test");
    }

    public ActionResult TestJsonCallback(int id)
    {
        return Json(new { Id = id, Name = "Ringo Starr", Position = "Drums", Data = "Some data" });
    }

    [AcceptHttpVerbs(HttpMethod.Get)]
	[Maza]
    public ActionResult TestArrayAndCollection(string maza)
    {
		string ss = @"{ Name: ""Wahha"", Index: 0 }";
		JavaScriptSerializer s = new JavaScriptSerializer();
		Section section = (Section)s.Deserialize(typeof(Section), ss);

		var str = new string[] {
			"wahha", "caitlin", "ksu", "putin"
		};
		ViewData["names"] = str;
        return View("Arrays");
    }

	public class TestClass
	{
		[SkipBinding]
		public HttpPostedFileBase File
		{
			get;
			set;
		}

		public AccessModes Access
		{
			get;
			set;
		}
	}

    [AcceptHttpVerbs(HttpMethod.Post)]
    public ActionResult TestArrayAndCollection(int[] indices, IEnumerable<string> names, 
        Dictionary<int, Tuple<bool, string>> dict, TestClass model, FormCollection form)
    {
        int errors = Errors.Count;

		ViewData["names"] = names;
        ViewData["Array"] = indices;
        ViewData["Collection"] = names;
        ViewData["Dictionary"] = dict;

        IValueSet set = form.Subset(s => s.StartsWith("dict-"))
            .Transform(s => s.Remove(0, 5));

        return View("Arrays");
    }

    public enum SectionType : int
    {
        Simple = 1,
        Extended,
        Statictics
    }

    [HttpCompression]
    [AcceptHttpVerbs(HttpMethod.Get)]
    [ApplyCulture("lc", Source = ParameterSource.QueryString | ParameterSource.Header)]
    public ActionResult Section([Bind(Source = ParameterSource.Url)]string section, 
        [Bind(Default = SectionType.Simple)]SectionType type)
    {        
        ViewData["Items"] = new object[] { 
            new { ID = 1, Title = "Wahha" }, 
            new { ID = 2, Title = "MVC" }, 
            new { ID = 3, Title = "Cool" }, 
            new { ID = 4, Title = "Website" },
            new { ID = 5, Title = "ASP.NET" }
        };

        return View("Section", new Section(section));
    }

    [ActionName("SectionItem")]
    public void VeryComplexActionMethodName(string user, string section, int item)
    {
        Response.Write(user);
    }

    [ActionName("Section")]
    [AcceptHttpVerbs(HttpMethod.Post)]
    [ValidateRequestToken(Timeout = 2)]
    public ActionResult Login()
    {
        UserCredentials creds = new UserCredentials() { Login = "sergey", Password = "***", Other = "SomeString" };
        BindModel<UserCredentials>(creds, new string[] { "login" });

        ViewData["Section"] = new Section("ya-krivetko");
        ViewData["Items"] = new object[] { 
            new { ID = 1, Title = "Wahha" }, 
            new { ID = 2, Title = "MVC" }, 
            new { ID = 3, Title = "Cool" }, 
            new { ID = 4, Title = "Website" },
            new { ID = 5, Title = "ASP.NET" }
        };
        return View("Section");
    }
}

public class UserCredentials
{
    private string _login;
    private string _password;
    private string _other;

    public string Login
    {
        get
        {
            return _login;
        }
        set
        {
            _login = value;
        }
    }

    public string Password
    {
        get
        {
            return _password;
        }
        set
        {
            _password = value;
        }
    }

    public string Other
    {
        get
        {
            return _other;
        }
        set
        {
            _other = value;
        }
    }
}

public class Section
{
    private string _name;
    private int _index;

    public int Index
    {
        get
        {
            return _index;
        }
        set
        {
            _index = value;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

	public Section()
		: this(null)
	{
	}

    public Section(string name)
    {
        _name = name;
    }
}
