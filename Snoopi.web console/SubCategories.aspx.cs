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
    public partial class SubCategories : AdminPageBase
    {
        bool HasEditPermission = false;
        
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            Master.AddButtonNew(CategoriesStrings.GetText(@"NewSubCategoryButton"), @"EditSubCategory.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
            dgSubCategories.PageIndexChanged += dgSubCategories_PageIndexChanged;
        }

        
           

       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //fill categories dropDown
                if (ddlCategory.Items.Count == 0)
                {
                    List<Category> categories = CategoryCollection.FetchAll();
                    ddlCategory.Items.Insert(0, new ListItem("-- please select --", "0"));
                    foreach (var item in categories)
                    {
                        ddlCategory.Items.Add(new ListItem(item.CategoryName, item.CategoryId.ToString()));
                    }
                   
                }
                
                
            }
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgSubCategories.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgSubCategories.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }



        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = CategoriesStrings.GetText(@"SubCategoriesPageTitle");
            Master.ActiveMenu = "SubCategories";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems(string sortField = "CategoryId", dg.Sql.SortDirection sortDirection = dg.Sql.SortDirection.ASC)
        {
            if (!HasEditPermission)
            {
                dgSubCategories.Columns[dgSubCategories.Columns.Count - 1].Visible = false;
            }
            if (Int64.Parse(ddlCategory.SelectedValue) != 0)
                dgSubCategories.VirtualItemCount = ProductController.GetSubCategories(Int64.Parse(ddlCategory.SelectedValue)).Count; 
            else
               dgSubCategories.VirtualItemCount = ProductController.GetSubCategories().Count;

            if (dgSubCategories.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = CategoriesStrings.GetText(@"MessageNoSubCategoriesFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;

                if (dgSubCategories.PageSize * dgSubCategories.CurrentPageIndex > dgSubCategories.VirtualItemCount)
                {
                    dgSubCategories.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgSubCategories.Value = dgSubCategories.CurrentPageIndex.ToString();
                }
                List<CategoryUI> categories = ProductController.GetSubCategories(Int64.Parse(ddlCategory.SelectedValue),dgSubCategories.PageSize, dgSubCategories.CurrentPageIndex);
                //foreach (CategoryUI item in categories)
                //{
                //    item.Filters = ProductController.GetAllFilter();                 
                //}
                
                BindList(categories);
            }

        }
        protected void BindList(List<CategoryUI> coll)
        {
            dgSubCategories.ItemDataBound += dgSubCategories_ItemDataBound;
            dgSubCategories.DataSource = coll;
            dgSubCategories.DataBind();
            Master.DisableViewState(dgSubCategories);
            lblTotal.Text = dgSubCategories.VirtualItemCount.ToString();
        }

        //if (ddlFilters.Items.Count == 0)
        //        {
        //            List<FilterUI> filters = ProductController.GetAllFilter();
        //            foreach (var item in filters)
        //            {
        //                ddlFilters.Items.Add(new ListItem(item.FilterName, item.FilterId.ToString()));
        //            }
        //        }

        void dgSubCategories_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgSubCategories.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgSubCategories.Value = dgSubCategories.CurrentPageIndex.ToString();
            LoadItems();
        }


        protected void dgSubCategories_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                LinkButton lbDelete = e.Item.FindControl("lbDelete") as LinkButton;
                lbDelete.Visible = HasEditPermission;

                ListBox ddlFilters = e.Item.FindControl("ddlFilters") as ListBox;
                List<FilterUI> filters = ProductController.GetAllFilter();
                if (ddlFilters.Items.Count == 0)
                     foreach (var item in filters)
                     {
                         ddlFilters.Items.Add(new ListItem(item.FilterName, item.FilterId.ToString()));
                     }
                //LinkButton lbEdit = e.Item.FindControl("lbEdit") as LinkButton;
                //lbEdit.Visible = HasEditPermission;
            }
        }
        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgSubCategories.CurrentPageIndex = 0;
            hfCurrentPageIndex_dgSubCategories.Value = dgSubCategories.CurrentPageIndex.ToString();

            LoadItems();

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (dgSubCategories.DataKeys.Count > 0)
            {
                //  List<ProductUI> prodlist = (List<ProductUI>)dgProducts.DataSource;
                foreach (DataGridItem item in dgSubCategories.Items)
                {
                    object obj = dgSubCategories.DataKeys[item.ItemIndex];

                    if (obj == null) continue;
                    Int64 SubCategoryId = obj != null ? Convert.ToInt64(obj) : 0;
                    TextBox txtRate = item.FindControl("txtSubCategoryRate") as TextBox;
                    SubCategory subcategory = SubCategory.FetchByID(SubCategoryId);
                    if (subcategory != null)
                    {
                        subcategory.SubCategoryRate = Convert.ToInt64(txtRate.Text);
                        subcategory.Save();
                    }
                    //Save Filters for SubCategory
                    ListBox ddlFilters = item.FindControl("ddlFilters") as ListBox;       
                    List<Int64> FiltersIdList = new List<Int64>();
                    foreach (ListItem filter in ddlFilters.Items)
                        if (filter.Selected)
                            FiltersIdList.Add(Convert.ToInt64(filter.Value));
                    if (FiltersIdList.Count > 0)
                    {
                        foreach (Int64 id in FiltersIdList)
                        {
                            SubCategoryFilter subCategoryFilter = SubCategoryFilter.FetchByID(subcategory.SubCategoryId, id, subcategory.CategoryId);
                            if (subCategoryFilter == null)
                            {
                                subCategoryFilter = new SubCategoryFilter();
                                subCategoryFilter.SubCategoryId = SubCategoryId;
                                subCategoryFilter.FilterId = id;
                                subCategoryFilter.CategoryId = subcategory.CategoryId;
                                subCategoryFilter.Save();
                            }
                        }
                    }

                }
            }

        }
        

        protected void dgSubCategories_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Int64 SubCategoryId;
            if (e.CommandName.Equals("Delete"))
            {
                SubCategoryId = Int64.Parse(dgSubCategories.DataKeys[index].ToString());
                Response.Redirect("DeleteSubCategory.aspx?SubCategoryId=" + SubCategoryId);
                LoadItems();
            }

            else
            {
                if (e.CommandName.Equals("Edit"))
                {
                    SubCategoryId = Int64.Parse(dgSubCategories.DataKeys[index].ToString());
                    Response.Redirect("EditSubCategory.aspx?SubCategoryId=" + SubCategoryId);
                    LoadItems();
                }

            }
        }


    }

}
