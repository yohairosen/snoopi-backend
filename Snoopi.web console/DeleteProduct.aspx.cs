using System;
using System.Collections;
using System.Configuration;
using Snoopi.core.DAL;
using System.Web.UI.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using dg.Utilities;
using Snoopi.web.WebControls;
using System.Web.UI;
using Snoopi.core;
using System.IO;
using System.Collections.Generic;
using dg.Sql;

namespace Snoopi.web
{
    public partial class DeleteProduct : AdminPageBase
    {
        bool HasSystemPermission = false;
        Int64 ProductId;
        object ProductName;

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Int64.TryParse(Request.QueryString[@"ProductId"], out ProductId))
            {
                //if (ProductController.IsSupplierProduct(ProductId))
                //{
                //    Master.MessageCenter.DisplayErrorMessage(ProductsStrings.GetText(@"MessageDeleteFailedSuppliersProductInUse"));
                //    pnlDelete.Visible = false;
                //}
            }
            ProductName = new Query(Product.TableSchema).Select(Product.Columns.ProductName).Where(Product.Columns.ProductId, ProductId).ExecuteScalar();

            if (ProductName == null)
            {
                Http.Respond404(true);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = string.Format(ProductsStrings.GetText(@"DeleteProductPageTitle"), ProductName);
            Master.ActiveMenu = "Products";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Product p = Product.FetchByID(ProductId);
                p.IsDeleted = true;
                p.Save();

                ProductFilterCollection coll = ProductController.GetProductFilterForProduct(ProductId);
                if (coll.Count > 0)
                {
                    foreach (ProductFilter item in coll)
                    {
                        ProductFilter.Delete(ProductId, item.FilterId, item.SubFilterId);
                    }
                }

                Master.MessageCenter.DisplaySuccessMessage(ProductsStrings.GetText(@"MessageProductDeleted"));
            }
        }
    }
}
