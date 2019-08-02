<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CalisanSayfa.aspx.cs" Inherits="CalisanSayfaaspx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="btnAnasayfa" Text="ANASAYFA" runat="server" PostBackUrl="~/AnaSayfa.aspx"/>
        <p></p>
        <asp:Label Text="Kullanıcı Adı" runat="server"></asp:Label>
        
        <p>
        <asp:TextBox ID="tbusername" runat="server"></asp:TextBox>
        </p>

        <asp:Button ID="btnSorgula" Text="Sorgula" OnClick="btnSorgula_Click" runat="server"/>    
        <p>
        
        <asp:DataList ID="DlVideoContainer" Visible="false" runat="server" RepeatColumns="2" CellSpacing="3">
            <ItemTemplate>
                <u>
                    <%# Eval("videoname") %>
                </u>
                <a class="videoPlayer" style="height:300px; width: 300px; display: block" href='<%# Eval("username", "Handler.ashx?username={0}") %>'>
                </a>
            </ItemTemplate>
        </asp:DataList>
            <script src="~/scripts/flowplayer-3.2.12.min.js"></script>
            <script>
                flowplayer("a.player", "FlowPlayer/flowplayer-3.2.16.swf", {
                    plugins: {
                        pseudo: { url: "FlowPlayer/flowplayer.pseudostreaming-3.2.12.swf" }
                    },
                    clip: { provider: 'pseudo', autoPlay: false },
                });
            </script>
    </form>
    
</body>
</html>
