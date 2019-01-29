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
    public partial class Offers : AdminPageBase
    {
        bool HasEditPermission = false;
        Int64 BidId;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            dgOffers.PageIndexChanged += dgOffers_PageIndexChanging;

            if (!Int64.TryParse(Request.QueryString["Id"] , out BidId))
            {
                BidId = 0;
            }

        }

        protected void dgOffers_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgOffers.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgOffers.Value = dgOffers.CurrentPageIndex.ToString();
            LoadItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgOffers.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgOffers.CurrentPageIndex = CurrentPageIndex;
            if (!IsPostBack)
            {
                Bid b = Bid.FetchByID(BidId);
                if (b.AppUserId != null)
                {
                    AppUser a = AppUser.FetchByID(b.AppUserId);
                    Master.PageTitle = String.Format(BidString.GetText(@"OffersTitle"), BidId.ToString(), a.FirstName + " " + a.LastName);
                }
                else if (b.TempAppUserId != null)
                {
                    Master.PageTitle = String.Format(BidString.GetText(@"OffersTitle"), BidId.ToString(), BidString.GetText("Temp"));
                }
            }
            LoadItems();
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
           
            Master.ActiveMenu = "Offers";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {          
            
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgOffers.PageSize * dgOffers.CurrentPageIndex > dgOffers.VirtualItemCount)
                {
                    dgOffers.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgOffers.Value = dgOffers.CurrentPageIndex.ToString();
                }
                List<OfferUI> coll = OfferController.GetAllOfferByBidIdWithIsOrder(BidId);
                
                BindList(coll);
        }
        protected void BindList(List<OfferUI> coll)
        {
            dgOffers.DataSource = coll;
            dgOffers.DataBind();
            Master.DisableViewState(dgOffers);
            lblTotal.Text = coll.Count.ToString();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"SupplierName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Price"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Gift"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"IsOrder"), typeof(string)));


            List<OfferUI> coll = OfferController.GetAllOfferByBidIdWithIsOrder(BidId);
            foreach (OfferUI offer in coll)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = offer.SupplierName;
                row[i++] = offer.TotalPrice;
                row[i++] = offer.Gift;
                row[i++] =  GlobalStrings.GetYesNo(offer.IsOrder);
                dt.Rows.Add(row);
            }
            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=OffersExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }
        #endregion

    }

}
