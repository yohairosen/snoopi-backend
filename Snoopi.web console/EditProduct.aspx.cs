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
    public partial class EditProduct : AdminPageBase
    {
        Int64 ProductId;
        bool IsNewMode = false;
        Int64 FilterId;
        Int64 ProductPage;

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
                if (!Int64.TryParse(Request.QueryString[@"ProductId"], out ProductId))
                {
                    Master.LimitAccessToPage();
                }
                if (!Int64.TryParse(Request.QueryString[@"page"], out ProductPage))
                {
                    ProductPage = 0;
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(ProductsStrings.GetText(@"NewProductButton"), @"EditProduct.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                hfOriginalProductId.Value = ProductId.ToString();
                LoadView();
            }
            else
            {
                if (hfOriginalProductId.Value.Length > 0 && hfOriginalProductId.Value != ProductId.ToString())
                {
                    Http.Respond404(true);
                }
            }

            if (IsNewMode)
            {
                isNew.Visible = true;
            }
        }

        protected void LoadView()
        {
            string ImageFile = "";
            if (ProductId > 0)
            {
                decimal minPrice, maxPrice;
                Product product = ProductController.GetProductById(ProductId, null, out minPrice, out maxPrice);
                txtProductName.Text = product.ProductName;
                txtProductCode.Text = product.ProductCode;
                txtProductNum.Text = product.ProductNum.ToString();
                ImageFile = Snoopi.core.MediaUtility.GetImagePath("Product", product.ProductImage, 0, 64, 64);
                if (ImageFile.ToLower().Contains(".jpg") || ImageFile.ToLower().Contains(".jpeg") || ImageFile.ToLower().Contains(".png"))
                    ImageFileHandler(fuImage, imgImage, btnDeleteImage, ImageFile);
                txtProductAmount.Text = product.Amount;
                txtProductDescription.Text = product.Description;
                txtRecomendedPrice.Text = product.RecomendedPrice.ToString();
            }

            LoadItems(ProductId);
            //TODO
            //ImageFileHandler(fuImage, imgImage, btnDeleteImage, MediaUtility.GetAdminImageFileUrl(ProductId, MediaUtility.SUBFOLDER_PRODUCTS, ImageFile, 64, 64));

            AnimalCollection animalList = AnimalCollection.FetchAll();
            int index = 0;
            foreach (var item in animalList)
            {
                ddlAnimalType.Items.Add(new ListItem(item.AnimalName, item.AnimalId.ToString()));
                if (ProductAnimal.FetchByID(ProductId, item.AnimalId) != null)
                {
                    ddlAnimalType.Items[index].Selected = true;
                }
                index++;
            }

            FillDdlCategory(ProductId);
            FillDdlSubCategory(ProductId, Convert.ToInt64(ddlCategory.SelectedItem.Value));
        }

        protected void LoadItems(Int64 ProductId = 0)
        {
            List<FilterUI> filters = ProductController.GetAllFilter();

            gvFilters.DataSource = filters;
            gvFilters.DataBind();
        }

        private void FillDdlFilter(DropDownList ddlFilter)
        {
            FilterCollection filterList = FilterCollection.FetchAll();
            ddlFilter.Items.Add(new ListItem(GlobalStrings.GetText(@"NoneForDropDowns"), "0"));
            foreach (var item in filterList)
            {
                ddlFilter.Items.Add(new ListItem(item.FilterName, item.FilterId.ToString()));
            }
        }

        private void FillDdlSubFilter(Int64 ProductId, Int64 FilterId, CheckBoxList ddlSubFilter)
        {
            ddlSubFilter.Items.Clear();
            List<FilterUI> filetrList = ProductController.GetAllFilter();
            foreach (var filter in filetrList)
            {
                if (filter.FilterId == FilterId)
                {
                    int index = 0;
                    foreach (var subFilter in filter.LstSubFilter)
                    {
                        ddlSubFilter.Items.Add(new ListItem(subFilter.SubFilterName, subFilter.SubFilterId.ToString()));
                        if (ProductFilter.FetchByID(ProductId, FilterId, subFilter.SubFilterId) != null)
                        {
                            ddlSubFilter.Items[index].Selected = true;
                        }
                        index++;
                    }
                }

            }
        }

        private void FillDdlCategory(Int64 ProductId)
        {
            Int64 selectedCategory = Product.FetchByID(ProductId) == null ? 0 : Product.FetchByID(ProductId).CategoryId;
            List<Category> categoryList = ProductController.getAllCategoriesWithSubCategories();
            int index = 0;
            if (categoryList.Count>0)
            {
                foreach (var item in categoryList)
                {
                    ddlCategory.Items.Add(new ListItem(item.CategoryName, item.CategoryId.ToString()));
                    if (selectedCategory == item.CategoryId)
                    {
                        ddlCategory.Items[index].Selected = true;
                    }
                    index++;
                } 
            }
        }

        private void FillDdlSubCategory(Int64 ProductId, Int64 CategoryId)
        {
            Int64 selectedSubCategory = Product.FetchByID(ProductId) == null ? 0 : Product.FetchByID(ProductId).SubCategoryId;
            Query q = new Query(SubCategory.TableSchema).Where(SubCategory.Columns.CategoryId, CategoryId).SelectAll();
            SubCategoryCollection subCategoryList = SubCategoryCollection.FetchByQuery(q);
            int index = 0;
            ddlSubCategory.Items.Clear();
            if (subCategoryList.Count>0)
            {
                foreach (var item in subCategoryList)
                {
                    ddlSubCategory.Items.Add(new ListItem(item.SubCategoryName, item.SubCategoryId.ToString()));
                    if (selectedSubCategory == item.SubCategoryId)
                    {
                        ddlSubCategory.Items[index].Selected = true;
                    }
                    index++;
                } 
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = ProductsStrings.GetText(IsNewMode ? @"NewProductPageTitle" : @"EditProductPageTitle");
            Master.ActiveMenu = IsNewMode ? "NewProduct" : "Products";

            //rfvProductName.Visible = rfvProductName.Enabled = IsNewMode;
            //rfvProductCode.Visible = rfvProductCode.Enabled = IsNewMode;
            //rfvProductAmount.Visible = rfvProductAmount.Enabled = IsNewMode;
            //rfvProductDescription.Visible = rfvProductDescription.Enabled = IsNewMode;
        }

        protected void btnDeleteImage_Click(object sender, System.EventArgs e)
        {
            ImageFileHandler(fuImage, imgImage, btnDeleteImage);
        }

        private void ImageFileHandler(FileUpload fu, System.Web.UI.WebControls.Image img, Button btn, string value = "")
        {
            if (String.IsNullOrEmpty(value))
            {
                fuImage.Visible = true;
                imgImage.Visible = false;
                btnDeleteImage.Visible = false;
            }
            else
            {
                fuImage.Visible = false;
                imgImage.Visible = true;
                imgImage.ImageUrl = value;
                btnDeleteImage.Visible = true;
            }
        }


        protected void gvFilters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Fill checked dropDownList
                CheckBoxList ddl = (CheckBoxList)e.Row.FindControl("ddlSubFilter") as CheckBoxList;
                Int64 filterId = Int64.Parse(gvFilters.DataKeys[e.Row.RowIndex].Value.ToString());
                foreach (ListItem item in ddl.Items)
                {
                    if (ProductFilter.FetchByID(ProductId, filterId, Convert.ToInt64(item.Value)) != null)
                    {
                        item.Selected = true;
                    }
                }
            }

        }

        protected void btnBackToProductList_Click(object sender, EventArgs e)
        {
           Response.Redirect("Products.aspx?page=" + ProductPage);
       //   Response.Redirect("Products.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            Product product = null;
            if (IsNewMode)
            {
                product = new Product();
                product.SendSupplier = cbxIsSendSupplier.Checked;
                product.IsDeleted = false;
            }
            else
            {
                product = Product.FetchByID(ProductId);
            }
            Product p = Product.FetchByCode(txtProductCode.Text);
            if (p != null && p.ProductId != ProductId)
            {
                Master.MessageCenter.DisplayErrorMessage(ProductsStrings.GetText(@"ProductCodeAlreadyExists"));
                return;
            }
            if (txtProductNum.Text != "")
            {
                Product p1 = Product.FetchByProductNum(Convert.ToInt64(txtProductNum.Text));
                if (p1 != null && p1.ProductId != ProductId)
                {
                    Master.MessageCenter.DisplayErrorMessage(ProductsStrings.GetText(@"ProductNumAlreadyExists"));
                    return;
                }
                else
                    product.ProductNum = Convert.ToInt64(txtProductNum.Text);
            }
            else
                product.ProductNum = null;
            product.ProductName = txtProductName.Text;
            product.ProductCode = txtProductCode.Text;
            product.Amount = txtProductAmount.Text;
            product.Description = txtProductDescription.Text;
            product.CategoryId = Convert.ToInt64(ddlCategory.SelectedValue);
            product.SubCategoryId = Convert.ToInt64(ddlSubCategory.SelectedValue);
            product.RecomendedPrice = txtRecomendedPrice.Text.Trim() != "" ? Convert.ToDecimal(txtRecomendedPrice.Text.Trim()) : 0;
            if (fuImage.HasFile)
            {
                if (!IsNewMode) MediaUtility.DeleteImageFilePath("Product", product.ProductImage, 64, 64, 0);
                string fn = MediaUtility.SaveFile(fuImage.PostedFile, "Product", 0);
                product.ProductImage = fn;
                imgImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Product", product.ProductImage, 0, 64, 64);
                ImageFileHandler(fuImage, imgImage, btnDeleteImage, imgImage.ImageUrl);
            }
            else if (product.ProductImage != "" && fuImage.Visible)
            {
                MediaUtility.DeleteImageFilePath("Product", product.ProductImage, 64, 64, 0);
                product.ProductImage = "";
            }

            product.Save();
            ProductId = product.ProductId;
            int count = 0;
            foreach (ListItem item in ddlAnimalType.Items)
            {
                if (item.Selected)
                {
                    count++;
                    ProductAnimal productAnimal = ProductAnimal.FetchByID(ProductId, Convert.ToInt64(item.Value));
                    if (productAnimal == null)
                    {
                        productAnimal = new ProductAnimal();
                        productAnimal.ProductId = ProductId;
                        productAnimal.AnimalId = Convert.ToInt64(item.Value);
                        productAnimal.Save();
                    }
                }
                else
                {
                    ProductAnimal.Delete(ProductId, Convert.ToInt64(item.Value));
                }
            }
            int index = 0;
            //save filters
            foreach (GridViewRow row in gvFilters.Rows)
            {
                Int64 FilterId = Int64.Parse(gvFilters.DataKeys[index].Value.ToString());
                CheckBoxList lsbx = (CheckBoxList)row.FindControl("ddlSubFilter");

                foreach (ListItem item in lsbx.Items)
                {
                    ProductFilter productFilter = ProductFilter.FetchByID(ProductId, FilterId, Convert.ToInt64(item.Value));
                    if (productFilter == null && item.Selected)
                    {
                        productFilter = new ProductFilter();
                        productFilter.ProductId = ProductId;
                        productFilter.FilterId = FilterId;
                        productFilter.SubFilterId = Convert.ToInt64(item.Value);
                        productFilter.Save();
                    }
                    else if (productFilter != null && !item.Selected)
                    {
                        (new Query(ProductFilter.TableSchema).Where(ProductFilter.Columns.ProductId, productFilter.ProductId)
                            .AddWhere(ProductFilter.Columns.FilterId, productFilter.FilterId)
                            .AddWhere(ProductFilter.Columns.SubFilterId, productFilter.SubFilterId).Delete()).Execute();
                    }
                }
                index++;
            }

            if (IsNewMode)
            {
                if (cbxIsSendSupplier.Checked)
                {
                    EmailMessagingService.SendEmailNewProductToSupplier(product);
                }
                string successMessage = ProductsStrings.GetText(@"MessageProductCreated");
                string url = @"EditProduct.aspx?ProductId=" + ProductId;
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);
            }
            else
            {
                string successMessage = ProductsStrings.GetText(@"MessageProductSaved");
                Master.MessageCenter.DisplaySuccessMessage(successMessage);
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDdlSubCategory(0, Convert.ToInt64(ddlCategory.SelectedItem.Value));
        }
    }

}
