<%@ Control Language="C#" %>

<script runat="server">

    private string _message;
    public string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }

</script>
<h1>Тут я есть сказать великая вещь! Контрол сцуко рендрится на ходу!</h1>
<h2>И он говорит нам: <%= Message %></h2>