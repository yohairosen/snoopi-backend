<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="DeleteFilter.aspx.cs" Inherits="Snoopi.web.DeleteFilter" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">
    <asp:Panel runat="server" ID="pnlDelete">
    <Localized:Label ID="lblDeleteConfirm" runat="Server" BlockMode="true" AssociatedControlID="chkDeleteConfirm" LocalizationClass="Filters" LocalizationId="DeleteFilterConfirmLabel"></Localized:Label>
    <Localized:CheckBox runat="server" ID="chkDeleteConfirm" LocalizationClass="Filters" LocalizationId="DeleteFilterConfirmCheckbox" /><br />
    <Localized:CheckboxValidator ID="rfvDeleteConfirm" ControlToValidate="chkDeleteConfirm" runat="server" Display="None" LocalizationClass="Filters" LocalizationId="DeleteFilterConfirmRequired"></Localized:CheckboxValidator>
    <br />
    <Localized:LinkButton runat="server" ID="btnDelete" OnClick="btnDelete_Click" LocalizationClass="Filters" LocalizationId="DeleteFilterButton"></Localized:LinkButton>
    </asp:Panel>
</asp:Content>
