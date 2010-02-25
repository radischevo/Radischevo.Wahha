<%@ Page Language="C#" ValidateRequest="false" Inherits="Radischevo.Wahha.Web.Mvc.UI.ViewPage" %>
<%@ Import Namespace="Radischevo.Wahha.Web.Mvc.Html" %>
<%@ Import Namespace="System.Collections.Generic" %>
<% Html.Controls.DataSource = Parameters.Form; %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Array Binder Test</title>
</head>
<body>
    <form action="/wahha-test/array" method="post" enctype="multipart/form-data">
        <p>
            <input type="text" name="indices" value="0" /> 1<br />
            <% Validation.Message("indices-0", error => { %><%= error.Message %><br /><% }); %>
            <input type="text" name="indices" value="1" /> 2<br />
            <% Validation.Message("indices-1", error => { %><%= error.Message %><br /><% }); %>
            <input type="text" name="indices" value="2" /> 3<br />
            <% Validation.Message("indices-2", error => { %><%= error.Message %><br /><% }); %>
            <input type="text" name="indices" value="3" /> 4<br />
            <% Validation.Message("indices-3", error => { %><%= error.Message %><br /><% }); %>
            <input type="text" name="indices" value="4" /> 5<br />
            <% Validation.Message("indices-4", error => { %><%= error.Message %><% }); %>
        </p>
        <p>
            <input type="checkbox" name="names-1" value="Катя" /> Катя<br />
            <input type="checkbox" name="names-2" value="Петя" /> Петя<br />
            <input type="checkbox" name="names-3" value="Вася" /> Вася<br />
            <input type="checkbox" name="names-4" value="Маша" /> Маша<br />
            <input type="checkbox" name="names-5" value="Ксюша" /> Ксюша<br />
        </p>
        <p>
            1 <input type="checkbox" name="dict-1-item1" />
              <input type="text" name="dict-1-item2" value="Имя" /><br />
            2 <input type="checkbox" name="dict-2-item1" />
              <input type="text" name="dict-2-item2" value="" /><br />
            3 <input type="checkbox" name="dict-3-item1" />
              <input type="text" name="dict-3-item2" value="" /><br />
        </p>
        <p>
			<input type="file" name="model-file" /><br />
			<input type="checkbox" name="model-access" value="Read" /> Read<br />
			<input type="checkbox" name="model-access" value="Edit" /> Edit<br /> 
			<input type="checkbox" name="model-access" value="Delete" /> Delete<br />
			<input type="checkbox" name="model-access" value="ChangePermissions" /> ChangePermissions<br />
        </p>
        
        <input type="submit" value="Поехали" />
    </form>
    
    <% if(ViewContext.Context.Request.HttpMethod == HttpMethod.Post) { %>
    <div style="border:1px solid #555">
        <b>Номера:</b>
        <% foreach (int value in ViewData.GetValue<int[]>("Array")) { %>
            <%= value %>, 
        <% } %>
        <b>Имена: </b>
        <% foreach (string value in ViewData.GetValue<ICollection<string>>("Collection")) { %>
            <%= value %>, 
        <% } %>
        <b>Словарь: </b>
        <% foreach (KeyValuePair<int, Tuple<bool, string>> value in 
               ViewData.GetValue<Dictionary<int, Tuple<bool, string>>>("Dictionary")) { %>
            <%= value.Key %> = <%= value.Value.Item1 %>, <%= value.Value.Item2 %>; 
        <% } %>
    </div>
    <% } %>
</body>
</html>
