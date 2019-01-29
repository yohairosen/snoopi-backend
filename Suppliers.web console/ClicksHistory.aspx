<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SuppliersTemplate.master" CodeFile="ClicksHistory.aspx.cs" Inherits="ClicksHistory" %>

<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>
<%@ Import Namespace="Snoopi.core.BL" %>
<%@ Import Namespace="Snoopi.core.DAL" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>

<asp:Content ID="cHead" runat="server" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content ID="cContent" runat="server" ContentPlaceHolderID="cphContent">
    <div class="title">
        <Localized:Label runat="server" LocalizationClass="SupplierEvent" LocalizationId="SupplierEventTitle"></Localized:Label>
    </div>
    <div class="sub-title">
        <Localized:Label ID="Label4" runat="server" LocalizationClass="SupplierProfile" LocalizationId="OrderDescription"></Localized:Label>
    </div>

    <div class="wrapper-order-search">

        <Localized:Label ID="Label6" runat="server" LocalizationClass="SupplierProfile" LocalizationId="SortLabel" CssClass="label frst"></Localized:Label>

        <Localized:Label ID="Label2" runat="server" LocalizationClass="SupplierProfile" LocalizationId="YearLabel" CssClass="label"></Localized:Label>
        <asp:DropDownList runat="server" ID="ddlyear" AutoPostBack="true" EnableViewState="true">
        </asp:DropDownList>

        <Localized:Label ID="Label1" runat="server" LocalizationClass="SupplierProfile" LocalizationId="MonthLabel" CssClass="label"></Localized:Label>
        <asp:DropDownList runat="server" ID="ddlMonth" AutoPostBack="true" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged" EnableViewState="true">
        </asp:DropDownList>

        <Localized:Label ID="Label3" runat="server" LocalizationClass="SupplierProfile" LocalizationId="DayFromLabel" CssClass="label"></Localized:Label>
        <asp:DropDownList runat="server" ID="ddlDayFrom" AutoPostBack="true" Enabled="false" EnableViewState="true" CssClass="ddl-day">
        </asp:DropDownList>

        <Localized:Label ID="Label5" runat="server" LocalizationClass="SupplierProfile" LocalizationId="DayToLabel" CssClass="label"></Localized:Label>
        <asp:DropDownList runat="server" ID="ddlDayTo" AutoPostBack="true" Enabled="false" EnableViewState="true" CssClass="ddl-day">
        </asp:DropDownList>

    </div>

    <div class="dvClicksHistory">
        <div class="dvClicksHistoryCenter">
            <table>
                <tr>
                    <td class="tdClickHistory">
                        <Localized:Label ID="lblClickNum" runat="server" LocalizationClass="SupplierEvent" LocalizationId="ClickNum" style="font-size:30px;float:left" CssClass="label"></Localized:Label>
                    </td>
                    <td class="tdClickHistory">
                        <Localized:TextBox ID="txtClickNum" runat="server" Enabled="false" CssClass="txtClickNum"/></td>
                    <td class="tdClickHistory">
                        <Localized:Label ID="lblClicksToCallNum" runat="server" LocalizationClass="SupplierEvent" LocalizationId="ClickToCallNum" style="font-size:30px;float:left" CssClass="label"></Localized:Label>
                    </td>
                    <td class="tdClickHistory">
                        <Localized:TextBox ID="txtClicksToCallNum" runat="server" Enabled="false" CssClass="txtClickNum" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
