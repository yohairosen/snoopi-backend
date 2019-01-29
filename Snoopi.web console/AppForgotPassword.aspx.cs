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
    public partial class AppForgotPassword : System.Web.UI.Page
    {
        string RecoveryKey;
        string Email;
        protected void Page_Init(object sender, EventArgs e)
        {
            RecoveryKey = Request.QueryString[@"key"] ?? @"";
            Email = (Request.QueryString[@"email"] ?? @"").Trim();
            if (RecoveryKey.Length == 0 || Email.Length == 0)
            {
                phForgotFields.Visible = true;
                phResetFields.Visible = false;
                rfvEml.Enabled = true;
                rfvPwd.Enabled = false;
                rfvRptPwd.Enabled = false;
                frmForgotPassword.DefaultButton = @"btnForgotPassword";
            }
            else
            {
                phForgotFields.Visible = false;
                phResetFields.Visible = true;
                rfvEml.Enabled = false;
                rfvPwd.Enabled = true;
                rfvRptPwd.Enabled = true;
                frmForgotPassword.DefaultButton = @"btnResetPassword";

                if (!Email.IsValidEmail())
                {
                    mcMessageCenter.DisplayWarningMessage(AppForgotPasswordStrings.GetText(@"InvalidEmail"));
                    phResetFields.Visible = false;
                }
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (RecoveryKey.Length == 0 || Email.Length == 0)
            {
                Page.Title = AppForgotPasswordStrings.GetText(@"ForgotPasswordPageTitle");
            }
            else
            {
                Page.Title = AppForgotPasswordStrings.GetText(@"ResetPasswordPageTitle");
            }
            Body.Attributes[@"class"] += @" " + GlobalStrings.GetText(@"direction");
        }
        protected void btnForgotPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string key = AppMembership.GenerateRecoveryKey(txtEmail.Text);
            if (!string.IsNullOrEmpty(key))
            {
                EmailMessagingService.SendPasswordRecoveryMailForAppUser(core.DAL.AppUser.FetchByEmail(txtEmail.Text), key, null);
                mcMessageCenter.DisplaySuccessMessage(AppForgotPasswordStrings.GetText(@"ForgotPasswordSent"));
                phForgotFields.Visible = false;
            }
            else
            {
                mcMessageCenter.DisplayErrorMessage(AppForgotPasswordStrings.GetText(@"ForgotPasswordFailed"));
                phForgotFields.Visible = false;
            }
        }
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            AppMembership.AppUserRecoveryResults results = AppMembership.VerifyRecoveryKey(Email, RecoveryKey, txtNewPassword.Text);
            switch (results)
            {
                case AppMembership.AppUserRecoveryResults.Success:
                    mcMessageCenter.DisplaySuccessMessage(AppForgotPasswordStrings.GetText(@"ResetPasswordSuccess"));
                    phResetFields.Visible = false;
                    break;
                default:
                case AppMembership.AppUserRecoveryResults.Expired:
                case AppMembership.AppUserRecoveryResults.KeyDoNotMatch:
                    mcMessageCenter.DisplayErrorMessage(AppForgotPasswordStrings.GetText(@"InvalidRecoveryKey"));
                    phResetFields.Visible = false;
                    break;
                case AppMembership.AppUserRecoveryResults.AppUserDoesNotExist:
                    mcMessageCenter.DisplayErrorMessage(AppForgotPasswordStrings.GetText(@"EmailDoesNotExist"));
                    phResetFields.Visible = false;
                    break;
            }
        }
    }
}