<%@ page language="C#" inherits="Radischevo.Wahha.Web.Mvc.UI.ViewPage<System.Collections.Generic.IEnumerable<MessageInfo>>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Архив SMS-сообщений за 2008 &mdash; 2009</title>
</head>
<body>
    <% foreach(MessageInfo message in Model) { %>
    <% Html.Choice(message.Direction == MessageDirection.Incoming, () => { %>
    <h1><%= message.Date.ToString("HH:mm, dd MMMM yyyy")%></h1>
    <h3><%= message.Message %></h3>
    <% }, () => { %>
    <h2><%= message.Date.ToString("HH:mm, dd MMMM yyyy")%></h2>
    <h4><%= message.Message %></h4>
    <% }); %>
    <% } %>
</body>
</html>