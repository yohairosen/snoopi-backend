using System;
using System.Collections;
using System.Configuration;
using Snoopi.core.DAL;
using System.Web.UI.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using dg.Utilities;
using Snoopi.web.WebControls;
using System.Web.UI;
using Snoopi.core;
using System.IO;
using System.Collections.Generic;
using dg.Sql;

namespace Snoopi.web
{
    public partial class DeleteAppUser : AdminPageBase
    {
        bool HasSystemPermission = false;
        Int64 AppUserId;
        object AppUserName = null;
        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);
            
            if (!Int64.TryParse(Request.QueryString[@"AppUserId"], out AppUserId))
            {
                string email = Request.QueryString[@"Email"];
                if (email != null && email.Length > 0)
                {
                    AppUser app_user = core.DAL.AppUser.FetchByEmail(email);
                    if (app_user == null)
                    {
                        Master.LimitAccessToPage();
                    }
                    else
                    {
                        AppUserId = app_user.AppUserId;
                     }
                }
                else
                {
                    Master.LimitAccessToPage();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AppUserName = new Query(core.DAL.AppUser.TableSchema).Select(core.DAL.AppUser.Columns.Email).Where(core.DAL.AppUser.Columns.AppUserId, AppUserId).LimitRows(1).ExecuteScalar();
            if (AppUserName == null)
            {
                Http.Respond404(true);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = string.Format(AppUsersStrings.GetText(@"DeleteAppUserPageTitle"), AppUserName);
            Master.ActiveMenu = "AppUsers";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //Delete AppUser Token
                try
                {
                      new Query(AppUserAuthToken.TableSchema).Delete().Where(AppUserAuthToken.Columns.AppUserId, AppUserId).Execute();
                      new Query(AppUserAPNSToken.TableSchema).Delete().Where(AppUserAPNSToken.Columns.AppUserId, AppUserId).Execute();
                      new Query(AppUserGcmToken.TableSchema).Delete().Where(AppUserGcmToken.Columns.AppUserId, AppUserId).Execute();
                      Query.New<AppUser>().Where(AppUser.Columns.AppUserId, AppUserId)
                        .Update(AppUser.Columns.IsDeleted, true)
                        .Update(AppUser.Columns.UniqueIdString, null)
                        .Update(AppUser.Columns.FacebookId, null)
                        .Execute();

                }
                catch
                {
                }
                Master.MessageCenter.DisplaySuccessMessage(AppUsersStrings.GetText(@"MessageAppUserDeleted"));
                lblDeleteConfirm.Visible = false;
                chkDeleteConfirm.Visible = false;
                btnDelete.Visible = false;
            }
        }
    }
}
