using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using dg.Utilities;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using dg.Utilities.Spreadsheet;
using System.Web;

namespace Snoopi.web
{
    public partial class SupplierProducts : AdminPageBase
    {
        bool HasEditPermission = false;
        bool HasSystemPermission = false;
        Int64 SupplierId;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
            dgSupplierProducts.PageIndexChanged += dgSupplierProducts_PageIndexChanged;
        }

        void dgSupplierProducts_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgSupplierProducts.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgSupplierProducts.Value = dgSupplierProducts.CurrentPageIndex.ToString();
            LoadItems();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadItems();
        }

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);

            if (!Int64.TryParse(Request.QueryString[@"SupplierId"], out SupplierId))
            {
                Master.LimitAccessToPage();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = string.Format(ProductsStrings.GetText(@"SupplierProductsPageTitle"), AppSupplier.FetchByID(SupplierId).BusinessName);
            Master.ActiveMenu = "Suppliers";
        }

        protected void LoadItems()
        {

            if (!HasEditPermission)
            {
                dgSupplierProducts.Columns[dgSupplierProducts.Columns.Count - 1].Visible = false;
            }


            dgSupplierProducts.VirtualItemCount = ProductController.GetSupplierProducts(SupplierId).Count;
            if (dgSupplierProducts.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgSupplierProducts.PageSize * dgSupplierProducts.CurrentPageIndex > dgSupplierProducts.VirtualItemCount)
                {
                    dgSupplierProducts.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgSupplierProducts.Value = dgSupplierProducts.CurrentPageIndex.ToString();
                }
                List<ProductUI> products = ProductController.GetSupplierProducts(SupplierId, dgSupplierProducts.PageSize, dgSupplierProducts.CurrentPageIndex);
                BindList(products);
            }

            if (phHasNoItems.Visible) dgSupplierProducts.Visible = false;

        }

        protected void BindList(List<ProductUI> coll)
        {
            dgSupplierProducts.DataSource = coll;
            dgSupplierProducts.DataBind();
            Master.DisableViewState(dgSupplierProducts);
            lblTotal.Text = dgSupplierProducts.VirtualItemCount.ToString();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductCode"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"Amount"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductPrice"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"Gift"), typeof(string)));
            

            List<ProductUI> products = ProductController.GetSupplierProducts(SupplierId);
            foreach (ProductUI product in products)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = product.ProductName;
                row[i++] = product.ProductCode;
                row[i++] = product.Amount;
                row[i++] = product.ProductPrice;
                row[i++] = product.Gift;
                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, true, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=SupplierProductsExport_" + DateTime.Now.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
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
