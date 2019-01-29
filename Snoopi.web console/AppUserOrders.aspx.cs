using System;
using System.Collections;
using System.Configuration;
using dg.Utilities;
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
using dg.Utilities.Spreadsheet;
using System.Linq;

namespace Snoopi.web
{
    public partial class AppUserOrders : AdminPageBase
    {
        bool HasEditPermission = true;
        Int64 AppUserId = 0;
        string AppUserEmail;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            dgOrders.PageIndexChanged += dgOrders_PageIndexChanging;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["AppUserId"] != null)
            {
                AppUserId = Int64.Parse(Request["AppUserId"].ToString());
                AppUser appUser = AppUser.FetchByID(AppUserId);
                if (appUser == null)
                {
                    Http.Respond404(true);
                }
                AppUserEmail = appUser.Email;
            }

            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgOrders.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgOrders.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = string.Format(OrdersStrings.GetText(@"AppUserOrdersPageTitle"), AppUserEmail); ;
            Master.ActiveMenu = "AppUsers";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems(bool IsSearch = false)
        {
            if (!HasEditPermission)
            {
                dgOrders.Columns[dgOrders.Columns.Count - 1].Visible = false;
            }
            string SearchBid = "";
            DateTime from = new DateTime();
            DateTime to = new DateTime();

            if (IsSearch)
            {
                DateTime.TryParse(datepickerFrom.Value.ToString(), out from);
                DateTime.TryParse(datepickerTo.Value.ToString(), out to);
                SearchBid = "%" + txtSearchBid.Text.Trim() + "%";
                //if (from != DateTime.MinValue && to != DateTime.MinValue)
                //{
                //    if (from > to)
                //    {
                //        Master.MessageCenter.DisplayErrorMessage(OrdersStrings.GetText(@"EnterValidDates"));
                //        return;
                //    }
                //}
            }
            dgOrders.VirtualItemCount = OrderController.GetAllAppUserOrders(AppUserId, from, to, SearchBid).Count;
            if (dgOrders.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = OrdersStrings.GetText(@"MessageNoOrdersFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgOrders.PageSize * dgOrders.CurrentPageIndex > dgOrders.VirtualItemCount)
                {
                    dgOrders.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgOrders.Value = dgOrders.CurrentPageIndex.ToString();
                }
                List<OrderUI> items = OrderController.GetAllAppUserOrders(AppUserId, from, to, SearchBid, dgOrders.PageSize, dgOrders.CurrentPageIndex);
                BindList(items);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            hfIsSearchActive.Value = true.ToString();
            dgOrders.CurrentPageIndex = 0;
            hfCurrentPageIndex_dgOrders.Value = dgOrders.CurrentPageIndex.ToString();
            LoadItems(true);
        }

        protected void BindList(List<OrderUI> coll)
        {
            dgOrders.DataSource = coll;
            dgOrders.DataBind();
            //Master.DisableViewState(dgOrders);
            lblTotal.Text = dgOrders.VirtualItemCount.ToString();
        }

        protected void dgOrders_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgOrders.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgOrders.Value = dgOrders.CurrentPageIndex.ToString();
            LoadItems();
        }


        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"BidId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"BidEndDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"LstProducts"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"Gift"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"DonationPrice"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"TotalPrice"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"PrecentDiscount"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"PriceAfterDiscount"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"CampaignName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"IsSendReceived"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"OrderDate1"), typeof(string)));

            DateTime from, to = new DateTime();
            DateTime.TryParse(datepickerFrom.Value.ToString(), out from);
            DateTime.TryParse(datepickerTo.Value.ToString(), out to);
            string SearchBid = "%" + txtSearchBid.Text.Trim() + "%";
            List<OrderUI> orders = OrderController.GetAllAppUserOrders(AppUserId, from, to, SearchBid);

            foreach (OrderUI order in orders)
            {
                System.Data.DataRow row = dt.NewRow();
                row[0] = order.BidId;
                row[1] = "\"" + order.BidEndDate + "\"";
                row[2] = (order.LstProduct.Count == 0 || order.LstProduct == null) ? "" : String.Join(", ", order.LstProduct.Select(p => p.Amount + " " + p.ProductName));
                row[3] = order.Gift;
                row[4] = order.DonationPrice;
                row[5] = order.TotalPrice;
                row[6] = order.PrecentDiscount + "%";
                row[7] = order.PriceAfterDiscount;
                row[8] = order.CampaignName;
                row[9] = GlobalStrings.GetYesNo(order.IsSendReceived);
                row[10] = "\"" + order.OrderDate + "\"";
                dt.Rows.Add(row);
            }
            System.Data.DataRow sumRow = dt.NewRow();
            dt.Rows.Add(sumRow);

            System.Data.DataRow sumRowTotalPrice = dt.NewRow();
            sumRowTotalPrice[0] = OrdersStrings.GetText(@"SumTotalPrice") + " " + orders.Sum(o => o.TotalPrice);
            dt.Rows.Add(sumRowTotalPrice);

            System.Data.DataRow sumRowPriceAfterDiscount = dt.NewRow();
            sumRowPriceAfterDiscount[0] = OrdersStrings.GetText(@"SumPriceAfterDiscount") + " " + orders.Sum(o => o.PriceAfterDiscount);
            dt.Rows.Add(sumRowPriceAfterDiscount);


            System.Data.DataRow sumRowSumDonation = dt.NewRow();
            sumRowSumDonation[0] = OrdersStrings.GetText(@"SumDonation") + " " + orders.Sum(o => o.DonationPrice);
            dt.Rows.Add(sumRowSumDonation);
            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=AppUserOrdersExport_" + "AppUserId:" + AppUserId + "_Date:" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
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
