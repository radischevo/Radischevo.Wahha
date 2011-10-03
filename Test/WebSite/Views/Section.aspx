<%@ Page Language="C#" Debug="true" EnableViewState="false" ValidateRequest="false" %>
<%@ Import Namespace="Radischevo.Wahha.Web.Mvc.Html" %>
<% Html.Controls.DataSource = HttpParameters.Form; %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Секция &laquo;<%= ViewData.Model.Evaluate("Name") %>&raquo;</title> 
</head>
<body>
    <a href="/TestSite/default.aspx">Назадь</a>
    <h1><%= DateTime.Now.ToLongDateString() %></h1>
    <div><h4><%= Url.HostName(1) %>, <%= HttpParameters.Form.GetValue<bool>("permanent") %></h4>
        <h2><%= DataBinder.Eval(ViewData["Section"], "Name") %></h2>
        <a href="<%= Url.Route<MainController>(p => p.VeryComplexActionMethodName("sergey", ((Section)ViewData["Section"]).Name, 18)) %>">Новость №18</a>
    </div>
    <div>
        <h2><%= Url.MakeAbsolute("~/alena", 1) %></h2>
        <% using (Html.Controls.Form(FormMethod.Post, Url.Route("login"), new { EncType = "multipart/form-data" })) { %>
        <input type="text" name="token" value="<%= Html.Controls.Token() %>" />
        <label>Имя <%= Html.Controls.TextBox("login", "логин", new { Size = 20 })%>
        </label>
        <label>Пароль <%= Html.Controls.Password("password", "пароль", new { Size = 30 })%></label>
        
        <h4>Тест списков</h4>
        <%= Html.Controls.DropDownList("section", (IEnumerable)ViewData["Items"], "Title", "ID", null)%>
        <%= Html.Controls.ListBox("selector", new string[] { "китаец", "кореец", "ITC", "MVC", "Wahha", "Aquilon", "KAFLAN" }, new { Size = 5 })%>
        
        <input type="submit" value="Войти" />
        <% } %>
    </div>
</body>
</html>
