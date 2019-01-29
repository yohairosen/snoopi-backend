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
    public partial class DeleteCategory : AdminPageBase
    {
        bool HasSystemPermission = false;
        Int64 CategoryId;
        object CategoryName;

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Int64.TryParse(Request.QueryString[@"CategoryId"], out CategoryId))
            {
                if (ProductController.IsCategoryInUse(CategoryId))
                {
                    Master.MessageCenter.DisplayErrorMessage(CategoriesStrings.GetText(@"MessageDeleteFailedCategoryInUse"));
                    pnlDelete.Visible = false;
                }
            }
            CategoryName = new Query(Category.TableSchema).Select(Category.Columns.CategoryName).Where(Category.Columns.CategoryId, CategoryId).ExecuteScalar();

            if (CategoryName == null)
            {
                Http.Respond404(true);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = string.Format(CategoriesStrings.GetText(@"DeleteCategoryPageTitle"), CategoryName);
            Master.ActiveMenu = "Categories";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SubCategoryCollection coll= ProductController.GetAllSubCategory(CategoryId);
                if (coll.Count>0)
                {
                    foreach (SubCategory subCategory in coll)
                    {
                        SubCategory.Delete(subCategory.SubCategoryId);
                    }
                }

                Category.Delete(CategoryId);
                string url = "Categories.aspx?message-success=" + CategoriesStrings.GetText(@"MessageCategoryDeleted");
                Response.Redirect(url, true);
            }
        }
    }
}
