<%@ Application Language="C#" %>
<%@ Import Namespace="Radischevo.Wahha.Core" %>
<%@ Import Namespace="Radischevo.Wahha.Web" %>
<%@ Import Namespace="Radischevo.Wahha.Web.Routing" %>
<%@ Import Namespace="Radischevo.Wahha.Web.Mvc" %>
<%@ Import Namespace="Radischevo.Wahha.Web.Mvc.Async" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        RouteTable.Routes.MapRoute("news_item", "~/news/{section}/{item}.html",
            new { Controller = "MainController", Action = "SectionItem" },
            new RegexConstraint("section", "[A-Za-z]+[A-Za-z0-9\\-_]*"),
            new RegexConstraint("item", "\\d+"));

        RouteTable.Routes.MapRoute("template_item", "~/template",
            new { Controller = "MainController", Action = "TemplatedItemTest" });

        RouteTable.Routes.MapRoute("register", "~/register",
            new { Controller = "OtherController", Action = "Register" });

        RouteTable.Routes.MapRoute("file_test", "~/test-file",
            new { Controller = "OtherController", Action = "CheckHttpPostedFile" });

        RouteTable.Routes.MapRoute("ajax-title", "~/ajax/check-title",
           new { Controller = "OtherController", Action = "CheckItemTitle" });
        
        RouteTable.Routes.MapRoute("sms_item", "~/sms/page{page}",
           new { Controller = "OtherController", Action = "Messages", Page = 1 });

		RouteTable.Routes.MapRoute("posts_last", "~/blog/new/{count}",
		   new {
			   Controller = "OtherController", Action = "Posts", Count = 12
		   });

		RouteTable.Routes.MapRoute("data_item", "~/data",
		   new {
			   Controller = "MainController", Action = "Database"
		   });

		RouteTable.Routes.MapAsyncRoute("sms_async", "~/sms-async/page{page}",
			new {
				Controller = "MegaController", Action = "Messages", Page = 1
			});
		
        RouteTable.Routes.MapRoute("array_test", "~/array",
            new { Controller = "MainController", Action = "TestArrayAndCollection" });
        
        RouteTable.Routes.MapRoute("news_section", "/TestSite/news/{section}",
            new { Controller = "MainController", Action = "Section", Page = 1 },
            new HttpMethodConstraint(HttpMethod.Get | HttpMethod.Head),
            new RegexConstraint("section", "[A-Za-z]+[A-Za-z0-9\\-_]*"));
        
        // login
        Route login = new Route("~/login", new MvcRouteHandler());
        login.Constraints.Add(new HttpMethodConstraint(HttpMethod.Post));
        login.Defaults.Add("controller", "MainController");
        login.Defaults.Add("action", "Section");

        // index
        Route index = new Route("~/default.aspx", new MvcRouteHandler());
        index.Defaults.Add("controller", "MainController");
        index.Defaults.Add("action", "Default");

        RouteTable.Routes.Add("login", login);
        RouteTable.Routes.Add("index", index);
    }
    void Application_End(object sender, EventArgs e) 
    { }  
    void Application_Error(object sender, EventArgs e) 
    { }
    void Session_Start(object sender, EventArgs e) 
    { }
    void Session_End(object sender, EventArgs e) 
    { }  
</script>
