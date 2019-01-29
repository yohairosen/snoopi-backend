<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="DeleteAppUser.aspx.cs" Inherits="Snoopi.web.DeleteAppUser" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">

    <Localized:Label ID="lblDeleteConfirm" runat="Server" BlockMode="true" AssociatedControlID="chkDeleteConfirm" LocalizationClass="AppUsers" LocalizationId="DeleteAppUserConfirmLabel"></Localized:Label>
    <Localized:CheckBox runat="server" ID="chkDeleteConfirm" LocalizationClass="AppUsers" LocalizationId="DeleteAppUserConfirmCheckbox" /><br />
    <Localized:CheckboxValidator ID="rfvDeleteConfirm" ControlToValidate="chkDeleteConfirm" runat="server" Display="None" LocalizationClass="AppUsers" LocalizationId="DeleteAppUserConfirmRequired"></Localized:CheckboxValidator>
    <br />
    <Localized:LinkButton runat="server" ID="btnDelete" OnClick="btnDelete_Click" LocalizationClass="AppUsers" LocalizationId="DeleteAppUserButton"></Localized:LinkButton>

</asp:Content>
