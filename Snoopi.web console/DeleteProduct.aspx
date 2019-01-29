<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="DeleteProduct.aspx.cs" Inherits="Snoopi.web.DeleteProduct" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">
    <asp:Panel runat="server" ID="pnlDelete">
    <Localized:Label ID="lblDeleteConfirm" runat="Server" BlockMode="true" AssociatedControlID="chkDeleteConfirm" LocalizationClass="Products" LocalizationId="DeleteProductConfirmLabel"></Localized:Label>
    <Localized:CheckBox runat="server" ID="chkDeleteConfirm" LocalizationClass="Products" LocalizationId="DeleteProductConfirmCheckbox" /><br />
    <Localized:CheckboxValidator ID="rfvDeleteConfirm" ControlToValidate="chkDeleteConfirm" runat="server" Display="None" LocalizationClass="Products" LocalizationId="DeleteProductConfirmRequired"></Localized:CheckboxValidator>
    <br />
    <Localized:LinkButton runat="server" ID="btnDelete" OnClick="btnDelete_Click" LocalizationClass="Products" LocalizationId="DeleteProductButton"></Localized:LinkButton>
    </asp:Panel>
</asp:Content>
