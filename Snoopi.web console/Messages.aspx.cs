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
using System.Web;
using System.Globalization;

namespace Snoopi.web
{
    public partial class Messages : AdminPageBase
    {
        bool HasEditPermission = false;
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }
        //protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_view_users, Permissions.PermissionKeys.sys_edit_users }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            //HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_edit_users);
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            Master.AddButtonNew(MessagesStrings.GetText(@"NewMessageButton"), @"EditMessage.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });

            dgMessages.PageIndexChanged += dgMessages_PageIndexChanging;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgMessages.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgMessages.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = MessagesStrings.GetText(@"MessagesPageTitle");
            Master.ActiveMenu = "Messages";

            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgMessages.Columns[dgMessages.Columns.Count - 1].Visible = false;
            }

            List<MessageUI> items = MessagesController.GetAllMessages("");//txtSearch.Text.Trim());

            //bool isSearchActive;
            //if (!bool.TryParse(hfIsSearchActive.Value, out isSearchActive)) isSearchActive = false;

            //dgMessages.VirtualItemCount = items.Count;
            //if (dgMessages.VirtualItemCount == 0)
            //{
            //    phHasItems.Visible = false;
            //    phHasNoItems.Visible = true;
            //    lblNoItems.Text = isSearchActive ? MessagesStrings.GetText(@"MessageNoMessagesFound") : MessagesStrings.GetText(@"MessageNoDataHere");
            //}
            //else
            //{
            //    phHasItems.Visible = true;
            //    phHasNoItems.Visible = false;

            //    if (dgMessages.PageSize * dgMessages.CurrentPageIndex > dgMessages.VirtualItemCount)
            //    {
            //        dgMessages.CurrentPageIndex = 0;
            //        hfCurrentPageIndex_dgMessages.Value = dgMessages.CurrentPageIndex.ToString();
            //    }
            //    items.GetRange(dgMessages.PageSize * dgMessages.CurrentPageIndex, (dgMessages.PageSize > items.Count ? items.Count : dgMessages.PageSize));
                    //LimitRows(dgMessages.PageSize).OffsetRows(dgMessages.PageSize * dgMessages.CurrentPageIndex);
                BindList(items);
            //}
        }
        protected void BindList(List<MessageUI> coll)
        {
            dgMessages.ItemDataBound += dgMessages_ItemDataBound;
            dgMessages.DataSource = coll;
            dgMessages.DataBind();
            Master.DisableViewState(dgMessages);
            //lblTotal.Text = dgMessages.VirtualItemCount.ToString();
        }

        protected void dgMessages_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgMessages.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgMessages.Value = dgMessages.CurrentPageIndex.ToString();
            LoadItems();
        }
        protected void dgMessages_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            
        }
        //protected void btnSearch_Click(object sender, EventArgs e)
        //{
        //    hfIsSearchActive.Value = txtSearch.Text.Trim() == "" ? false.ToString() : true.ToString();
        //    dgMessages.CurrentPageIndex = 0;
        //    hfCurrentPageIndex_dgMessages.Value = dgMessages.CurrentPageIndex.ToString();
        //    LoadItems();
        //}
        protected void dgMessages_SortCommand(object source, DataGridSortCommandEventArgs e)
        {

        }
}
}
