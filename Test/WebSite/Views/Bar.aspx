<%@ Page Language="C#" Inherits="Radischevo.Wahha.Web.Mvc.UI.ViewPage" %>
<% Ajax.Scripts.Block("onload").Append(() => { %>document.title = "Аллес андер май контрол!";<% }); %>
<h2 style="color:#ff6600"><%= ViewData.Model %></h2>
<%= Html.Controls.Tag("a", new { HRef = "http://sergey.ro", Class = "active-link" }, "Иди туда не знаю куда") %><%= Html.Controls.Tag("br") %>
<%= Html.Controls.Tag("a", new { HRef = Url.Route<MainController>(p => p.Section("xenia", MainController.SectionType.Simple)), Class = "section-link" }, "В новости") %>