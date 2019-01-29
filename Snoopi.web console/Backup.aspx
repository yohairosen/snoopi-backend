<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Backup.aspx.cs" Inherits="Snoopi.web.Backup" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHead">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" Runat="Server">

    <Localized:Panel ID="pnlBackup" runat="Server" LocalizationId="BackupPanelTitle" LocalizationClass="DatabaseBackup" DefaultButton="btnBackup">
    
        <Localized:CheckBox runat="Server" CssClass="checkbox-wrapper" ID="chkBackupToGzip" Checked="true" LocalizationId="BackupToGzip" LocalizationClass="DatabaseBackup" /><br /><br />
        <asp:LinkButton runat="server" ID="btnBackup" CssClass="button" OnClick="btnBackup_Click"><%= DatabaseBackupStrings.GetHtml(@"DownloadBackupButton")%></asp:LinkButton>

    </Localized:Panel>
    
    <Localized:Panel ID="pnlRestore" runat="Server" LocalizationId="RestorePanelTitle" LocalizationClass="DatabaseBackup" DefaultButton="btnRestore">

        <asp:Label ID="lblRestoreFile" runat="Server" CssClass="label-line" AssociatedControlID="fuRestoreFile"><%= DatabaseBackupStrings.GetHtml(@"BackupFileLabel")%> <i class="sup"><%= DatabaseBackupStrings.GetHtml(@"BackupFileType")%></i></asp:Label>
        <asp:FileUpload runat="server" id="fuRestoreFile" /><br /><br />
        
        <Localized:RequiredFieldValidator runat="server" ID="rfvRestoreFile" ControlToValidate="fuRestoreFile" Display="None" ValidationGroup="Restore" LocalizationId="BackupFileRequiredErrorMessage" LocalizationClass="DatabaseBackup"></Localized:RequiredFieldValidator>
        <Localized:RegularExpressionValidator runat="server" ID="revRestoreFile" ControlToValidate="fuRestoreFile" Display="None" ValidationGroup="Restore" ValidationExpression=".*((\.[sS][qQ][lL])|(\.[gG][zZ]))$" LocalizationId="BackupFileTypeErrorMessage" LocalizationClass="DatabaseBackup"></Localized:RegularExpressionValidator>
        <Localized:CheckboxValidator runat="server" ID="cvAcceptRestore1" ControlToValidate="chkAcceptRestore1" Display="None" ValidationGroup="Restore" LocalizationId="AcceptRestore1ErrorMessage" LocalizationClass="DatabaseBackup"></Localized:CheckboxValidator>
        <Localized:CheckboxValidator runat="server" ID="cvAcceptRestore2" ControlToValidate="chkAcceptRestore2" Display="None" ValidationGroup="Restore" LocalizationId="AcceptRestore2ErrorMessage" LocalizationClass="DatabaseBackup"></Localized:CheckboxValidator>

        <Localized:CheckBox runat="Server" CssClass="checkbox-wrapper" ID="chkAcceptRestore1" Checked="false" LocalizationId="AcceptRestoreCheckbox1" LocalizationClass="DatabaseBackup" /><br /><br />
        <Localized:CheckBox runat="Server" CssClass="checkbox-wrapper" ID="chkAcceptRestore2" Checked="false" LocalizationId="AcceptRestoreCheckbox2" LocalizationClass="DatabaseBackup" /><br /><br />

        <asp:LinkButton runat="server" ID="btnRestore" CssClass="button" OnClick="btnRestore_Click" ValidationGroup="Restore"><%= DatabaseBackupStrings.GetHtml(@"RestoreButton")%></asp:LinkButton>
      
    </Localized:Panel>
    

</asp:Content>

