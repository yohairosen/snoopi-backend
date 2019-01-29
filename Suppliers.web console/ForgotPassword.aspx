<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ForgotPassword.aspx.cs" Inherits="Snoopi.web.ForgotPassword" MasterPageFile="~/SuppliersTemplate.master" %>
<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<asp:Content ContentPlaceHolderID="cphContent" runat="Server">
     <div class="title">
        <Localized:Label ID="Label1"  runat="server" LocalizationClass="SupplierProfile" LocalizationId="ForgotTitle" ></Localized:Label>
    </div>
    <div class="wrapper-forgot-pass">
                    
                        
                        <asp:PlaceHolder runat="server" ID="phForgotFields" Visible="false">
                            <Localized:RequiredFieldValidator ID="rfvEml" runat="Server" ControlToValidate="txtEmail" Display="None" LocalizationClass="LoginPage" LocalizationId="EmailRequired"></Localized:RequiredFieldValidator>
				            <table class="nostyle">
					            <tr>
						            <td class="label"><asp:Label ID="lblEmail" AssociatedControlID="txtEmail" runat="Server"><%= LoginPageStrings.GetHtml(@"Email") %></asp:Label></td>
						            <td colspan="2"><asp:TextBox runat="Server" ID="txtEmail" CssClass="input-text" AutoCompleteType="Email" type="email"></asp:TextBox></td>
					            </tr>
					            <tr>
						            <td colspan="3" class="t-hopposite">
                                        <Localized:Button runat="server" ButtonStyle="ButtonStyle1" ID="btnForgotPassword" OnClick="btnForgotPassword_Click" LocalizationClass="LoginPage" LocalizationId="ResetPassword" />
                                    </td>
					            </tr>
				            </table>
                                            <Localized:RegularExpressionValidator ID="revEml" runat="Server" Display="None" ControlToValidate="txtEmail" LocalizationClass="SupplierProfile" LocalizationId="EmailWrong" ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"></Localized:RegularExpressionValidator>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="phResetFields" Visible="false">
                            <Localized:RequiredFieldValidator ID="rfvPwd" runat="Server" ControlToValidate="txtNewPassword" Display="None" LocalizationClass="LoginPage" LocalizationId="NewPasswordRequired"></Localized:RequiredFieldValidator>
                            <Localized:CompareValidator ID="rfvRptPwd" runat="Server" ControlToValidate="txtRepeatPassword" ControlToCompare="txtNewPassword" Display="None" LocalizationClass="LoginPage" LocalizationId="RepeatPasswordInvalid"></Localized:CompareValidator>
				            <table class="nostyle">
					            <tr>
						            <td><Localized:Label ID="lblNewPassword" AssociatedControlID="txtNewPassword" LocalizationClass="LoginPage" LocalizationId="NewPassword" runat="Server"></Localized:Label></td>
						            <td colspan="2"><asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" CssClass="input-text"></asp:TextBox></td>
					            </tr>
					            <tr>
						            <td><Localized:Label ID="lblRepeatPassword" AssociatedControlID="txtRepeatPassword" LocalizationClass="LoginPage" LocalizationId="RepeatPassword" runat="Server"></Localized:Label></td>
						            <td colspan="2"><asp:TextBox runat="server" ID="txtRepeatPassword" TextMode="Password" CssClass="input-text"></asp:TextBox></td>
					            </tr>
					            <tr>
						            <td colspan="3" class="t-hopposite">
                                        <Localized:Button runat="server" ButtonStyle="ButtonStyle1" ID="btnResetPassword" OnClick="btnResetPassword_Click" LocalizationClass="LoginPage" LocalizationId="ResetPassword" />
                                    </td>
					            </tr>
				            </table>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="phLogin" Visible="false">
                            <asp:HyperLink runat="server" ID="hlForgotPassword" NavigateUrl="/Login.aspx"><%= LoginPageStrings.GetHtml(@"ResetPasswordForwardToLogin")%></asp:HyperLink>
                        </asp:PlaceHolder>
</div>
    </asp:Content>
