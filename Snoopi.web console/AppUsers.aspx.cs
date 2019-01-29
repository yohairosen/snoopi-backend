using System;
using System.Collections;
using System.Configuration;
using dg.Utilities;
using dg.Utilities.Spreadsheet;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core;
using dg.Sql;
using System.Web;
using System.Collections.Generic;

namespace Snoopi.web
{
    public partial class AppUsers : AdminPageBase
    {
        bool HasEditPermission = false;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            Master.AddButtonNew(AppUsersStrings.GetText(@"NewAppUserButton"), @"EditAppUser.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });

            dgAppUsers.PageIndexChanged += dgAppUsers_PageIndexChanging;
        }

        protected void dgAppUsers_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgAppUsers.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgAppUsers.Value = dgAppUsers.CurrentPageIndex.ToString();
            LoadItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgAppUsers.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgAppUsers.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = AppUsersStrings.GetText(@"AppUsersPageTitle");
            Master.ActiveMenu = "AppUsers";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgAppUsers.Columns[dgAppUsers.Columns.Count - 1].Visible = false;
            }
            string searchName = "%" + txtSearchName.Text.Trim() + "%";
            string searchPhone = "%" + txtSearchPhone.Text.Trim() + "%";
            DateTime from, to = new DateTime();
            DateTime.TryParse(dpSearchCreateDateFrom.Value.ToString(), out from);
            DateTime.TryParse(dpSearchCreateDateTo.Value.ToString(), out to);

            
            dgAppUsers.VirtualItemCount = AppUserUI.GetAllAppUserUI(from, to, searchName, searchPhone).Count;
            if (dgAppUsers.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = AppUsersStrings.GetText(@"MessageNoUsersFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgAppUsers.PageSize * dgAppUsers.CurrentPageIndex > dgAppUsers.VirtualItemCount)
                {
                    dgAppUsers.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgAppUsers.Value = dgAppUsers.CurrentPageIndex.ToString();
                }
                List<AppUserUI> app_users = AppUserUI.GetAllAppUserUI(from, to, searchName, searchPhone, dgAppUsers.PageSize, dgAppUsers.CurrentPageIndex);
                BindList(app_users);
            }



        }
        protected void BindList(List<AppUserUI> coll)
        {
            dgAppUsers.ItemDataBound += dgAppUsers_ItemDataBound;
            dgAppUsers.DataSource = coll;
            dgAppUsers.DataBind();
            Master.DisableViewState(dgAppUsers);
            lblTotal.Text = dgAppUsers.VirtualItemCount.ToString();
        }


        protected void dgAppUsers_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                LinkButton lbEdit = e.Item.FindControl("lbEdit") as LinkButton;
                lbEdit.Visible = HasEditPermission;

                LinkButton lbDelete = e.Item.FindControl("lbDelete") as LinkButton;
                lbDelete.Visible = HasEditPermission;

                LinkButton lbOrder = e.Item.FindControl("lbOrder") as LinkButton;
                lbOrder.Visible = HasEditPermission;
            }
        }


        protected void dgAppUsers_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Int64 AppUserId;
            if (e.CommandName.Equals("Edit"))
            {
                AppUserId = Int64.Parse(dgAppUsers.DataKeys[index].ToString());
                Response.Redirect("EditAppUser.aspx?AppUserId=" + AppUserId);
                LoadItems();
            }
            else if (e.CommandName.Equals("Delete"))
            {
                AppUserId = Int64.Parse(dgAppUsers.DataKeys[index].ToString());
                Response.Redirect("DeleteAppUser.aspx?AppUserId=" + AppUserId);
                LoadItems();
            }
            else if (e.CommandName.Equals("Order"))
            {
                AppUserId = Int64.Parse(dgAppUsers.DataKeys[index].ToString());
                Response.Redirect("AppUserOrders.aspx?AppUserId=" + AppUserId);
                LoadItems();
            }


        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"Email"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"FirstName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"LastName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"IsLocked"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"Phone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"Address"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"IsAdv"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"LastLogin"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"CreateDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"UserId"), typeof(string)));

            string searchName = "%" + txtSearchName.Text.Trim() + "%";
            string searchPhone = "%" + txtSearchPhone.Text.Trim() + "%";
            DateTime from, to = new DateTime();
            DateTime.TryParse(dpSearchCreateDateFrom.Value.ToString(), out from);
            DateTime.TryParse(dpSearchCreateDateTo.Value.ToString(), out to);

            List<AppUserUI> app_users = AppUserUI.GetAllAppUserUI(from, to, searchName, searchPhone);

            foreach (AppUserUI appUser in app_users)
            {
                System.Data.DataRow row = dt.NewRow();
                row[0] = appUser.Email;
                row[1] = appUser.FirstName;
                row[2] = appUser.LastName;
                row[3] = GlobalStrings.GetYesNo(appUser.IsLocked);
                row[4] = "\"" + appUser.Phone + "\"";
                row[5] = appUser.Street + " " + appUser.HouseNum + "\n"
                    + AppUsersStrings.GetText(@"Floor") + " " + appUser.Floor + "\n"
                    + AppUsersStrings.GetText(@"ApartmentNumber") + " " + appUser.ApartmentNumber + "\n"
                    + appUser.CityName;
                row[6] = GlobalStrings.GetYesNo(appUser.IsAdv);
                row[7] = "\"" + appUser.LastLogin + "\"";
                row[8] = "\"" + appUser.CreateDate + "\"";
                row[9] = "\"" + appUser.AppUserId + "\"";

                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=AppUsersExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }
        #endregion

    }

}
