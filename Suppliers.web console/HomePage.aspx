<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SuppliersTemplate.master" CodeFile="HomePage.aspx.cs" Inherits="HomePage" %>

<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>

<asp:Content ID="cHead" runat="server" ContentPlaceHolderID="head"></asp:Content>

<asp:Content ID="cContent" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Literal runat="server" ID="ltDescription"></asp:Literal>
    <div class="center">
        <div class="home-page-center">
        <asp:LinkButton ID="LinkButtonProfile" CssClass="link-button-main button-profile" PostBackUrl="/MyProfile.aspx" runat="server"></asp:LinkButton>
            <br />
        <asp:LinkButton ID="LinkButtonProducts" CssClass="link-button-main button-products" PostBackUrl="/ProductManegement.aspx" runat="server"></asp:LinkButton>
            <br />
        <asp:LinkButton ID="LinkButtonHistory" CssClass="link-button-main button-history" PostBackUrl="/DealsHistory.aspx" runat="server"></asp:LinkButton>
            <br />        
        <asp:LinkButton ID="LinkButtonClicks" CssClass="link-button-main button-history" PostBackUrl="/ClicksHistory.aspx" runat="server"></asp:LinkButton>
 </div>
    </div>
</asp:Content>
