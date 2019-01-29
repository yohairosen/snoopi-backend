using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using Snoopi.web.Resources;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using Snoopi.core.BL;
using dg.Utilities;

namespace Snoopi.web
{
    public partial class Login : SupplierPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                if (SuppliersSessionHelper.IsAuthenticated())
                {
                    Response.Redirect(@"/", true);
                }
             
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            //Page.Title = LoginPageStrings.GetText(@"PageTitle");
            //Body.Attributes[@"class"] += @" " + GlobalStrings.GetText(@"direction");
            
            //chkRememberMe.Text = " " + string.Format(LoginPageStrings.GetText(@"RememberMe"), AppConfig.GetInt32(@"Authentication.AuthCookieLifeInHours", 72) / 24).ToHtml();
            btnLogin.Text = LoginPageStrings.GetHtml(@"Enter");

            rfvEml.ErrorMessage = LoginPageStrings.GetHtml(@"EmailRequired");
            revEml.ErrorMessage = LoginPageStrings.GetHtml(@"EmailRequired");
            rfvPwd.ErrorMessage = LoginPageStrings.GetHtml(@"PasswordRequired");
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            Membership.UserAuthenticateResults results = SuppliersSessionHelper.Login(txtEmail.Text.Trim(), txtPassword.Text, false);
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
                    Master.MessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"ErrorGeneral"));
                    break;
                case Membership.UserAuthenticateResults.Locked:
                    Master.MessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"ErrorLocked"));
                    break;
            }
        }
}
}