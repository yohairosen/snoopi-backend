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
    public partial class MyBids : AdminPageBase
    {
        bool AllBids = false;
        bool HasEditPermission = false;
        string CustomerId;
        Int64 ci;
        CustomerType customerType;
        BidType bidType;
        bool BidLeave = false;
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
            if (Request.QueryString["id"] == "all")
            {
                if (Request.QueryString["bool"] == "bidleave")
                {
                    BidLeave = true;

                }

                AllBids = true;
            }
            if (Request.QueryString["Id"] != null && Request.QueryString["Id"] != "all")
            {
                CustomerId = Request.QueryString["Id"].ToString();
            }

            if (Request.QueryString["Type"] != null)
            {
                customerType = (CustomerType)int.Parse(Request.QueryString["Type"]);
            }

            if (Request.QueryString["BidType"] != null)
            {
                bidType = (BidType)int.Parse(Request.QueryString["BidType"]);
            }

            dgMyBids.PageIndexChanged += dgMyBids_PageIndexChanging;
        }

        protected void dgMyBids_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgMyBids.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgMyBids.Value = dgMyBids.CurrentPageIndex.ToString();
            LoadItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgMyBids.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgMyBids.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!AllBids)
            {
                CustomerUI customer = BidController.GetCustomerData(CustomerId, customerType);
                if (bidType == BidType.BidActive)
                {
                    Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"MyActiveBidsPageTitle") + " " + customer.CustomerName : BidString.GetText(@"CustomersTempPageTitle");
                    BidsCountLabel.Text = BidString.GetText(@"SumActiveBids");
                }
                if (bidType == BidType.BidCount)
                {
                    Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"MyCountBidsPageTitle") + " " + customer.CustomerName : BidString.GetText(@"CustomersTempPageTitle");
                    BidsCountLabel.Text = BidString.GetText(@"SumCountBids");
                }
                if (bidType == BidType.BidAbandoned)
                {
                    Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"MyBidsAbandonedPageTitle") + " " + customer.CustomerName : BidString.GetText(@"CustomersTempPageTitle");
                    BidsCountLabel.Text = BidString.GetText(@"SumAbandonedBids");
                }
                if (bidType == BidType.BidPurchase)
                {
                    Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"MyBidsPurchasePageTitle") + " " + customer.CustomerName : BidString.GetText(@"CustomersTempPageTitle");
                    BidsCountLabel.Text = BidString.GetText(@"SumPurchaseBids");
                }
            }
            else
            {

                if (bidType == BidType.BidActive)
                {
                    Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"DetailsMyActiveBidsPageTitle") : BidString.GetText(@"CustomersTempPageTitle");
                    BidsCountLabel.Text = BidString.GetText(@"SumActiveBids");
                }
                if (bidType == BidType.BidCount)
                {
                    Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"DetailsMyCountBidsPageTitle") : BidString.GetText(@"CustomersTempPageTitle");
                    BidsCountLabel.Text = BidString.GetText(@"SumCountBids");
                }
                if (bidType == BidType.BidAbandoned)
                {
                    if (BidLeave)
                    {
                        Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"DetailsMyWithNotOffersPageTitle") : BidString.GetText(@"CustomersTempPageTitle");
                        BidsCountLabel.Text = BidString.GetText(@"SumAbandonedBids");
                    }
                    else
                    {
                        Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"DetailsMyBidsAbandonedPageTitle") : BidString.GetText(@"CustomersTempPageTitle");
                        BidsCountLabel.Text = BidString.GetText(@"SumAbandonedBids");

                    }
                }
                if (bidType == BidType.BidPurchase)
                {
                    Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"DetailsMyBidsPurchasePageTitle") : BidString.GetText(@"CustomersTempPageTitle");
                    BidsCountLabel.Text = BidString.GetText(@"SumPurchaseBids");
                }
            }
            Master.ActiveMenu = "MyBids";

            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgMyBids.Columns[dgMyBids.Columns.Count - 1].Visible = false;
            }
            if (AllBids)
            {
                dgMyBids.VirtualItemCount = Convert.ToInt32(BidController.GetCountAllBidByCustomerTypeAndBidType(customerType, bidType, BidLeave));
            }
            else
            {
                dgMyBids.VirtualItemCount = Convert.ToInt32(BidController.GetCountBidByCustomerIdAndBidType(CustomerId, customerType, bidType));
            }
            if (dgMyBids.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = BidString.GetText(@"MessageNoBidsFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgMyBids.PageSize * dgMyBids.CurrentPageIndex > dgMyBids.VirtualItemCount)
                {
                    dgMyBids.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgMyBids.Value = dgMyBids.CurrentPageIndex.ToString();
                }
                if (AllBids)
                {
                    List<BidUI> coll = BidController.GetAllBidByCustomerTypeAndBidType(customerType, bidType, BidLeave, dgMyBids.PageSize, dgMyBids.CurrentPageIndex);
                    BindList(coll);
                }
                else
                {
                    List<BidUI> coll = BidController.GetAllBidByCustomerIdAndBidType(CustomerId, customerType, bidType, dgMyBids.PageSize, dgMyBids.CurrentPageIndex);
                    BindList(coll);
                }

            }



        }
        protected void BindList(List<BidUI> coll)
        {
            dgMyBids.DataSource = coll;
            dgMyBids.DataBind();
            if (bidType != BidType.BidCount || bidType != BidType.BidPurchase)
            {
                dgMyBids.Columns[5].Visible = false;
            }
            if(bidType == BidType.BidAbandoned)
                dgMyBids.Columns[7].Visible = false;
            Master.DisableViewState(dgMyBids);
            lblTotal.Text = dgMyBids.VirtualItemCount.ToString();
        }




        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"BidId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"StartDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"EndDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"OffersCount"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Products"), typeof(string)));
            if (bidType != BidType.BidCount || bidType != BidType.BidPurchase)
            {
                dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"OrderDate"), typeof(string)));
            }
            List<BidUI> coll;
            if (AllBids)
            {
                coll = BidController.GetAllBidByCustomerTypeAndBidType(customerType, bidType, BidLeave, 0, 0);
            }
            else
            {
                coll = BidController.GetAllBidByCustomerIdAndBidType(CustomerId, customerType, bidType, 0, 0);
            }
            foreach (BidUI Bid in coll)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = Bid.BidId;
                row[i++] = "\"" + Bid.StartDate + "\"";
                row[i++] = "\"" + Bid.EndDate + "\"";
                row[i++] = Bid.OfferNum;
                row[i++] = Bid.Products.Replace("<br>", ",");
                if (bidType != BidType.BidCount || bidType != BidType.BidPurchase)
                {
                    row[i++] = "\"" + Bid.OrderDate + "\"";
                }
                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=MyBidsExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
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
