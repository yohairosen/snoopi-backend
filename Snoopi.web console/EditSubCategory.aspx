<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditSubCategory.aspx.cs" Inherits="Snoopi.web.EditSubCategory" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel ID="pnlEditSubCategory" runat="server" DefaultButton="btnSave">
        <table>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblCategoryName" AssociatedControlID="txtCategoryName" LocalizationClass="Categories" LocalizationId="lblCategoryName"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtCategoryName" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvCategoryName" runat="server"
                        ControlToValidate="txtCategoryName" Display="None" LocalizationClass="Categories" LocalizationId="CategoryNameRequired">*</Localized:RequiredFieldValidator>
                </td>
            </tr>
             <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFilters" AssociatedControlID="ddlFilters" LocalizationClass="Categories" LocalizationId="lblFilters"></Localized:Label></th>
                <td class="nowrap">
                    <asp:ListBox ID="ddlFilters" runat="server" SelectionMode="Multiple" ClientIDMode="Static" AutoPostBack="true"></asp:ListBox>
                </td>
            </tr>
<%--            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblImage" AssociatedControlID="fuImage" LocalizationClass="Categories" LocalizationId="CategoryImage"></Localized:Label></th>
                <td class="nowrap">
                    <asp:FileUpload ID="fuImage" runat="server" />
                    <asp:Image ID="imgImage" runat="server" CssClass="image-small" />
                    <Localized:Button ID="btnDeleteImage" runat="server" CssClass="button-02" LocalizationClass="Products" LocalizationId="DeleteImage" OnClick="btnDeleteImage_Click" />
                </td>
            </tr>--%>
             <tr runat="server" id="tr_main_category">
                <th class="t-natural">
                    <Localized:Label ID="lblMainCategory" runat="Server" AssociatedControlID="ddlCategory" LocalizationClass="Categories" LocalizationId="lblMainCategory"></Localized:Label></th>
                <td class="nowrap">
                    <asp:DropDownList CssClass="input-text" ID="ddlCategory" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="3" class="t-center">
                    <Localized:LinkButton runat="server" ID="btnSave" OnClick="btnSave_Click" CssClass="button" LocalizationClass="Categories" LocalizationId="SaveSubCategory"></Localized:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>


</asp:Content>

