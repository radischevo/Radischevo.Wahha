<%@ Page Language="C#" ValidateRequest="false" Inherits="Radischevo.Wahha.Web.Mvc.UI.ViewPage" %>
<%@ Import Namespace="Radischevo.Wahha.Web.Mvc.Html" %>
<% Html.Controls.DataSource = Parameters.Form; %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SGML Parser Test</title>
</head>
<body>
    <form action="/wahha-test/sgml" method="post">
        <%= Html.Controls.TextArea("text", new { Rows = 7, Cols = 40 }) %><br />
        <input type="submit" value="парс" />
    </form>
    <div style="border:1px solid #555"><%= ViewData.GetValue<string>("Output") %></div>
</body>
</html>
