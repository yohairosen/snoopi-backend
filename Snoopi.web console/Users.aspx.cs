using System;
using System.Collections;
using System.Configuration;
using dg.Utilities;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core;
using dg.Sql;
using System.Web;

namespace Snoopi.web
{
    public partial class Users : AdminPageBase
    {
        bool HasEditPermission = false;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            Master.AddButtonNew(UsersStrings.GetText(@"NewUserButton"), @"EditUser.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });

            dgUsers.PageIndexChanged += dgUsers_PageIndexChanging;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgUsers.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgUsers.CurrentPageIndex = CurrentPageIndex;

            LoadItems();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = UsersStrings.GetText(@"UsersPageTitle");
            Master.ActiveMenu = "Users";

            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgUsers.Columns[dgUsers.Columns.Count - 1].Visible = false;
            }

            Query qry = new Query(core.DAL.User.TableSchema);

            bool isSearchActive;
            if (!bool.TryParse(hfIsSearchActive.Value, out isSearchActive)) isSearchActive = false;
            if (isSearchActive)
            {
                string searchString = "%" + txtSearch.Text.Trim() + "%";
                qry.Where(core.DAL.User.Columns.Email, WhereComparision.Like, searchString);
            }

            dgUsers.VirtualItemCount = (int)qry.GetCount(core.DAL.User.Columns.UserId);
            if (dgUsers.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = isSearchActive ? UsersStrings.GetText(@"MessageNoUsersFound") : UsersStrings.GetText(@"MessageNoDataHere");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;

                qry.OrderBy(core.DAL.User.Columns.Email, dg.Sql.SortDirection.ASC);

                if (dgUsers.PageSize * dgUsers.CurrentPageIndex > dgUsers.VirtualItemCount)
                {
                    dgUsers.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgUsers.Value = dgUsers.CurrentPageIndex.ToString();
                }
                qry.LimitRows(dgUsers.PageSize).OffsetRows(dgUsers.PageSize * dgUsers.CurrentPageIndex);

                UserCollection items = UserCollection.FetchByQuery(qry);

                BindList(items);
            }
        }
        protected void BindList(UserCollection coll)
        {
            dgUsers.ItemDataBound += dgUsers_ItemDataBound;
            dgUsers.DataSource = coll;
            dgUsers.DataBind();
            Master.DisableViewState(dgUsers);
            lblTotal.Text = dgUsers.VirtualItemCount.ToString();
        }

        protected void dgUsers_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgUsers.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgUsers.Value = dgUsers.CurrentPageIndex.ToString();
            LoadItems();
        }
        protected void dgUsers_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || 
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                HyperLink hlEdit = e.Item.FindControl("hlEdit") as HyperLink;
                hlEdit.Visible = HasEditPermission;

                HyperLink hlDelete = e.Item.FindControl("hlDelete") as HyperLink;
                hlDelete.Visible = HasEditPermission;
            }
        }
        protected string FormatEditUrl(object item)
        {
            return string.Format("EditUser.aspx?Email={0}", HttpUtility.UrlEncode(((User)item).Email));
        }
        protected string FormatDeleteUrl(object item)
        {
            return string.Format("DeleteUser.aspx?Email={0}", HttpUtility.UrlEncode(((User)item).Email));
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            hfIsSearchActive.Value = txtSearch.Text.Trim() == "" ? false.ToString() : true.ToString();
            dgUsers.CurrentPageIndex = 0;
            hfCurrentPageIndex_dgUsers.Value = dgUsers.CurrentPageIndex.ToString();
            LoadItems();
        }
    }
}
