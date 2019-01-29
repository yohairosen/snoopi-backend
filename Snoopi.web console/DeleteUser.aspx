<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="DeleteUser.aspx.cs" Inherits="Snoopi.web.DeleteUser" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">

    <Localized:Label ID="lblDeleteConfirm" runat="Server" BlockMode="true" AssociatedControlID="chkDeleteConfirm" LocalizationClass="Users" LocalizationId="DeleteUserConfirmLabel"></Localized:Label>
    <Localized:CheckBox runat="server" ID="chkDeleteConfirm" LocalizationClass="Users" LocalizationId="DeleteUserConfirmCheckbox" /><br />
    <Localized:CheckboxValidator ID="rfvDeleteConfirm" ControlToValidate="chkDeleteConfirm" runat="server" Display="None" LocalizationClass="Users" LocalizationId="DeleteUserConfirmRequired"></Localized:CheckboxValidator>
    <br />
    <Localized:LinkButton runat="server" ID="btnDelete" OnClick="btnDelete_Click" LocalizationClass="Users" LocalizationId="DeleteUserButton"></Localized:LinkButton>

</asp:Content>
