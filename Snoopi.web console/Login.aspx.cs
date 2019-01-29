using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.core;
using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.web.Localization;
using System.Globalization;

namespace Snoopi.web
{
    public partial class LoginPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionHelper.IsAuthenticated())
            {
                Response.Redirect(@"/", true);
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.Title = LoginPageStrings.GetText(@"PageTitle", new CultureInfo("he-IL"));
            Body.Attributes[@"class"] += @" " + GlobalStrings.GetText(@"direction", new CultureInfo("he-IL"));

            chkRememberMe.Text = string.Format(LoginPageStrings.GetText(@"RememberMe", new CultureInfo("he-IL")), AppConfig.GetInt32(@"Authentication.AuthCookieLifeInHours", 72) / 24).ToHtml();
            btnLogin.Text = LoginPageStrings.GetHtml(@"Submit", new CultureInfo("he-IL"));

            rfvEml.ErrorMessage = LoginPageStrings.GetHtml(@"EmailRequired",new CultureInfo("he-IL"));
            revEml.ErrorMessage = LoginPageStrings.GetHtml(@"EmailRequired",new CultureInfo("he-IL"));
            rfvPwd.ErrorMessage = LoginPageStrings.GetHtml(@"PasswordRequired", new CultureInfo("he-IL"));
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            Membership.UserAuthenticateResults results = SessionHelper.Login(txtEmail.Text.Trim(), txtPassword.Text, chkRememberMe.Checked);
            switch (results)
            {
                case Membership.UserAuthenticateResults.Success:
                    {
                        string ReturnUrl = Request.QueryString[@"ReturnUrl"] ?? "";
                        ReturnUrl = ReturnUrl.Trim();
                        if (ReturnUrl.Length == 0 || !Http.IsLocalUrl(ReturnUrl)) ReturnUrl = @"/";
                        Response.Redirect(ReturnUrl, true);
                    }
                    break;
                default:
                case Membership.UserAuthenticateResults.NoMatch:
                case Membership.UserAuthenticateResults.LoginError:
                    mcMessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"ErrorGeneral",new CultureInfo("he-IL")));
                    break;
                case Membership.UserAuthenticateResults.Locked:
                    mcMessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"ErrorLocked",new CultureInfo("he-IL")));
                    break;
            }
        }
    }
}