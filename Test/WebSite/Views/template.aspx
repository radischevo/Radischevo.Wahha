<%@ Page Language="C#" Inherits="ViewPage<TemplatedItem>" MasterPageFile="~/Views/default.master" %>
<asp:Content ContentPlaceHolderID="title" runat="server">Нереальный сайт MVC</asp:Content>
<asp:Content ContentPlaceHolderID="head" runat="server">
<% Ajax.Scripts.Include("http://ajax.microsoft.com/ajax/jquery/jquery-1.3.2.min.js")
        .Include("http://ajax.microsoft.com/ajax/jquery.validate/1.5.5/jquery.validate.min.js")
        .Include("~/resources/jquery.mvc.validation.js");
   Html.Controls.DataSource = HttpParameters.Form; %>
</asp:Content>
<asp:Content ContentPlaceHolderID="main" runat="server">
<form action="/wahha-test/template" method="post" id="default">    
    <%= Html.Serialize("state", Model) %>
    <input type="submit" value="Save" />
</form>
</asp:Content>