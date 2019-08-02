<%@ Page Language="C#" AutoEventWireup="true" CodeFile="YoneticiSayfa.aspx.cs" Inherits="CS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body {
            font-family: Arial;
            font-size: 15pt;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
         <asp:Button ID="btnAnasayfa" Text="ANASAYFA" runat="server" PostBackUrl="~/AnaSayfa.aspx"/>
        <p></p>
        <asp:FileUpload ID="FileUpload1" runat="server"/>
        <asp:TextBox ID="tbAliciAdi" runat="server"></asp:TextBox>
        <asp:Button ID="btnAliciEkle" runat="server" Text="Alıcı Ekle" OnClick="btnAliciEkle_Click"/>  
        <br />
        <br />        
        <asp:CheckBoxList ID="CheckBoxList1" Visible="true" runat="server" DataTextField="Video"
            CellSpacing="10" RepeatLayout="OrderedList">
        </asp:CheckBoxList>
        <asp:Button ID="BtnGonder" runat="server" Text="Gönder" OnClick="BtnGonder_Click"/>
        <hr />
    </form>
</body>
</html>
