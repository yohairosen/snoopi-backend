<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResendDeal.aspx.cs" Inherits="ResendDeal"  MasterPageFile="~/Template.master" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentBids" ContentPlaceHolderID="cphContent" runat="Server">
    <Localized:Label ID="Label4" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SupplierName" />&nbsp;
    <asp:DropDownList runat="server" ID="ddlSuppliers" style="margin-right:5px">
    </asp:DropDownList>
    <Localized:Button runat="server" ID="bt_resend"
                            ButtonStyle="ButtonStyle2"
                            OnClick="bt_resend_Click"
                            style="margin-right:22px"
                            CommandArgument='<%# Eval("BidId")%>'
                            LocalizationClass="Bid" LocalizationId="ResendDeal" />
    <br />
       <br />
       <Localized:Button runat="server" ID="bt_back_to_admin_bids"
                            ButtonStyle="ButtonStyle2"
                            style="margin-right:350px"
                            OnClick="bt_back_to_admin_bids_Click"
                            LocalizationClass="Bid" LocalizationId="BackToAdminBids" />
</asp:Content>