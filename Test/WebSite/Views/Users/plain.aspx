<%@ Page Language="C#" %>
<script runat="server">
    protected override void OnLoad(EventArgs e)
    {
        lbShow.Text = DateTime.Now.ToString("HH:mm");
        base.OnLoad(e);
    }

    protected void OnButtonClick(object sender, EventArgs e)
    {
        ((LinkButton)sender).Text = "Clicked";
    }
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>This is the plain old Web Form</title>
</head>
<body>
    <h1>This is a plain old Web Form.</h1>    
    <form runat="server">
        <asp:linkbutton id="lbShow" runat="server" onclick="OnButtonClick" />
    </form>
</body>
</html>
