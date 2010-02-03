<%@ Page Language="C#" Inherits="Radischevo.Wahha.Web.Mvc.UI.ViewPage<TemplatedItem>" MasterPageFile="~/Views/default.master" %>
<%@ import namespace="Radischevo.Wahha.Web.Mvc.Validation" %>
<%@ Import Namespace="Radischevo.Wahha.Web.Scripting.Templates" %>
<asp:Content ContentPlaceHolderID="title" runat="server">Нереальный сайт MVC</asp:Content>
<asp:Content ContentPlaceHolderID="head" runat="server">
<% Ajax.Scripts.Include("http://ajax.microsoft.com/ajax/jquery/jquery-1.3.2.min.js")
		.Include("http://ajax.microsoft.com/ajax/jquery.validate/1.5.5/jquery.validate.min.js")
		.Include("~/resources/jquery.mvc.validation.js");
   Html.Controls.DataSource = Parameters.Form; %>
</asp:Content>
<asp:Content ContentPlaceHolderID="main" runat="server">
<script type="text/javascript" src="<%= JavaScriptTemplateHandler.GenerateUrl(Url.Resource("~/scriptTemplate.axd"), "~/views/global.js.html") %>">
</script>
<h3>test</h3>
<div id="templated"></div>
<form action="/wahha-test/register" method="post" id="default">    
    <p>login <input type="text" name="login" /></p>
    <p>e-mail <input type="text" name="email" /></p>
    <p>password <input type="text" name="password" /></p>
    <p>confirmation <input type="text" name="confirmation" /></p>
    <input type="submit" value="Save" />
    <a href="/wahha-test/photos/10" target="_blank" id="clicker">туды его</a>
</form>
<%  Ajax.Scripts.Block("validate", () => { %>
    $().ready(function() {
		for(var i = 0; i < 10; ++i) {
			$("#clicker").click();
		}
        MvcValidation.apply(<%= Validation.Rules()
            .Apply("form#default") %>);
        $("#templated").html(global({ 
			PageTitle: "Китаец!", 
			Title: "Крутая штука", 
			Rel: "external", 
			Url: "http://sergey.ro",
			ShowHiddenItems: true 
		}, 10));
    });
<% }); %>
</asp:Content>