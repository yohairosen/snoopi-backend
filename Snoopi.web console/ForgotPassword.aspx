<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ForgotPassword.aspx.cs" Inherits="Snoopi.web.ForgotPassword" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.core" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" id="Head">
	<meta http-equiv="content-type" content="text/html; charset=utf-8" />
	<meta name="robots" content="noindex,nofollow" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />
    <meta name="viewport" content="width=device-width,initial-scale=0.85" />
	<link rel="stylesheet" media="screen,projection" type="text/css" href="/resources/css/main.css" /> <!-- MAIN STYLE SHEET -->
	<title></title>
</head>

<body runat="server" id="Body">

    <div class="login-page">

        <div class="vhcentered">
    
            <div class="form">
        
                <div class="frame">
            
                    <form id="frmForgotPassword" runat="server">

				        <div class="logo"><a href="http://www.Snoopi.co.il" target="_blank"><img src="/resources/images/logo.png" alt="Snoopi" /></a></div>
                    
                        <Custom:MessageCenter runat="Server" id="mcMessageCenter"></Custom:MessageCenter>
                        
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

                    </form>

                </div>
            
            </div>

        </div>

    </div>
    
</body>

</html>
