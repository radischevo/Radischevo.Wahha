<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sample Page</title>
</head>
<body>
    <h1>OTHER PAGE</h1>
    <table>
    <% foreach (Person p in ViewData.GetValue<IEnumerable<Person>>("Bunch"))
       { %>
       <tr>
            <td><%= p.ID %></td>
            <td><%= p.FirstName %></td>
            <td><%= p.MiddleName %></td>
            <td><%= p.LastName %></td>
            <td><%= p.Address %></td>
       </tr> 
    <% } %>
    </table>
</body>
</html>
