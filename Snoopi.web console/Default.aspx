<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Template.master" CodeFile="Default.aspx.cs" Inherits="Snoopi.web.DefaultPage" %>
<%@ MasterType VirtualPath="~/Template.master" %>

<asp:Content ID="cHead" runat="server" ContentPlaceHolderID="cphHead"></asp:Content>

<asp:Content ID="cContent" runat="server" ContentPlaceHolderID="cphContent">
<asp:Literal runat="server" id="ltDescription"></asp:Literal>
</asp:Content>
