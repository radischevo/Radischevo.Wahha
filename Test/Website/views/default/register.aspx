<%@ Page Language="C#" Inherits="ViewPage<RegistrationForm>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Register</title>
</head>
<body>
	<% Validation.Summary ((e) => { %><span>"<%= e.Key %>" => <%= e.Message %></span><% }); %>
	<form action="<%= Url.Route("default", new { controller = "Default", action = "register" })%>" method="post">
		<p><label>Login: <input name="login" value="<%= Model.Login %>" /></label></p>
		<% Validation.Messages ("login", (e) => { %><span><%= e.Message %></span><% }); %>
		
		<p><label>Password: <input name="password" value="<%= Model.Password %>" /></label></p>
		<% Validation.Messages ("password", (e) => { %><span><%= e.Message %></span><% }); %>
		
		<p><input type="submit" value="Register"/> <a href="#" class="submit">Ajax Register</a> </p>
	</form>
</body>
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
<script type="text/javascript">
$(function() {
	$("a.submit").click(function() {
		var form = $(this).closest("form");
		$.ajax({
			url: form.attr("action"),
			type: form.attr("method"),
			dataType: "json",
			contentType: "application/json",
			data: '{"login":"mike","password":"123456","data":{"test":1,"scope":"global","array":[1,2,3,4],"obj":{"start":true,"end":100}}}',
			success: function(data) {
				alert(data);
			}
		});
		return false;
	});		
});		
</script>		
</html>	

