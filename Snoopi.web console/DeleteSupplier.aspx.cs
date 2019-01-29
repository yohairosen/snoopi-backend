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
    public partial class DeleteSupplier : AdminPageBase
    {
        bool HasSystemPermission = false;
        Int64 SupplierId;
        object SupplierName = null;

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);

            if (Int64.TryParse(Request.QueryString[@"SupplierId"], out SupplierId))
            {
                AppSupplier supplier = core.DAL.AppSupplier.FetchByID(SupplierId);
                if (supplier == null)
                {
                    Master.LimitAccessToPage();
                }
                else
                {
                    SupplierName = supplier.Email;
                }
            }
            else
            {
                Master.LimitAccessToPage();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SupplierName == null)
            {
                Http.Respond404(true);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = string.Format(SuppliersStrings.GetText(@"DeleteSupplierPageTitle"), SupplierName);
            Master.ActiveMenu = "Suppliers";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Query.New<AppSupplier>().Where(AppSupplier.Columns.SupplierId, SupplierId)
                    .Update(AppSupplier.Columns.IsDeleted, true)
                    .Update(AppSupplier.Columns.UniqueIdString, null)
                    .Execute();

                Master.MessageCenter.DisplaySuccessMessage(SuppliersStrings.GetText(@"MessageSupplierDeleted"));
                lblDeleteConfirm.Visible = false;
                chkDeleteConfirm.Visible = false;
                btnDelete.Visible = false;
            }
        }
    }
}
