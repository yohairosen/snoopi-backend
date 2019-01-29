<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SuppliersTemplate.master" CodeFile="Login.aspx.cs" Inherits="Snoopi.web.Login" %>

<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<asp:Content ID="ContentAppUsers" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <div class="center">
            <div class="login-center">
                <Localized:Label runat="server" CssClass="head-label" LocalizationClass="LoginPage" LocalizationId="LoginRequest"></Localized:Label>
               
                <Localized:Label ID="lblEmail" CssClass="txt-label" runat="server" AssociatedControlID="txtEmail" LocalizationClass="LoginPage" LocalizationId="UserName"></Localized:Label>
                <br />
                <asp:TextBox runat="Server" ID="txtEmail" CssClass="input-text" AutoCompleteType="Email" type="email"></asp:TextBox>
                <br />
                <Localized:Label ID="lblPassword" CssClass="txt-label" runat="server" AssociatedControlID="txtPassword" LocalizationClass="LoginPage" LocalizationId="Password1"></Localized:Label>
                <br />
                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="input-text"></asp:TextBox>
                <br />
                <asp:Button runat="server" ID="btnLogin" OnClick="btnLogin_Click" Text="enter" CssClass="button button-entry" />
<%--                <div>
                    <asp:CheckBox runat="server" CssClass="checkbox-wrapper" ID="chkRememberMe" />
                </div>--%>
                <div>
                    <asp:HyperLink runat="server" ID="hlForgotPassword" NavigateUrl="/ForgotPassword.aspx"><%= LoginPageStrings.GetHtml(@"ForgotPassword") %></asp:HyperLink>
                </div>
            </div>
            <asp:RequiredFieldValidator ID="rfvEml" runat="Server" ControlToValidate="txtEmail" Display="None"></asp:RequiredFieldValidator>
                           <Localized:RegularExpressionValidator ID="revEml" runat="Server" Display="None" ControlToValidate="txtEmail" LocalizationClass="SupplierProfile" LocalizationId="EmailWrong" 
                               ValidationExpression="^[A-Za-z0-9\.-_%+\-]+@[A-Za-z0-9\.-]+\.([A-Za-z0-9\-]+)*$"></Localized:RegularExpressionValidator>

            <asp:RequiredFieldValidator ID="rfvPwd" runat="Server" ControlToValidate="txtPassword" Display="None"></asp:RequiredFieldValidator>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false"></asp:PlaceHolder>

</asp:Content>

