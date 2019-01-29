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
    public partial class EditCategory : AdminPageBase
    {
        Int64 CategoryId;
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
                if (!Int64.TryParse(Request.QueryString[@"CategoryId"], out CategoryId))
                {
                    Master.LimitAccessToPage();
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(CategoriesStrings.GetText(@"NewCategoryButton"), @"EditCategory.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = CategoriesStrings.GetText(IsNewMode ? @"NewCategoryPageTitle" : @"EditCategoryPageTitle");
            Master.ActiveMenu = "NewCategory";
            rfvCategoryName.Visible = rfvCategoryName.Enabled;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadView();
            }
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

        protected void LoadView()
        {
            string ImageFile = "";
            if (CategoryId > 0)
            {
                Category category = Category.FetchByID(CategoryId);
                txtCategoryName.Text = category.CategoryName;
                //txtCategoryName.ReadOnly = true;
                ImageFile = Snoopi.core.MediaUtility.GetImagePath("Category", category.CategoryImage, 0, 64, 64);
                //ImageFileHandler(fuImage, imgImage, btnDeleteImage, ImageFile);

            }
            //TODO
            //ImageFileHandler(fuImage, imgImage, btnDeleteImage, MediaUtility.GetAdminImageFileUrl(ProductId, MediaUtility.SUBFOLDER_PRODUCTS, ImageFile, 64, 64));
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            Category category;
            if (!Page.IsValid) return;
            if (IsNewMode)
            {
                category = Category.FetchByName(txtCategoryName.Text);
                if (category != null)
                {
                    Master.MessageCenter.DisplayErrorMessage(CategoriesStrings.GetText(@"MessageCategoryAlreadyExists"));
                    return;
                }
                category = new Category();
                category.CategoryName = txtCategoryName.Text;
            }
            else
                category = Category.FetchByID(CategoryId);
            if (category == null)
            {
                Master.MessageCenter.DisplayErrorMessage(CategoriesStrings.GetText(@"MessageUnknownError"));
                return;
            }

            
            //if (fuImage.HasFile)
            //{
            //    if (!IsNewMode) MediaUtility.DeleteImageFilePath("Category", category.CategoryImage, 64, 64, 0);
            //    string fn = MediaUtility.SaveFile(fuImage.PostedFile, "Category", 0);
            //    category.CategoryImage = fn;
            //    imgImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Category", category.CategoryImage, 0, 64, 64);
            //    ImageFileHandler(fuImage, imgImage, btnDeleteImage, imgImage.ImageUrl);
            //}
            //else if (category.CategoryImage != "" && fuImage.Visible)
            //{
            //    MediaUtility.DeleteImageFilePath("Category", category.CategoryImage, 64, 64, 0);
            //    category.CategoryImage = "";
            //}
            category.Save();

            CategoryId = category.CategoryId;
            string successMessage = IsNewMode ? CategoriesStrings.GetText(@"MessageCategoryCreated") : CategoriesStrings.GetText(@"MessageCategorySaved");
            string url = @"EditCategory.aspx?New=yes";
            url += @"&message-success=" + Server.UrlEncode(successMessage);
            Response.Redirect(url, true);


        }

    }
}
