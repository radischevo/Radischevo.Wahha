<%@ Page Language="C#" Inherits="Radischevo.Wahha.Web.Mvc.UI.ViewPage<TemplatedItem>" MasterPageFile="~/Views/default.master" %>
<%@ import namespace="Radischevo.Wahha.Web.Mvc.Validation" %>
<asp:Content ContentPlaceHolderID="title" runat="server">Нереальный сайт MVC</asp:Content>
<asp:Content ContentPlaceHolderID="head" runat="server">
<% Ajax.Scripts.Include("http://ajax.microsoft.com/ajax/jquery/jquery-1.3.2.min.js")
        .Include("http://ajax.microsoft.com/ajax/jquery.validate/1.5.5/jquery.validate.min.js")
        .Include("~/resources/jquery.mvc.validation.js");
   Html.Controls.DataSource = Parameters.Form; %>
</asp:Content>
<asp:Content ContentPlaceHolderID="main" runat="server">
<form action="/wahha-test/template" method="post" id="default">    
    <%= Html.Templates.Editor(model => model, null, "item") %>
    <%= Html.Serialize("state", Model) %>
    <input type="submit" value="Save" />
    <% Validation.Messages("item-title", errors => { %><ul class="error"><% foreach(var error in errors) { %><li><%= error.Message %></li><% } %></ul><% }); %>
</form>
<%  Ajax.Scripts.Block("validate", () => { %>
    $().ready(function() {
		MvcValidation.apply(<%= Validation.Rules()
            .Apply("form#default") %>);
        MvcValidation.apply(<%= Validation.Rules(model => model.Title)
            .Apply("form#default") %>);
        MvcValidation.apply(<%= Validation.Rules("item", model => model.Inner)
            .Apply("form#default") %>);
    });
<% }); %>
</asp:Content>