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
    public partial class Categories : AdminPageBase
    {
        bool HasEditPermission = false;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            Master.AddButtonNew(CategoriesStrings.GetText(@"NewCategoryButton"), @"EditCategory.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
            dgCategories.PageIndexChanged += dgCategories_PageIndexChanged;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgCategories.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgCategories.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }



        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = CategoriesStrings.GetText(@"CategoriesPageTitle");
            Master.ActiveMenu = "Categories";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems(string sortField = "CategoryId", dg.Sql.SortDirection sortDirection = dg.Sql.SortDirection.ASC)
        {
            if (!HasEditPermission)
            {
                dgCategories.Columns[dgCategories.Columns.Count - 1].Visible = false;
            }

            dgCategories.VirtualItemCount = ProductController.GetAllCategory().Count;
            if (dgCategories.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = CategoriesStrings.GetText(@"MessageNoCategoriesFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;

                if (dgCategories.PageSize * dgCategories.CurrentPageIndex > dgCategories.VirtualItemCount)
                {
                    dgCategories.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgCategories.Value = dgCategories.CurrentPageIndex.ToString();
                }
                List<CategoryUI> categories = ProductController.GetAllCategory(0, dgCategories.PageSize, dgCategories.CurrentPageIndex);
                BindList(categories);
            }

        }
        protected void BindList(List<CategoryUI> coll)
        {
            dgCategories.ItemDataBound += dgCategories_ItemDataBound;
            dgCategories.DataSource = coll;
            dgCategories.DataBind();
            Master.DisableViewState(dgCategories);
            lblTotal.Text = dgCategories.VirtualItemCount.ToString();
        }


        void dgCategories_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgCategories.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgCategories.Value = dgCategories.CurrentPageIndex.ToString();
            LoadItems();
        }


        protected void dgCategories_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                LinkButton lbDelete = e.Item.FindControl("lbDelete") as LinkButton;
                lbDelete.Visible = HasEditPermission;

                //LinkButton lbEdit = e.Item.FindControl("lbEdit") as LinkButton;
                //lbEdit.Visible = HasEditPermission;
            }
        }


        protected void dgCategories_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Int64 CategoryId;
            if (e.CommandName.Equals("Delete"))
            {
                CategoryId = Int64.Parse(dgCategories.DataKeys[index].ToString());
                Response.Redirect("DeleteCategory.aspx?CategoryId=" + CategoryId);
                LoadItems();
            }
            else
            {
                if (e.CommandName.Equals("Edit"))
                {
                    CategoryId = Int64.Parse(dgCategories.DataKeys[index].ToString());
                    Response.Redirect("EditCategory.aspx?CategoryId=" + CategoryId);
                    LoadItems();
                }
            }


        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (dgCategories.DataKeys.Count > 0)
            {
                //  List<ProductUI> prodlist = (List<ProductUI>)dgProducts.DataSource;
                foreach (DataGridItem item in dgCategories.Items)
                {
                    object obj = dgCategories.DataKeys[item.ItemIndex];

                    if (obj == null) continue;
                    Int64 CategoryId = obj != null ? Convert.ToInt64(obj) : 0;
                    TextBox txtRate = item.FindControl("txtCategoryRate") as TextBox;
                    Category category = Category.FetchByID(CategoryId);
                    if (category != null)
                    {
                        category.CategoryRate = Convert.ToInt64(txtRate.Text);
                        category.Save();
                    }

                }
            }
 
        }
        

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(CategoriesStrings.GetText(@"CategoryId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(CategoriesStrings.GetText(@"CategoryName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(CategoriesStrings.GetText(@"CategoryRate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(CategoriesStrings.GetText(@"SubCategoryId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(CategoriesStrings.GetText(@"SubCategoryName"), typeof(string)));
            List<CategoryUI> categories = ProductController.GetAllCategoriesAndSubCategories();
            foreach (CategoryUI category in categories)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = category.CategoryId;
                row[i++] = category.CategoryName;
                row[i++] = category.CategoryRate;
                row[i++] = category.SubCategoryId;
                row[i++] = category.SubCategoryName;
                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, true, true);
            ExcelSheetStyle _style = new ExcelSheetStyle();
            _style.Alignment.Horizontal = HorizontalAlignment.Center;
            ex.AddStyle(_style);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=CategoriesExport_" + DateTime.Now.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
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
