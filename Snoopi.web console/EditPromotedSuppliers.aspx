<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditPromotedSuppliers.aspx.cs" MasterPageFile="Template.master"  Inherits="EditPromotedSuppliers" %>


<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <asp:Panel ID="pnlEditSupplier" runat="server" DefaultButton="btnSave">
        <asp:HiddenField runat="server" ID="hfOriginalId" />
        <table>
            <tr>
               <th class="t-natural">
                                       <Localized:Label runat="server" LocalizationClass="PromotedArea" LocalizationId="AreaName"></Localized:Label>
</th>
                <td class="nowrap">
                                        <asp:DropDownList runat="server" ID="ddlAreas" DataTextField="name" DataValueField="id"></asp:DropDownList>

                </td>

            </tr>
                        <tr>
               <th class="t-natural">
                                   <Localized:Label runat="server" LocalizationClass="Suppliers" LocalizationId="ServiceType"></Localized:Label>
    
               </th>
                <td class="nowrap">
                                        <asp:DropDownList ID="ddlServices" runat="server" ></asp:DropDownList>

                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblBusinessName" AssociatedControlID="ddlPromtedSuppliers" LocalizationClass="Suppliers" LocalizationId="BusinessName"></Localized:Label></th>
                <td class="nowrap">
                                                <asp:DropDownList runat="server" ID="ddlPromtedSuppliers" onchange="ddlSuppliersChange(this)" Style="display: none" Width="320px">
                                                </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFromDate" AssociatedControlID="txtFromDate" LocalizationClass="Advertisements" LocalizationId="FromDate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtFromDate" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblToDate" AssociatedControlID="txtToDate" LocalizationClass="Advertisements" LocalizationId="ToDate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtToDate" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="3" class="t-center">
                    <Localized:LinkButton runat="server" ID="btnSave" CssClass="button" OnClick="btnSave_Click" LocalizationClass="Suppliers" LocalizationId="SaveButton"></Localized:LinkButton>
                </td>
            </tr>

        </table>
        <asp:HiddenField ID="hfBeforeRegisterSupplierName" runat="server" Visible="false" />

    </asp:Panel>
   <script type="text/javascript">
        $(function () {
            $('#<%= txtFromDate.ClientID%>').datepicker({ dateFormat: 'dd/mm/yy' }).val();
            $('#<%= txtToDate.ClientID%>').datepicker({ dateFormat: 'dd/mm/yy' }).val();
        });

    </script>
</asp:Content>
