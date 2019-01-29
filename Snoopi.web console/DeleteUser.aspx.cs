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
    public partial class DeleteUser : AdminPageBase
    {
        bool HasSystemPermission = false;
        Int64 UserId;
        object UserName = null;

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);
            
            if (Int64.TryParse(Request.QueryString[@"UserId"], out UserId))
            {
                //if (!Membership.UserCanAffectUser(SessionHelper.UserId(), UserId) || UserId == SessionHelper.UserId())
                //{
                //    Master.LimitAccessToPage();
                //}
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
                        //if (!Membership.UserCanAffectUser(SessionHelper.UserId(), UserId) || UserId == SessionHelper.UserId())
                        //{
                        //    Master.LimitAccessToPage();
                        //}
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
            UserName = new Query(core.DAL.User.TableSchema).Select(core.DAL.User.Columns.Email).Where(core.DAL.User.Columns.UserId, UserId).LimitRows(1).ExecuteScalar();
            if (UserName == null)
            {
                Http.Respond404(true);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = string.Format(UsersStrings.GetText(@"DeleteUserPageTitle"), UserName);
            Master.ActiveMenu = "Users";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                core.DAL.User.Delete(UserId);
                Master.MessageCenter.DisplaySuccessMessage(UsersStrings.GetText(@"MessageUserDeleted"));
                lblDeleteConfirm.Visible = false;
                chkDeleteConfirm.Visible = false;
                btnDelete.Visible = false;
                
            }
        }
    }
}
