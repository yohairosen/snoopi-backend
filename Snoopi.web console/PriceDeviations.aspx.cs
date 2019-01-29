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
    public partial class PriceDeviations : AdminPageBase
    {
        bool HasSystemPermission = false;
        Int64 SupplierId;
        string SupplierName;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            dgPriceDeviations.PageIndexChanged += dgPriceDeviations_PageIndexChanging;      
        }

        //protected override void VerifyAccessToThisPage()
        //{
        //    string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
        //    HasSystemPermission = permissions.Contains(Permissions.PermissionKeys.sys_perm);
           
        //    if (Int64.TryParse(Request.QueryString[@"SupplierId"], out SupplierId))
        //    {
        //        AppSupplier supplier = core.DAL.AppSupplier.FetchByID(SupplierId);
        //        if (supplier == null)
        //        {
        //            Master.LimitAccessToPage();
        //        }
        //        else
        //        {
        //            SupplierName = supplier.Email;
        //        }
        //    }
        //    else
        //    {
        //        Master.LimitAccessToPage();
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgPriceDeviations.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgPriceDeviations.CurrentPageIndex = CurrentPageIndex;
            //if (!IsPostBack)
            //{
            //    Bid b = Bid.FetchByID(BidId);
            //    if (b.AppUserId != null)
            //    {
            //        AppUser a = AppUser.FetchByID(b.AppUserId);
            //        Master.PageTitle = String.Format(BidString.GetText(@"PriceDeviationsTitle"), BidId.ToString(), a.FirstName + " " + a.LastName);
            //    }
            //    else if (b.TempAppUserId != null)
            //    {
            //        Master.PageTitle = String.Format(BidString.GetText(@"PriceDeviationsTitle"), BidId.ToString(), BidString.GetText("Temp"));
            //    }
            //}
            LoadItems();
        }

        protected void approveDeviation(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)(sender);
            var arguments = btn.CommandArgument.Split(',');
            int supplierId ,productId;
            Int32.TryParse(arguments[0], out supplierId);
            Int32.TryParse(arguments[1], out productId);
            var deviation = PriceDeviation.FetchByID(supplierId, productId);
            deviation.IsApproved = true;
            deviation.TimeOfApproval = DateTime.Now;
            deviation.Save();
            Master.PageTitleHtml = string.Format(SuppliersStrings.GetText(@"DeleteSupplierPageTitle"), SupplierName);    
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
           
            Master.ActiveMenu = "PriceDeviations";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void dgPriceDeviations_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgPriceDeviations.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgPriceDeviations.Value = dgPriceDeviations.CurrentPageIndex.ToString();
            LoadItems();
        }

        protected void LoadItems()
        {               
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgPriceDeviations.PageSize * dgPriceDeviations.CurrentPageIndex > dgPriceDeviations.VirtualItemCount)
                {
                    dgPriceDeviations.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgPriceDeviations.Value = dgPriceDeviations.CurrentPageIndex.ToString();
                }
                List<PriceDeviationUi> coll = PriceDeviationController.GetUnapprovedPriceDeviation();
                
                BindList(coll);
        }
        protected void BindList(List<PriceDeviationUi> coll)
        {
            dgPriceDeviations.DataSource = coll;
            dgPriceDeviations.DataBind();
            Master.DisableViewState(dgPriceDeviations);
            lblTotal.Text = coll.Count.ToString();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

    }

}
