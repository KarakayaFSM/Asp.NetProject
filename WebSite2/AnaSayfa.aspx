<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AnaSayfa.aspx.cs" Inherits="AnaSayfa" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="div1" >
    </div>
        <asp:ImageButton ID="imgBtnCalisan" ImageUrl="~/images/Calisan.PNG" PostBackUrl="~/CalisanSayfa.aspx" runat="server" ToolTip="Çalışan Sayfası" />    
        <asp:ImageButton ID="imgBtnYonetici" ImageUrl="~/images/Yonetici.PNG" PostBackUrl="~/YoneticiSayfa.aspx" runat="server" ToolTip="Yönetici Sayfası" />
        
    </form>
</body>
</html>
