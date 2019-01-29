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
    public partial class Filters : AdminPageBase
    {
        bool HasEditPermission = false;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            Master.AddButtonNew(FiltersStrings.GetText(@"NewFilterButton"), @"EditFilter.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
            dgFilters.PageIndexChanged += dgFilters_PageIndexChanged;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgFilters.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgFilters.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle=FiltersStrings.GetText(@"FiltersPageTitle");
            Master.ActiveMenu="Filters";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }     

        protected void LoadItems(string sortField = "ProductId", dg.Sql.SortDirection sortDirection = dg.Sql.SortDirection.ASC)
        {
            if (!HasEditPermission)
            {
                dgFilters.Columns[dgFilters.Columns.Count - 1].Visible = false;
            }

            dgFilters.VirtualItemCount = ProductController.GetAllFilter().Count;
            if (dgFilters.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = FiltersStrings.GetText(@"MessageNoFiltersFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;

                if (dgFilters.PageSize * dgFilters.CurrentPageIndex > dgFilters.VirtualItemCount)
                {
                    dgFilters.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgFilters.Value = dgFilters.CurrentPageIndex.ToString();
                }
                List<FilterUI> filters = ProductController.GetAllFilter(dgFilters.PageSize, dgFilters.CurrentPageIndex);
                BindList(filters);
            }

        }

        protected void BindList(List<FilterUI> coll)
        {
            dgFilters.ItemDataBound += dgFilters_ItemDataBound;
            dgFilters.DataSource = coll;
            dgFilters.DataBind();
            Master.DisableViewState(dgFilters);
            lblTotal.Text = dgFilters.VirtualItemCount.ToString();
        }

        void dgFilters_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgFilters.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgFilters.Value = dgFilters.CurrentPageIndex.ToString();
            LoadItems();
        }

        protected void dgFilters_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                LinkButton lbDelete = e.Item.FindControl("lbDelete") as LinkButton;
                lbDelete.Visible = HasEditPermission;

                LinkButton lbEdit = e.Item.FindControl("lbEdit") as LinkButton;
                lbEdit.Visible = HasEditPermission;
            }
        }

        protected void dgFilters_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Int64 FilterId;
            if (e.CommandName.Equals("Delete"))
            {
                FilterId = Int64.Parse(dgFilters.DataKeys[index].ToString());
                Response.Redirect("DeleteFilter.aspx?FilterId=" + FilterId);
                LoadItems();
            }
            else
            {
                if (e.CommandName.Equals("Edit"))
                {
                    FilterId = Int64.Parse(dgFilters.DataKeys[index].ToString());
                    Response.Redirect("EditFilter.aspx?FilterId=" + FilterId);
                    LoadItems();
                }
            }


        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(FiltersStrings.GetText(@"FilterName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(FiltersStrings.GetText(@"SubFilterName"), typeof(string)));

            List<FilterUI> filters = ProductController.GetAllFilter();
            foreach (FilterUI filter in filters)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = filter.FilterName;
                row[i++] = ProductController.ConvertSubFilterListToString(filter.LstSubFilter);
                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, true, true);

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
    }

}
