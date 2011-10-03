<%@ Page Language="C#" MasterPageFile="~/Views/default.master" %>
<asp:Content ContentPlaceHolderID="title" runat="server">Нереальный сайт MVC</asp:Content>
<asp:Content ContentPlaceHolderID="head" runat="server">
<% Ajax.Scripts.Include("~/resources/scripts/jquery.js");%>
   <link rel="stylesheet" media="all" href="/wahha-test/resources/forms.css" />
   <script type="text/javascript" src="/wahha-test/routing.js"></script>
   <script type="text/javascript">
   	alert(RouteTable.route("news-item", { "section": "caitlin-ryan", "item": 10 }));
   	alert(RouteTable.route("template-item"));
   	alert(RouteTable.route("news-section", { "section": "caitlin-ryan" }));
   </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="main" runat="server">
    <div>
        &#8594;<a href="<%= Url.Route<MainController>(p => p.Section("caitlin-ryan", MainController.SectionType.Simple)) %>">Новости, однако</a>.
    </div>
	<div class="button-box">
		<input type="submit" value="Сохранить" class="huge-button" />
		<input type="button" value="Отменить" />
		<input type="button" value="Вернуться" disabled="disabled" />
	</div>
    <% Html.Component<SmallComponent>(a => a.WriteMessage("Мегакомпонент")); %>
    <% Html.Component<MainController>(a => a.SampleComponent(new Section("Maza"))); %>
</asp:Content>