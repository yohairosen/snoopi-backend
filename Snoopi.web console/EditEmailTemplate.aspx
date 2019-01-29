<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="EditEmailTemplate.aspx.cs" Inherits="Snoopi.web.EditEmailTemplate" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">

    <asp:Panel ID="pnlEdit" runat="Server" DefaultButton="btnSave">
    
        <Localized:Label ID="lblName" runat="Server" BlockMode="true" AssociatedControlID="txtName" LocalizationClass="EmailTemplates" LocalizationId="TemplateNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" runat="server" ID="txtName"></asp:TextBox><br /><br />
        <Localized:RequiredFieldValidator ID="rfvName" ControlToValidate="txtName" runat="server" Display="None" LocalizationClass="EmailTemplates" LocalizationId="TemplateNameRequired"></Localized:RequiredFieldValidator>

        <Localized:Label ID="lblSubject" runat="Server" BlockMode="true" AssociatedControlID="txtSubject" LocalizationClass="EmailTemplates" LocalizationId="SubjectLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" runat="server" ID="txtSubject"></asp:TextBox><br /><br />

        <Localized:Label ID="lblTo" runat="Server" BlockMode="true" AssociatedControlID="txtTo" LocalizationClass="EmailTemplates" LocalizationId="RecipientLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtTo"></asp:TextBox><br /><br />

        <Localized:Label ID="lblCc" runat="Server" BlockMode="true" AssociatedControlID="txtCc" LocalizationClass="EmailTemplates" LocalizationId="CcLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtCc"></asp:TextBox><br /><br />

        <Localized:Label ID="lblBcc" runat="Server" BlockMode="true" AssociatedControlID="txtBcc" LocalizationClass="EmailTemplates" LocalizationId="BccLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtBcc"></asp:TextBox><br /><br />

        <Localized:Label ID="lblFromEmail" runat="Server" BlockMode="true" AssociatedControlID="txtFromEmail" LocalizationClass="EmailTemplates" LocalizationId="SenderEmailLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtFromEmail"></asp:TextBox><br /><br />

        <Localized:Label ID="lblFromName" runat="Server" BlockMode="true" AssociatedControlID="txtFromName" LocalizationClass="EmailTemplates" LocalizationId="SenderNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" runat="server" ID="txtFromName"></asp:TextBox><br /><br />

        <Localized:Label ID="lblReplyToEmail" runat="Server" BlockMode="true" AssociatedControlID="txtReplyToEmail" LocalizationClass="EmailTemplates" LocalizationId="ReplyToEmailLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtReplyToEmail"></asp:TextBox><br /><br />

        <Localized:Label ID="lblReplyToName" runat="Server" BlockMode="true" AssociatedControlID="txtReplyToName" LocalizationClass="EmailTemplates" LocalizationId="ReplyToNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" runat="server" ID="txtReplyToName"></asp:TextBox><br /><br />

        <Localized:Label ID="lblMailPriority" runat="Server" BlockMode="true" AssociatedControlID="ddlMailPriority" LocalizationClass="EmailTemplates" LocalizationId="PriorityLabel"></Localized:Label>
        <asp:DropDownList runat="Server" id="ddlMailPriority"></asp:DropDownList><br /><br />

        <Localized:Label ID="lblBody" runat="Server" BlockMode="true" AssociatedControlID="ckBody" LocalizationClass="EmailTemplates" LocalizationId="BodyLabel"></Localized:Label>
        <CKEditor:CKEditorControl ID="ckBody" runat="server" /><br />

        <Localized:LinkButton runat="server" ID="btnSave" OnClick="btnSave_Click" LocalizationClass="EmailTemplates" LocalizationId="SaveButton"></Localized:LinkButton>

    </asp:Panel>
    
</asp:Content>
