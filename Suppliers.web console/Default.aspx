<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SuppliersTemplate.master" CodeFile="Default.aspx.cs" Inherits="Snoopi.web.DefaultPage" %>
<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>

<asp:Content ID="cHead" runat="server" ContentPlaceHolderID="head"></asp:Content>

<asp:Content ID="cContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder">
<asp:Literal runat="server" id="ltDescription"></asp:Literal>
</asp:Content>
