<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Snoopi.web.LoginPage" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
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
            
                    <form id="frmLogin" runat="server" defaultbutton="btnLogin">

				        <div class="logo"><img src="/resources/images/logo.png" alt="Snoopi" /></div>
                    
                        <Custom:MessageCenter runat="Server" id="mcMessageCenter"></Custom:MessageCenter>
                    
                        <asp:RequiredFieldValidator ID="rfvEml" runat="Server" ControlToValidate="txtEmail" Display="None"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revEml" runat="Server" Display="None" ControlToValidate="txtEmail" ValidationExpression="^[A-Za-z0-9\._%+\-]+@[A-Za-z0-9\-]+(\.[A-Za-z0-9\-]+)*$"></asp:RegularExpressionValidator>
                        <asp:RequiredFieldValidator ID="rfvPwd" runat="Server" ControlToValidate="txtPassword" Display="None"></asp:RequiredFieldValidator>

				        <table class="nostyle">
					        <tr>
						        <td class="label"><asp:Label ID="lblEmail" AssociatedControlID="txtEmail" runat="Server"><%= LoginPageStrings.GetHtml(@"Email",new System.Globalization.CultureInfo("he-IL")) %></asp:Label></td>
						        <td colspan="2"><asp:TextBox runat="Server" ID="txtEmail" CssClass="input-text" AutoCompleteType="Email" type="email"></asp:TextBox></td>
					        </tr>
					        <tr>
						        <td><asp:Label ID="lblPassword" AssociatedControlID="txtPassword" runat="Server"><%= LoginPageStrings.GetHtml(@"Password",new System.Globalization.CultureInfo("he-IL")) %></asp:Label></td>
						        <td colspan="2"><asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="input-text"></asp:TextBox></td>
					        </tr>
					        <tr>
						        <td></td>
						        <td class="nowrap">
							        <asp:CheckBox runat="server" CssClass="checkbox-wrapper" ID="chkRememberMe" />
						        </td>
						        <td class="t-hopposite nowrap">
                                    <asp:HyperLink runat="server" ID="hlForgotPassword" NavigateUrl="/ForgotPassword.aspx"><%= LoginPageStrings.GetHtml(@"ForgotPassword",new System.Globalization.CultureInfo("he-IL")) %></asp:HyperLink>
                                </td>
					        </tr>
					        <tr>
						        <td colspan="3" class="t-hopposite">
                                    <asp:Button runat="server" CssClass="button" ID="btnLogin" OnClick="btnLogin_Click" />
                                </td>
					        </tr>
				        </table>

                    </form>

                </div>
               
                <div class="form-footer">
                   <!-- <div class="no-account"><%= LoginPageStrings.GetHtml(@"NoAccount",new System.Globalization.CultureInfo("he-IL")) %> <a href="mailto:support@Snoopi.co.il"><%= LoginPageStrings.GetHtml(@"ContactUs",new System.Globalization.CultureInfo("he-IL")) %></a></div>-->
              
                </div>
            
            </div>

        </div>

    </div>
    
</body>

</html>
