<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="EditUserProfile.aspx.cs" Inherits="Snoopi.web.EditUserProfile" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">

   
    <asp:Panel ID="pnlEdit" runat="Server" DefaultButton="btnSave">

        <Localized:Label ID="lblFirstName" runat="Server" BlockMode="true" AssociatedControlID="txtFirstName" LocalizationClass="UserProfile" LocalizationId="FirstNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" runat="server" ID="txtFirstName"></asp:TextBox><br /><br />

        <Localized:Label ID="lblLastName" runat="Server" BlockMode="true" AssociatedControlID="txtLastName" LocalizationClass="UserProfile" LocalizationId="LastNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" runat="server" ID="txtLastName"></asp:TextBox><br /><br />

        <Localized:Label ID="lblPhone" runat="Server" BlockMode="true" AssociatedControlID="txtPhone" LocalizationClass="UserProfile" LocalizationId="PhoneLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text ltr" runat="server" ID="txtPhone"></asp:TextBox><br /><br />

        <Localized:Label ID="lblEmail" runat="Server" BlockMode="true" AssociatedControlID="txtEmail" LocalizationClass="UserProfile" LocalizationId="EmailLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" ID="txtEmail" runat="server"></asp:TextBox><br /><br />
        <Localized:RequiredFieldValidator ID="rfvEmail" runat="server" 
            ControlToValidate="txtEmail" LocalizationClass="UserProfile" LocalizationId="UserEmailRequired" Display="None" 
            ></Localized:RequiredFieldValidator><Localized:RegularExpressionValidator ID="revEml" runat="Server" 
            LocalizationClass="UserProfile" LocalizationId="UserEmailInvalid" Display="None" ControlToValidate="txtEmail" 
            ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$">*</Localized:RegularExpressionValidator>
        <br />
        

     <asp:Panel ID="pwdChange" runat="server" DefaultButton="btnSave">
            
        <Localized:Label ID="lblCurrentPassword" runat="Server" BlockMode="true" AssociatedControlID="txtEmail" LocalizationClass="UserProfile" LocalizationId="CurrentPasswordLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" ID="txtCurrentPassword" runat="server" TextMode="Password"></asp:TextBox>

        <Localized:Label ID="lblPassword" runat="Server" BlockMode="true" AssociatedControlID="txtPassword" LocalizationClass="UserProfile" LocalizationId="PasswordLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" runat="server" ID="txtPassword"></asp:TextBox><br /><br />
        <Localized:RequiredFieldValidator ID="rfvPassword" ControlToValidate="txtPassword" runat="server" Display="None" LocalizationClass="UserProfile" LocalizationId="PasswordRequired"></Localized:RequiredFieldValidator>
        
        <Localized:Label ID="lblConfirmPassword" runat="Server" BlockMode="true" AssociatedControlID="txtConfirmPassword" LocalizationClass="UserProfile" LocalizationId="ConfirmPasswordLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text" runat="server" ID="txtConfirmPassword"></asp:TextBox><br /><br />
        <Localized:RequiredFieldValidator ID="rfvConfirmPasswordRequired" runat="server" 
            ControlToValidate="txtConfirmPassword" Display="None" LocalizationClass="UserProfile" LocalizationId="UserNewPasswordConfirmRequired"
            ></Localized:RequiredFieldValidator><Localized:CompareValidator ID="cvPasswordCompare" runat="server" 
            ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword" 
            Display="None" LocalizationClass="UserProfile" LocalizationId="UserNewPasswordConfirmInvalid"
            ></Localized:CompareValidator>  
        </asp:Panel>
        <Localized:LinkButton runat="server" ID="btnChangePwd" OnClick="btnChangePwd_Click" LocalizationClass="UserProfile" LocalizationId="ChangePassword"></Localized:LinkButton>
        <Localized:LinkButton runat="server" ID="btnSave" OnClick="btnSave_Click" LocalizationClass="UserProfile" LocalizationId="SaveButton"></Localized:LinkButton>

    </asp:Panel>
    
</asp:Content>
