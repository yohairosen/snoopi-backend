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
    public partial class DeleteSubCategory : AdminPageBase
    {
        bool HasSystemPermission = false;
        Int64 SubCategoryId;
        object SubCategoryName;

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Int64.TryParse(Request.QueryString[@"SubCategoryId"], out SubCategoryId))
            {
                if (ProductController.IsSubCategoryInUse(SubCategoryId))
                {
                    Master.MessageCenter.DisplayErrorMessage(CategoriesStrings.GetText(@"MessageDeleteFailedCategoryInUse"));
                    pnlDelete.Visible = false;
                }
            }
            SubCategoryName = new Query(SubCategory.TableSchema).Select(SubCategory.Columns.SubCategoryName).Where(SubCategory.Columns.SubCategoryId, SubCategoryId).ExecuteScalar();

            if (SubCategoryName == null)
            {
                Http.Respond404(true);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = string.Format(CategoriesStrings.GetText(@"DeleteCategoryPageTitle"), SubCategoryName);
            Master.ActiveMenu = "SubCategories";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                
                SubCategory.Delete(SubCategoryId);
                string url = "SubCategories.aspx?message-success=" + CategoriesStrings.GetText(@"MessageSubCategoryDeleted");
                Response.Redirect(url, true);
            }
        }
    }
}
