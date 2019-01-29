<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="SuppliersTemplate.master" CodeFile="404.aspx.cs" Inherits="error_404" %>
<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
<meta name="robots" content="noindex,nofollow" />
<meta http-equiv="cache-control" content="no-cache" />
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<meta http-equiv="pragma" content="no-cache" />
<meta name="googlebot" content="noarchive" />
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="wrapper-404"><%= Snoopi.web.Localization.GlobalStrings.GetText("Error404Line1") %></div>
</asp:Content>
