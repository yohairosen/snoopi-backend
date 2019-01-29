<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Template.master" CodeFile="EditAdCompany.aspx.cs" Inherits="EditAdCompany" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <asp:Panel ID="pnlEditSupplier" runat="server" DefaultButton="btnSave">
        <asp:HiddenField runat="server" ID="hfOriginalCompanyId" />
        <table>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblBusinessName" AssociatedControlID="txtBusinessName" LocalizationClass="Suppliers" LocalizationId="BusinessName"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtBusinessName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblEmail" AssociatedControlID="txtEmail" LocalizationClass="Suppliers" LocalizationId="Email"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtEmail" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvEmail" runat="server"
                        ControlToValidate="txtEmail" LocalizationClass="Suppliers" LocalizationId="SupplierEmailRequired" Display="None"></Localized:RequiredFieldValidator>
                    <Localized:RegularExpressionValidator ID="revEml" runat="Server"
                        LocalizationClass="Suppliers" LocalizationId="SupplierEmailInvalid" Display="None" ControlToValidate="txtEmail"
                        ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$">*</Localized:RegularExpressionValidator>
                </td>
            </tr>

            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblPhone" AssociatedControlID="txtPhone" LocalizationClass="Suppliers" LocalizationId="Phone"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPhone" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblContactName" AssociatedControlID="txtContactName" LocalizationClass="Suppliers" LocalizationId="ContactName"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtContactName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblContactPhone" AssociatedControlID="txtContactPhone" LocalizationClass="Suppliers" LocalizationId="ContactPhone"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtContactPhone" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblDescription" AssociatedControlID="txtDescription" LocalizationClass="Suppliers" LocalizationId="Description"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtDescription" runat="server"></asp:TextBox>
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

</asp:Content>
