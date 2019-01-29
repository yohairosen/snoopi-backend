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
    public partial class DeleteFilter : AdminPageBase
    {
        bool HasSystemPermission = false;
        Int64 FilterId;
        object FilterName;

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Int64.TryParse(Request.QueryString[@"FilterId"], out FilterId))
            {
                if (ProductController.IsFilterInUse(FilterId))
                {
                    Master.MessageCenter.DisplayErrorMessage(FiltersStrings.GetText(@"MessageDeleteFailedFilterInUse"));
                    pnlDelete.Visible = false;
                }
            }
            FilterName = new Query(Filter.TableSchema).Select(Filter.Columns.FilterName).Where(Filter.Columns.FilterId, FilterId).ExecuteScalar();

            if (FilterName == null)
            {
                Http.Respond404(true);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = string.Format(FiltersStrings.GetText(@"DeleteFilterPageTitle"), FilterName);
            Master.ActiveMenu = "Filters";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Query q = new Query(SubFilter.TableSchema).Where(SubFilter.Columns.FilterId, FilterId);
                q.Delete().Execute();                 

                Filter.Delete(FilterId);
                //string url = "Filters.aspx?message-success=" + FiltersStrings.GetText(@"MessageFilterDeleted");
                //Response.Redirect(url, true);
                Master.MessageCenter.DisplaySuccessMessage(FiltersStrings.GetText(@"MessageFilterDeleted"));
            }
        }
    }
}
