<%@ Page Title="" Language="C#" MasterPageFile="~/ServerSide/ServerSide.Master" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="DrinkOrderSystem.ClientSide.Feedback" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label1" runat="server" Text="暱稱:"></asp:Label>
    <asp:TextBox ID="txtboxName" runat="server"></asp:TextBox><br />
    <asp:Label ID="Label2" runat="server" Text="E-mail:"></asp:Label>
    <asp:TextBox ID="txtboxMail" runat="server"></asp:TextBox><br />
    <asp:Label ID="Label3" runat="server" Text="主題:"></asp:Label>
    <asp:TextBox ID="txtboxMain" runat="server"></asp:TextBox><br />
    <asp:Label ID="lblFeedtext" runat="server" Text="使用意見回饋:"></asp:Label><br />
    <asp:TextBox ID="TextBox1" runat="server" Height="253px" MaxLength="500" Width="492px" TextMode="MultiLine" ToolTip="字數上限500字"></asp:TextBox><br />
    <asp:Button ID="btndelete" runat="server" Text="重新填寫" OnClick="btndelete_Click"/>
    <asp:Button ID="btnsend" runat="server" Text="送出意見" OnClick="btnsend_Click"/>
</asp:Content>
