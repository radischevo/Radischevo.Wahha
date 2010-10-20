<%@ Page Language="C#" Inherits="Radischevo.Wahha.Web.Mvc.UI.ViewPage" MasterPageFile="~/Views/default.master" %>
<asp:Content ContentPlaceHolderID="title" runat="server">Нереальный сайт MVC</asp:Content>
<asp:Content ContentPlaceHolderID="head" runat="server">
<% Ajax.Scripts.Include("~/resources/scripts/jquery.js");
   Ajax.Scripts.Block("onload").Append(() => { %> alert("maza-faza"); <% }); %>
</asp:Content>
<asp:Content ContentPlaceHolderID="main" runat="server">
    <div>
        &#8594;<a href="<%= Url.Route<MainController>(p => p.Section("caitlin-ryan", MainController.SectionType.Simple)) %>">Новости, однако</a>.
    </div>

    <%= Html.Templates.Display("this") %>
    <% Html.Component<MegaController>(a => a.MessagesAsync(10)); %>
    <% Html.Component<MegaController>(a => a.SimpleAction()); %>
    <% Html.Component<SmallComponent>(a => a.WriteMessage("Мегакомпонент")); %>
    <% Html.Component<MainController>(a => a.SampleComponent(new Section("Maza"))); %>
</asp:Content>