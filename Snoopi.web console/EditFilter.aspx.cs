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
    public partial class EditFilter : AdminPageBase
    {
        static Int64 FilterId = 0;
        bool IsNewMode = false;
        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            if (!permissions.Contains(Permissions.PermissionKeys.sys_perm))
            {
                Master.LimitAccessToPage();
            }

            IsNewMode = Request.QueryString[@"New"] != null;
            FilterId = 0;

            if (!IsNewMode)
            {
                if (!Int64.TryParse(Request.QueryString[@"FilterId"], out FilterId))
                {
                    Master.LimitAccessToPage();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!IsNewMode)
                {
                    Filter f = Filter.FetchByID(FilterId);
                    txtFilterName.Text = f.FilterName;
                    LoadItems();
                }                                         
            }
        }

        public void LoadItems()
        {
            
            SubFilterCollection filters = ProductController.GetSubFilters(FilterId);
            if (FilterId != 0 && filters.Count == 0)
            {
                filters.Add(new SubFilter { SubFilterId = 0, SubFilterName = "" });
                gvFilters.DataSource = filters;
                gvFilters.DataBind();
                gvFilters.Rows[0].Visible = false;
            }
            else
            {
                gvFilters.DataSource = filters;
                gvFilters.DataBind();
            }

        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = FiltersStrings.GetText(IsNewMode ? @"NewFiltersPageTitle" : @"EditFiltersPageTitle");
            Master.AddButtonNew(FiltersStrings.GetText(@"NewFilterButton"), @"EditFilter.aspx?New=Yes", new string[] { Permissions.PermissionKeys.sys_perm });
            Master.ActiveMenu = "Filters";
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            // check if filter name already exists
            Query q = new Query(Filter.TableSchema).Where(Filter.Columns.FilterName , txtFilterName.Text.Trim())
                .AddWhere(Filter.Columns.FilterId, WhereComparision.NotEqualsTo, FilterId);
            if (q.GetCount() > 0)
            {
                Master.MessageCenter.DisplayWarningMessage(FiltersStrings.GetText(@"AlreadyExists"));
                return;
            }

            Filter filter;
            if (IsNewMode) 
                filter = new Filter();
            else
            {
                filter = Filter.FetchByID(FilterId);
                if (filter == null)
                {
                    Master.MessageCenter.DisplaySuccessMessage(FiltersStrings.GetText(@"MessageUnknownError"));
                    return;
                }
            }
            filter.FilterName = txtFilterName.Text;
            filter.Save();
            
            if(IsNewMode)
            {
                FilterId = filter.FilterId;
                string successMessage = FiltersStrings.GetText(@"MessageFilterCreated");
                string url = @"EditFilter.aspx?FilterId=" + FilterId;
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);
                return;
            }
            else
            {
                string successMessage = FiltersStrings.GetText(@"MessageFilterUpdate");
                string url = @"EditFilter.aspx?FilterId=" + FilterId;
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);
                return;
            }
        }


        protected void gvSubFolder_RowEdit(object sender, GridViewEditEventArgs e)
        {
            gvFilters.EditIndex = e.NewEditIndex;
            LoadItems();
        }

        protected void gvSubFolder_RowUpdate(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int index = e.RowIndex;
                GridViewRow row = gvFilters.Rows[index];
                Int64 Id = Int64.Parse(gvFilters.DataKeys[index].Value.ToString());
                TextBox txtSubFilterName = (TextBox)row.Cells[0].FindControl("txtSubFilterName");
                if (txtSubFilterName.Text.Trim() == "")
                {
                    Master.MessageCenter.DisplayWarningMessage(GlobalStrings.GetText(@"ErrorSubFilterName"));
                    return;
                }
                Query q = new Query(SubFilter.TableSchema).Where(SubFilter.Columns.SubFilterName, txtSubFilterName.Text.Trim())
                        .AddWhere(SubFilter.Columns.FilterId, WhereComparision.EqualsTo, FilterId)
                        .AddWhere(SubFilter.Columns.SubFilterId, WhereComparision.NotEqualsTo, Id);
                if (q.GetCount() > 0)
                {
                    Master.MessageCenter.DisplayErrorMessage(FiltersStrings.GetText(@"MessageSubFiltersAlreadyExists"));
                    return;
                }

                SubFilter subFilter = SubFilter.FetchByID(Id);
                subFilter.SubFilterName = txtSubFilterName.Text.Trim();
                subFilter.Save();
                gvFilters.EditIndex = -1;

                LoadItems();
            }
            catch (Exception) { }
        }

        protected void gvSubFolder_RowCancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvFilters.EditIndex = -1;
            LoadItems();
        }

        protected void gvSubFolder_RowDelete(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int index = e.RowIndex;
                Int64 Id = Int64.Parse(gvFilters.DataKeys[index].Value.ToString());
                Query q = new Query(ProductFilter.TableSchema).Where(ProductFilter.Columns.SubFilterId, Id);
                if (q.GetCount() > 0)
                {
                    Master.MessageCenter.DisplayErrorMessage(FiltersStrings.GetText(@"ErrorDeleteSubFilter"));
                    return;
                }
                else
                {
                    (new Query(SubFilter.TableSchema).Where(SubFilter.Columns.SubFilterId, Id).Delete()).Execute();
                    LoadItems();
                }
            }
            catch (Exception) { }
        }

        protected void gvSubFolder_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (!e.CommandName.Equals("AddNew")) return;
                
                GridViewRow row = gvFilters.FooterRow;
                int index = gvFilters.EditIndex;
                TextBox txtSubFilterNameNew = (TextBox)row.Cells[0].FindControl("txtSubFilterNameNew");
                if (txtSubFilterNameNew.Text.Trim() == "")
                {
                    Master.MessageCenter.DisplayErrorMessage(GlobalStrings.GetText(@"ErrorSubFilterName"));
                    return;
                }
                Query q = new Query(SubFilter.TableSchema).Where(SubFilter.Columns.SubFilterName, txtSubFilterNameNew.Text.Trim())
                .AddWhere(SubFilter.Columns.FilterId, WhereComparision.EqualsTo, FilterId);
                if (q.GetCount() > 0)
                {
                    Master.MessageCenter.DisplayErrorMessage(FiltersStrings.GetText(@"MessageSubFiltersAlreadyExists"));
                    return;
                }                

                SubFilter subFilter = new SubFilter();
                subFilter.FilterId = FilterId;
                subFilter.SubFilterName = txtSubFilterNameNew.Text.Trim();
                subFilter.Save();
                gvFilters.EditIndex = -1;

                // refresh the grid view data
                LoadItems();
            }
            catch (Exception) { }

        }

    }
}
