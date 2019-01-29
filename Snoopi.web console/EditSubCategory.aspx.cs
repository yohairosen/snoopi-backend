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
    public partial class EditSubCategory : AdminPageBase
    {
        Int64 SubCategoryId;
        bool IsNewMode = false;

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            if (!permissions.Contains(Permissions.PermissionKeys.sys_perm))
            {
                Master.LimitAccessToPage();
            }

            IsNewMode = Request.QueryString[@"New"] != null;

            if (!IsNewMode)
            {
                if (!Int64.TryParse(Request.QueryString[@"SubCategoryId"], out SubCategoryId))
                {
                    Master.LimitAccessToPage();
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(CategoriesStrings.GetText(@"NewSubCategoryButton"), @"EditSubCategory.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //fill mainCategory ddl
                CategoryCollection categoryList = CategoryCollection.FetchAll();
                foreach (var item in categoryList)
                {
                    ddlCategory.Items.Add(new ListItem(item.CategoryName, item.CategoryId.ToString()));
                }
                if (ddlFilters.Items.Count == 0)
                {
                    List<FilterUI> filters = ProductController.GetAllFilter();
                    foreach (var item in filters)
                    {
                        ddlFilters.Items.Add(new ListItem(item.FilterName, item.FilterId.ToString()));
                    }
                }

                LoadView();
            }
        }

        protected void LoadView()
        {
            string ImageFile = "";
            if (SubCategoryId > 0)
            {
                SubCategory category = SubCategory.FetchByID(SubCategoryId);
                txtCategoryName.Text = category.SubCategoryName;
               // txtCategoryName.ReadOnly = true;
                tr_main_category.Visible = false;
                ImageFile = Snoopi.core.MediaUtility.GetImagePath("SubCategory", category.SubCategoryImage, 0, 64, 64);
                //ImageFileHandler(fuImage, imgImage, btnDeleteImage, ImageFile);

            }
            //TODO
            //ImageFileHandler(fuImage, imgImage, btnDeleteImage, MediaUtility.GetAdminImageFileUrl(ProductId, MediaUtility.SUBFOLDER_PRODUCTS, ImageFile, 64, 64));
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = CategoriesStrings.GetText(IsNewMode ? @"NewSubCategoryPageTitle" : @"EditSubCategoryPageTitle");
            Master.ActiveMenu = "NewSubCategory";
            rfvCategoryName.Visible = rfvCategoryName.Enabled;
        }


        protected void btnDeleteImage_Click(object sender, System.EventArgs e)
        {
            //ImageFileHandler(fuImage, imgImage, btnDeleteImage);
        }

        private void ImageFileHandler(FileUpload fu, System.Web.UI.WebControls.Image img, Button btn, string value = "")
        {
            //if (String.IsNullOrEmpty(value))
            //{
            //    fuImage.Visible = true;
            //    imgImage.Visible = false;
            //    btnDeleteImage.Visible = false;
            //}
            //else
            //{
            //    fuImage.Visible = false;
            //    imgImage.Visible = true;
            //    imgImage.ImageUrl = value;
            //    btnDeleteImage.Visible = true;
            //}
        }

        private List<Int64> FillFiltersrList()
        {
            List<Int64> FiltersIdList = new List<Int64>();
            foreach (ListItem item in ddlFilters.Items)
            {
                if (item.Selected)
                {
                    FiltersIdList.Add(Convert.ToInt64(item.Value));
                }
            }
            return FiltersIdList;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SubCategory subCategory;
            if (!Page.IsValid) return;
            if (IsNewMode)
            {
                Int64 CategoryId = ddlCategory.SelectedValue != null ? Convert.ToInt64(ddlCategory.SelectedValue) : 0;
                subCategory = SubCategory.FetchByName(txtCategoryName.Text, CategoryId);
                if (subCategory != null)
                {
                    Master.MessageCenter.DisplayErrorMessage(CategoriesStrings.GetText(@"MessageSubCategoryAlreadyExists"));
                    return;
                }
                subCategory = new SubCategory();
                subCategory.SubCategoryName = txtCategoryName.Text;
                subCategory.CategoryId = Convert.ToInt64(ddlCategory.SelectedValue);
            }
            else
            {
                subCategory = SubCategory.FetchByID(SubCategoryId);
                subCategory.SubCategoryName = txtCategoryName.Text;
                //if(txtCategoryName.Text != subCategory.SubCategoryName)

            }
            if (subCategory == null)
            {
                Master.MessageCenter.DisplayErrorMessage(CategoriesStrings.GetText(@"MessageUnknownError"));
                return;
            }
            subCategory.Save();

            List<Int64> FiltersToSave = FillFiltersrList();
            if (FiltersToSave.Count > 0)
            {
                foreach (Int64 item in FiltersToSave)
                {
                    SubCategoryFilter subCategoryFilter = SubCategoryFilter.FetchByID(subCategory.SubCategoryId, item, subCategory.CategoryId);
                    if (subCategoryFilter == null)
                    {
                        subCategoryFilter = new SubCategoryFilter();
                        subCategoryFilter.SubCategoryId = subCategory.SubCategoryId;
                        subCategoryFilter.FilterId = item;
                        subCategoryFilter.CategoryId = subCategory.CategoryId;
                        subCategoryFilter.Save();
                    }
                }
            }

            //if (fuImage.HasFile)
            //{
            //    if (!IsNewMode) MediaUtility.DeleteImageFilePath("SubCategory", subCategory.SubCategoryImage, 64, 64, 0);
            //    string fn = MediaUtility.SaveFile(fuImage.PostedFile, "SubCategory", 0);
            //    subCategory.SubCategoryImage = fn;
            //    imgImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("SubCategory", subCategory.SubCategoryImage, 0, 64, 64);
            //    ImageFileHandler(fuImage, imgImage, btnDeleteImage, imgImage.ImageUrl);
            //}
            //else if (subCategory.SubCategoryImage != "" && fuImage.Visible)
            //{
            //    MediaUtility.DeleteImageFilePath("SubCategory", subCategory.SubCategoryImage, 64, 64, 0);
            //    subCategory.SubCategoryImage = "";
            //}


            SubCategoryId = subCategory.SubCategoryId;
            string successMessage = IsNewMode ? CategoriesStrings.GetText(@"MessageSubCategoryCreated") : CategoriesStrings.GetText(@"MessageSubCategorySaved");
            string url = @"EditSubCategory.aspx?New=yes";
            url += @"&message-success=" + Server.UrlEncode(successMessage);
            Response.Redirect(url, true);

        }
        protected void btnBackToSubCategory_Click(object sender, EventArgs e)
        {
            string url = @"SubCategories.aspx";
            Response.Redirect(url, true);
        }

        
    }
}
