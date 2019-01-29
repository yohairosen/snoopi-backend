using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.core;
using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.web.Localization;

namespace Snoopi.web
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        string RecoveryKey;
        string Email;
        protected void Page_Init(object sender, EventArgs e)
        {
            RecoveryKey = (Request.QueryString[@"key"] ?? @"").Trim().Replace(" ", "+");
            Email = (Request.QueryString[@"email"] ?? @"").Trim();
            if (RecoveryKey.Length == 0 || Email.Length == 0)
            {
                phForgotFields.Visible = true;
                phResetFields.Visible = false;
                phLogin.Visible = false;
                rfvEml.Enabled = true;
                rfvPwd.Enabled = false;
                rfvRptPwd.Enabled = false;
                frmForgotPassword.DefaultButton = @"btnForgotPassword";
            }
            else
            {
                phForgotFields.Visible = false;
                phResetFields.Visible = true;
                phLogin.Visible = false;
                rfvEml.Enabled = false;
                rfvPwd.Enabled = true;
                rfvRptPwd.Enabled = true;
                frmForgotPassword.DefaultButton = @"btnResetPassword";

                if (!Email.IsValidEmail())
                {
                    mcMessageCenter.DisplayWarningMessage(LoginPageStrings.GetText(@"InvalidEmail"));
                    phResetFields.Visible = false;
                }
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (RecoveryKey.Length == 0 || Email.Length == 0)
            {
                Page.Title = LoginPageStrings.GetText(@"ForgotPasswordPageTitle");
            }
            else
            {
                Page.Title = LoginPageStrings.GetText(@"ResetPasswordPageTitle");
            }
            Body.Attributes[@"class"] += @" " + GlobalStrings.GetText(@"direction");
        }
        protected void btnForgotPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string key = Membership.GenerateRecoveryKey(txtEmail.Text);
            if (!string.IsNullOrEmpty(key))
            {
                EmailMessagingService.SendPasswordRecoveryMailForUser(core.DAL.User.FetchByEmail(txtEmail.Text), key, null);
                mcMessageCenter.DisplaySuccessMessage(LoginPageStrings.GetText(@"ForgotPasswordSent"));
                phForgotFields.Visible = false;
            }
            else
            {
                mcMessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"ForgotPasswordFailed"));
                //phForgotFields.Visible = false;
            }
        }
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            Membership.UserRecoveryResults results = Membership.VerifyRecoveryKey(Email, RecoveryKey, txtNewPassword.Text);
            switch (results)
            {
                case Membership.UserRecoveryResults.Success:
                    mcMessageCenter.DisplaySuccessMessage(LoginPageStrings.GetText(@"ResetPasswordSuccess"));
                    phResetFields.Visible = false;
                    phLogin.Visible = true;
                    break;
                default:
                case Membership.UserRecoveryResults.Expired:
                case Membership.UserRecoveryResults.KeyDoNotMatch:
                    mcMessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"InvalidRecoveryKey"));
                    //phResetFields.Visible = false;
                    break;
                case Membership.UserRecoveryResults.UserDoesNotExist:
                    mcMessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"EmailDoesNotExist"));
                    //phResetFields.Visible = false;
                    break;
            }
        }
    }
}