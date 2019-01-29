using System;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core;
using dg.Utilities;
using Snoopi.core.DAL;
using System.Collections.Generic;
using dg.Sql;
using dg.Sql.Connector;
using System.Web.UI;

namespace Snoopi.web
{
    public partial class EditUser : AdminPageBase
    {
        Int64 UserId;

        bool IsNewMode = false;

        protected override void VerifyAccessToThisPage()
        {
            //string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            //if (!permissions.Contains(Permissions.PermissionKeys.sys_edit_users))
            //{
            //    Master.LimitAccessToPage();
            //}
            IsNewMode = Request.QueryString[@"New"] != null;

            if (!IsNewMode)
            {
                if (Int64.TryParse(Request.QueryString[@"UserId"], out UserId))
                {
                    if (!Membership.UserCanAffectUser(SessionHelper.UserId(), UserId))
                    {
                        Master.LimitAccessToPage();
                    }
                }
                else
                {
                    string email = Request.QueryString[@"Email"];
                    if (email != null && email.Length > 0)
                    {
                        User user = core.DAL.User.FetchByEmail(email);
                        if (user == null)
                        {
                            Master.LimitAccessToPage();
                        }
                        else
                        {
                            UserId = user.UserId;
                            if (!Membership.UserCanAffectUser(SessionHelper.UserId(), UserId))
                            {
                                Master.LimitAccessToPage();
                            }
                        }
                    }
                    else
                    {
                        UserId = SessionHelper.UserId();
                    }
                }
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(UsersStrings.GetText(@"NewUserButton"), @"EditUser.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                hfOriginalUserId.Value = UserId.ToString();

                LoadView();
            }
            else
            {
                if (hfOriginalUserId.Value.Length > 0 && hfOriginalUserId.Value != UserId.ToString())
                {
                    Http.Respond404(true);
                }
            }
        }
        protected void LoadView()
        {
            if (UserId > 0)
            {
                User user = core.DAL.User.FetchByID(UserId);

                txtEmail.Text = user.Email;
                chkIsLocked.Checked = user.IsLocked;
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = UsersStrings.GetText(IsNewMode ? @"NewUserPageTitle" : @"EditUserPageTitle");
            Master.ActiveMenu = IsNewMode ? "NewUser" : "Users";

            trCurrentPassword.Visible = !IsNewMode && UserId == SessionHelper.UserId();
            rfvPasswordRequired.Visible = rfvPasswordRequired.Enabled = IsNewMode;
            rfvConfirmPasswordRequired.Visible = rfvConfirmPasswordRequired.Enabled = IsNewMode;
        }

        public string PermissionDescription(object permission)
        {
            return UsersStrings.GetText(@"Perm_" + permission.ToString());
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string UserEmail = null;
            bool EmailChanged = false;

            if (IsNewMode)
            {
                User user = null;
                Membership.UserCreateResults results = Membership.CreateUser(txtEmail.Text, txtPassword.Text.Trim(), out user);
                switch (results)
                {
                    default:
                    case Membership.UserCreateResults.UnknownError:
                        Master.MessageCenter.DisplayErrorMessage(UsersStrings.GetText(@"MessageCreateFailedUnknown"));
                        return;
                    case Membership.UserCreateResults.AlreadyExists:
                        Master.MessageCenter.DisplayErrorMessage(UsersStrings.GetText(@"MessageCreateFailedAlreadyExists"));
                        return;
                    case Membership.UserCreateResults.InvalidEmailAddress:
                        Master.MessageCenter.DisplayErrorMessage(UsersStrings.GetText(@"MessageCreateFailedEmailAddressInvalid"));
                        return;
                    case Membership.UserCreateResults.Success:
                        break;
                }
                if (chkIsLocked.Checked != user.IsLocked)
                {
                    user.IsLocked = chkIsLocked.Checked;
                    user.Save();
                }
                UserId = user.UserId;
                UserEmail = user.Email;
            }
            else
            {
                User user = core.DAL.User.FetchByID(UserId);
                UserEmail = user.UniqueEmail;

                if (user.UniqueEmail != txtEmail.Text.NormalizeEmail() ||
                    user.IsLocked != chkIsLocked.Checked)
                {
                    try
                    {
                        user.Email = txtEmail.Text.Trim();
                        user.UniqueEmail = user.Email.NormalizeEmail();
                        user.IsLocked = chkIsLocked.Checked;
                        user.Save();
                        if (user.UniqueEmail != UserEmail)
                        {
                            UserEmail = user.Email;
                            EmailChanged = true;
                        }
                    }
                    catch
                    {
                        Master.MessageCenter.DisplayWarningMessage(UsersStrings.GetText(@"MessageEmailChangeFailed"));
                    }
                }

                if (txtPassword.Text.Length > 0)
                {
                    Membership.UserPasswordChangeResults results;
                    if (UserId == SessionHelper.UserId())
                    {
                        results = Membership.ChangeUserPassword(user.Email, txtCurrentPassword.Text, txtPassword.Text);
                    }
                    else
                    {
                        results = Membership.ChangeUserPassword(user.Email, txtPassword.Text);
                    }
                    switch (results)
                    {
                        default:
                            Master.MessageCenter.DisplayWarningMessage(UsersStrings.GetText(@"MessagePasswordChangeFailedUnknown"));
                            break;
                        case Membership.UserPasswordChangeResults.PasswordDoNotMatch:
                            Master.MessageCenter.DisplayWarningMessage(UsersStrings.GetText(@"MessagePasswordChangeBadOldPassword"));
                            break;
                        case Membership.UserPasswordChangeResults.Success:
                            break;
                    }
                }
                UserEmail = user.Email;
            }

            if (IsNewMode)
            {
                string successMessage = UsersStrings.GetText(@"MessageUserCreated");
                string url = @"EditUser.aspx?Email=" + UserEmail;
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);
            }
            else
            {
                string successMessage = UsersStrings.GetText(@"MessageUserSaved");
                if (EmailChanged)
                {
                    string url = @"EditUser.aspx?message-success=" + Server.UrlEncode(successMessage);
                    if (UserId != SessionHelper.UserId())
                    {
                        url += @"&Email=" + UserEmail;
                    }
                    Response.Redirect(url, true);
                }
                else
                {
                    Master.MessageCenter.DisplaySuccessMessage(successMessage);
                }
            }
        }
    }
}
