using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Web.UI;

namespace Snoopi.web
{
    public partial class EditUserProfile : AdminPageBase
    {
        protected override string[] AllowedPermissions
        {
            get { return new string[] { }; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            pwdChange.Visible = false;
        }
        private void LoadView()
        {
            UserProfile userProfile = UserProfile.FetchByID(SessionHelper.UserId());
            if (userProfile != null)
            {
                txtFirstName.Text = userProfile.FirstName;
                txtLastName.Text = userProfile.LastName;
                txtPhone.Text = userProfile.Phone;
                txtEmail.Text = SessionHelper.UserEmail();
                txtEmail.Enabled = false;
                txtCurrentPassword.Text = @"";
                txtPassword.Text = @"";
                txtConfirmPassword.Text = @"";
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = UserProfileStrings.GetText(@"UserProfilePageTitle");
            Master.ActiveMenu = "MyProfile";
            LoadView();
        }

        protected void btnChangePwd_Click(object sender, EventArgs e)
        {
            pwdChange.Visible = true;
            txtCurrentPassword.Visible =true;
            txtPassword.Visible = true;
            txtConfirmPassword.Visible = true;
            btnChangePwd.Enabled = false;

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                UserProfile userProfile = UserProfile.FetchByID(SessionHelper.UserId());
                if (userProfile == null)
                {
                    userProfile = new UserProfile();
                    userProfile.UserId = SessionHelper.UserId();
                }

                userProfile.FirstName = txtFirstName.Text.Trim();
                userProfile.LastName = txtLastName.Text.Trim();
                userProfile.Phone = txtPhone.Text.Trim();

                userProfile.Save();
                Master.MessageCenter.DisplaySuccessMessage(UserProfileStrings.GetText(@"MessageSaved"));

                if (txtCurrentPassword.Text.Length > 0)
                {
                    if (txtPassword.Text.Length > 0)
                    {
                        Membership.UserPasswordChangeResults results;
                        results = Membership.ChangeUserPassword(SessionHelper.UserEmail(), txtCurrentPassword.Text, txtPassword.Text);
                        switch (results)
                        {
                            default:
                                Master.MessageCenter.DisplayWarningMessage(UserProfileStrings.GetText(@"MessagePasswordChangeFailedUnknown"));
                                break;
                            case Membership.UserPasswordChangeResults.PasswordDoNotMatch:
                                Master.MessageCenter.DisplayWarningMessage(UserProfileStrings.GetText(@"MessagePasswordChangeBadOldPassword"));
                                break;
                            case Membership.UserPasswordChangeResults.Success:
                                Master.MessageCenter.DisplaySuccessMessage(UserProfileStrings.GetText(@"MessagePasswordChanged"));
                                break;
                        }
                    }
                    if (txtEmail.Text.Length > 0 && txtEmail.Text != SessionHelper.UserEmail())
                    {
                        if (SessionHelper.UserEmail().NormalizeEmail() != txtEmail.Text.NormalizeEmail())
                        {
                            try
                            {
                                Snoopi.core.DAL.User user = Snoopi.core.DAL.User.FetchByID(SessionHelper.UserId());
                                user.Email = txtEmail.Text.Trim();
                                user.UniqueEmail = user.Email.NormalizeEmail();
                                user.Save();
                                SessionHelper.SetUserEmail(user.Email);
                                Master.MessageCenter.DisplayWarningMessage(UserProfileStrings.GetText(@"MessageEmailChanged"));
                            }
                            catch
                            {
                                Master.MessageCenter.DisplaySuccessMessage(UserProfileStrings.GetText(@"MessageEmailChangeFailed"));
                            }
                        }
                    }
                }
            }
        }
    }
}
