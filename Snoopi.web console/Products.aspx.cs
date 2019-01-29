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
    public partial class Products : AdminPageBase
    {
        bool HasEditPermission = false;
        Int64 ProductPage;
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
            Master.AddButtonNew(ProductsStrings.GetText(@"NewProductButton"), @"EditProduct.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
            dgProducts.PageIndexChanged += dgProducts_PageIndexChanged;

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
                    if (ddlCategory.SelectedItem != null)
                        ddlSubCategory.Items.Insert(0, new ListItem("-- please select --", "0"));

                }
            }
            if (!Int64.TryParse(Request.QueryString[@"page"], out ProductPage))
            {
                ProductPage = 0;
            }
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgProducts.Value, out CurrentPageIndex))
                CurrentPageIndex = 0;

            else if (ProductPage > 0)
                CurrentPageIndex = (int)ProductPage;
            //  ProductPage = CurrentPageIndex;

            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgProducts.CurrentPageIndex = CurrentPageIndex;
            LoadItems();

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = ProductsStrings.GetText(@"ProductsPageTitle");
            Master.ActiveMenu = "Products";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgProducts.Columns[dgProducts.Columns.Count - 1].Visible = false;
            }

            string searchCode = "%" + txtSearchCode.Text.Trim() + "%";
            if (Int64.Parse(ddlCategory.SelectedValue) != 0)
            {
                dgProducts.VirtualItemCount = ProductController.GetAllProductUI(searchCode, Int64.Parse(ddlCategory.SelectedValue), 0).Count;
                if (Int64.Parse(ddlSubCategory.SelectedValue) != 0)
                    dgProducts.VirtualItemCount = ProductController.GetAllProductUI(searchCode, Int64.Parse(ddlCategory.SelectedValue), Int64.Parse(ddlSubCategory.SelectedValue)).Count;
            }
            else
                dgProducts.VirtualItemCount = ProductController.GetAllProductUI(searchCode).Count;

            if (dgProducts.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = ProductsStrings.GetText(@"MessageNoProductsFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;

                if (dgProducts.PageSize * dgProducts.CurrentPageIndex > dgProducts.VirtualItemCount)
                {
                    dgProducts.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgProducts.Value = dgProducts.CurrentPageIndex.ToString();
                }
                List<ProductUI> products = ProductController.GetAllProductUI(searchCode, Int64.Parse(ddlCategory.SelectedValue), Int64.Parse(ddlSubCategory.SelectedValue), dgProducts.PageSize, dgProducts.CurrentPageIndex);
                BindList(products);
            }

        }
        protected void BindList(List<ProductUI> coll)
        {

            dgProducts.ItemDataBound += dgProducts_ItemDataBound;
            dgProducts.DataSource = coll;
            dgProducts.DataBind();
            Master.DisableViewState(dgProducts);
            lblTotal.Text = dgProducts.VirtualItemCount.ToString();
        }

        void dgProducts_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {

            dgProducts.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgProducts.Value = dgProducts.CurrentPageIndex.ToString();

            if (Int64.Parse(ddlCategory.SelectedValue) == 0)
            {
                string urlPath = HttpContext.Current.Request.Url.AbsoluteUri;
                Uri uri = new Uri(urlPath);
                string url = uri.GetLeftPart(UriPartial.Path);
                Response.Redirect(string.Format("{0}?page={1}", url, dgProducts.CurrentPageIndex));
            }
            LoadItems();
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgProducts.CurrentPageIndex = 0;
            hfCurrentPageIndex_dgProducts.Value = dgProducts.CurrentPageIndex.ToString();

            ddlSubCategory.Items.Clear();
            List<SubCategory> subcategories = ProductController.GetSubCategoryByCategoryID(Int64.Parse(ddlCategory.SelectedValue));
            ddlSubCategory.Items.Insert(0, new ListItem("-- please select --", "0"));
            foreach (var item in subcategories)
            {
                ddlSubCategory.Items.Add(new ListItem(item.SubCategoryName, item.SubCategoryId.ToString()));
            }
            LoadItems();

        }

        protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgProducts.CurrentPageIndex = 0;
            hfCurrentPageIndex_dgProducts.Value = dgProducts.CurrentPageIndex.ToString();

            LoadItems();
        }

        protected void dgProducts_ItemDataBound(object sender, DataGridItemEventArgs e)
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


        protected void dgProducts_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Int64 ProductId;
            if (e.CommandName.Equals("Delete"))
            {
                ProductId = Int64.Parse(dgProducts.DataKeys[index].ToString());
                Response.Redirect("DeleteProduct.aspx?ProductId=" + ProductId);
                LoadItems();
            }
            else
            {
                if (e.CommandName.Equals("Edit"))
                {
                    ProductId = Int64.Parse(dgProducts.DataKeys[index].ToString());
                    Response.Redirect("EditProduct.aspx?ProductId=" + ProductId + "&&page=" + dgProducts.CurrentPageIndex);
                    // Response.Redirect("EditProduct.aspx?ProductId=" + ProductId);
                    LoadItems();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (dgProducts.DataKeys.Count > 0)
            {
                //  List<ProductUI> prodlist = (List<ProductUI>)dgProducts.DataSource;
                foreach (DataGridItem item in dgProducts.Items)
                {
                    object obj = dgProducts.DataKeys[item.ItemIndex];

                    if (obj == null) continue;
                    Int64 ProductId = obj != null ? Convert.ToInt64(obj) : 0;
                    TextBox txtRate = item.FindControl("txtProductRate") as TextBox;
                    Product product = Product.FetchByID(ProductId);
                    if (product != null)
                    {
                        product.ProductRate = Convert.ToInt64(txtRate.Text);
                        product.Save();
                    }

                }
            }

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductCode"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"Amount"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"RecomendedPrice"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"Description"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"AnimalType"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"Category"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"SubCategory"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductRate"), typeof(string)));

            string searchCode = "%" + txtSearchCode.Text.Trim() + "%";

            List<ProductUI> products = ProductController.GetAllProductUI(searchCode, Int64.Parse(ddlCategory.SelectedValue), Int64.Parse(ddlSubCategory.SelectedValue));
            foreach (ProductUI product in products)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = product.ProductName;
                row[i++] = product.ProductCode;
                row[i++] = product.Amount;
                row[i++] = product.RecomendedPrice;
                row[i++] = product.Description;
                row[i++] = ProductController.ConvertListToString(product.AnimalLst);
                row[i++] = product.CategoryName;
                row[i++] = product.SubCategoryName;
                row[i++] = product.ProductRate;
                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, true, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=ProductsExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }
    }

}
