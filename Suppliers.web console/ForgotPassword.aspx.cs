using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.core;
using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;

namespace Snoopi.web
{
    public partial class ForgotPassword : SupplierPageBase
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
                //frmForgotPassword.DefaultButton = @"btnForgotPassword";
            }
            else
            {
                phForgotFields.Visible = false;
                phResetFields.Visible = true;
                phLogin.Visible = false;
                rfvEml.Enabled = false;
                rfvPwd.Enabled = true;
                rfvRptPwd.Enabled = true;
                //frmForgotPassword.DefaultButton = @"btnResetPassword";

                if (!Email.IsValidEmail())
                {
                    Master.MessageCenter.DisplayWarningMessage(LoginPageStrings.GetText(@"InvalidEmail"));
                    phResetFields.Visible = false;
                }
            }
                //frmForgotPassword.DefaultButton = @"btnForgotPassword";            
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            //if (RecoveryKey.Length == 0 || Email.Length == 0)
            //{
            //    Page.Title = LoginPageStrings.GetText(@"ForgotPasswordPageTitle");
            //}
            //else
            //{
            //    Page.Title = LoginPageStrings.GetText(@"ResetPasswordPageTitle");
            //}
            //Body.Attributes[@"class"] += @" " + GlobalStrings.GetText(@"direction");
            //Page.Title = LoginPageStrings.GetText(@"ForgotPasswordPageTitle");         
           
            //Body.Attributes[@"class"] += @" " + GlobalStrings.GetText(@"direction");
        }
        protected void btnForgotPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string key = Membership.GenerateRecoveryKeySupplier(txtEmail.Text);
            if (!string.IsNullOrEmpty(key))
            {
                var user = core.DAL.AppSupplier.FetchByEmail(txtEmail.Text);
                if (user == null)
                {
                    Master.MessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"ForgotPasswordFailed"));
                    return;
                }
                EmailMessagingService.SendPasswordRecoveryMailForSupplier(user, key, "he-IL");
                Master.MessageCenter.DisplaySuccessMessage(LoginPageStrings.GetText(@"ForgotPasswordSent"));
                phForgotFields.Visible = false;
            }
            else
            {
                Master.MessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"ForgotPasswordFailed"));
                //phForgotFields.Visible = false;
            }
        }
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            Membership.UserRecoveryResults results = Membership.SupplierVerifyRecoveryKey(Email, RecoveryKey, txtNewPassword.Text);
            switch (results)
            {
                case Membership.UserRecoveryResults.Success:
                    Master.MessageCenter.DisplaySuccessMessage(LoginPageStrings.GetText(@"ResetPasswordSuccess"));
                    phResetFields.Visible = false;
                    phLogin.Visible = true;
                    break;
                default:
                case Membership.UserRecoveryResults.Expired:
                case Membership.UserRecoveryResults.KeyDoNotMatch:
                    Master.MessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"InvalidRecoveryKey"));
                    //phResetFields.Visible = false;
                    break;
                case Membership.UserRecoveryResults.UserDoesNotExist:
                    Master.MessageCenter.DisplayErrorMessage(LoginPageStrings.GetText(@"EmailDoesNotExist"));
                    //phResetFields.Visible = false;
                    break;
            }
        }
    }
}