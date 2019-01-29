using dg.Utilities;
using dg.Utilities.Spreadsheet;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Snoopi.web
{
    public partial class Bids : AdminPageBase
    {
        bool HasEditPermission = false;
        string filterSearch = null;
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
            if (Request.QueryString["Filter"] != null)
            {
                filterSearch = Request.QueryString["Filter"];
            }
            dgBids.PageIndexChanged += dgBids_PageIndexChanging;
            dgBids.ItemDataBound += DgBids_ItemDataBound; ;
        }

        private void DgBids_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item != null && (e.Item.ItemType == ListItemType.Item  ||
                e.Item.ItemType == ListItemType.AlternatingItem))
            {
                DataGridItem item = e.Item;
                BidUI bidUI = (BidUI)item.DataItem;
                if (bidUI.SupplierId<=0 && bidUI.IsActive==true)
                {
                    if (item.FindControl("btnCancelBid") != null)
                        ((Button)item.FindControl("btnCancelBid")).Visible = true;
                }
                else
                    if (item.FindControl("btnDeleteBid") != null)
                        ((Button)item.FindControl("btnDeleteBid")).Visible = true;
            }
        }
   
         protected void dgBids_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgBids.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgBids.Value = dgBids.CurrentPageIndex.ToString();
            LoadItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgBids.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgBids.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (filterSearch != null)
            {
                linksSearch.Visible = false;
                BidsCountLabel.Visible = true;
                if (filterSearch == "ActiveBids")
                {
                    Master.PageTitle = BidString.GetText(@"DetailsMyActiveBidsPageTitle");
                   // BidsCountLabel.Text = BidString.GetText(@"SumActiveBids");
                }
                if (filterSearch == "BidsWithOffers")
                {
                    Master.PageTitle = BidString.GetText(@"DetailsMyCountBidsPageTitle") ;
                   // BidsCountLabel.Text = BidString.GetText(@"SumCountBids");
                }
                if (filterSearch == "BidsWithOutOffers")
                {
                    Master.PageTitle = BidString.GetText(@"DetailsMyWithNotOffersPageTitle");
                }
                if (filterSearch == "PurchaseBids")
                {
                    Master.PageTitle =  BidString.GetText(@"DetailsMyBidsPurchasePageTitle");
                    //BidsCountLabel.Text = BidString.GetText(@"SumPurchaseBids");
                }
            }
            else
            {
                BidsCountLabel.Visible = false;
                linksSearch.Visible = true;
                Master.PageTitle = BidString.GetText(@"BidsPageTitle");
            }
            Master.ActiveMenu = "Bids";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgBids.Columns[dgBids.Columns.Count - 1].Visible = false;
            }
            DateTime from = new DateTime(), to = new DateTime();
            DateTime.TryParse(dpSearchCreateDateFrom.Value.ToString(), out from);
            DateTime.TryParse(dpSearchCreateDateTo.Value.ToString(), out to);
            Int64 _BidId = -1;
            if(txtBidNumber.Text != "")
                Int64.TryParse(txtBidNumber.Text, out _BidId);
            List<BidUI> au = BidController.GetAllBids(filterSearch,from, to, txtSearchPhone.Text, txtCustomerId.Text, _BidId, txtCityName.Text, 0, 0, true);
            dgBids.VirtualItemCount = au.Count;
            if (dgBids.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = BidString.GetText(@"MessageNoBidsFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgBids.PageSize * dgBids.CurrentPageIndex > dgBids.VirtualItemCount)
                {
                    dgBids.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgBids.Value = dgBids.CurrentPageIndex.ToString();
                }
                if (filterSearch == null)
                {
                    lblSumAbandonedBidsWithOffers.Text = au.Count(b => b.OfferNum > 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now).ToString();
                    lblSumAbandonedBidsWithOutOffers.Text = au.Count(b => b.OfferNum == 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now).ToString();
                    lblSumActiveBids.Text = au.Count(b => b.EndDate >= DateTime.Now).ToString();
                    lblSumPurchaseBids.Text = au.Count(b => b.OrderDate != (DateTime?)null).ToString();
                }
                else 
                {
                    dgBids.VirtualItemCount = au.Count();
                }
                List<BidUI> app_users = BidController.GetAllBids(filterSearch, from, to, txtSearchPhone.Text, txtCustomerId.Text, _BidId, txtCityName.Text, dgBids.PageSize, dgBids.CurrentPageIndex, true);
                BindList(app_users);
            }



        }
        protected void BindList(List<BidUI> coll)
        {
            dgBids.DataSource = coll;
            dgBids.DataBind();
            Master.DisableViewState(dgBids);
            lblTotal.Text = dgBids.VirtualItemCount.ToString();
            if (filterSearch != null)
            {
                lblTotal1.Text = dgBids.VirtualItemCount.ToString();
            }
        }

        protected void btnDeletedBid_Click(object sender, EventArgs e)
        {
            Button btn = (Button)(sender);
            var arguments = btn.CommandArgument;
            long bidId;

            long.TryParse(arguments, out bidId);

            if (!deleteBid(bidId))
            {
                Master.MessageCenter.DisplayErrorMessage(BidString.GetText(@"DeleteBidError"));
                //LoadItems(false);
            }
            else
            {
                Master.MessageCenter.DisplaySuccessMessage(BidString.GetText(@"DeleteBidSuccess"));
                LoadItems();
            }
            Page.Response.Redirect(Page.Request.Url.AbsoluteUri);
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
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"CustomerId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"CustomerName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Phone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"CityName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"OffersCount"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"SupplierName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Price"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Products"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"OrderStatus"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"IsActive"), typeof(string)));


            DateTime from = new DateTime(), to = new DateTime();
            DateTime.TryParse(dpSearchCreateDateFrom.Value.ToString(), out from);
            DateTime.TryParse(dpSearchCreateDateTo.Value.ToString(), out to);
            Int64 _BidId = -1;
            if (txtBidNumber.Text != "")
                Int64.TryParse(txtBidNumber.Text, out _BidId);
            List<BidUI> app_users = BidController.GetAllBids(filterSearch,from, to, txtSearchPhone.Text, txtCustomerId.Text, _BidId, txtCityName.Text);

            foreach (BidUI Bid in app_users)
            {
                int i= 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = Bid.BidId;
                row[i++] = "\"" +Bid.StartDate+ "\"";
                row[i++] = "\"" +Bid.EndDate+ "\"";
                row[i++] = Bid.CustomerId;
                row[i++] = Bid.CustomerType == CustomerType.Temp ? BidString.GetText("Temp"):  Bid.CustomerName;
                row[i++] = Bid.Phone;
                row[i++] = Bid.City;
                row[i++] = Bid.OfferNum;
                row[i++] = Bid.SupplierName;
                row[i++] = Bid.Price;
                row[i++] = Bid.Products;
                row[i++] = Bid.OrderStatus== OrderDeliveryStatus.None?"": BidString.GetText(Bid.OrderStatus.ToString()).Replace("XXX",Bid.SuppliedDate.ToString());
                row[i++] = GlobalStrings.GetYesNo(Bid.IsActive);
                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=BidsExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }


        protected void btnExportForCRM_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"CustomerId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"FirstName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"LastName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"Email"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Phone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"Address"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"CityName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"CreateDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"BidId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Products"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Price"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"OrderDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"ActionType"), typeof(string)));



            DateTime from = new DateTime(), to = new DateTime();
            DateTime.TryParse(dpSearchCreateDateFrom.Value.ToString(), out from);
            DateTime.TryParse(dpSearchCreateDateTo.Value.ToString(), out to);
            Int64 _BidId = -1;
            if (txtBidNumber.Text != "")
                Int64.TryParse(txtBidNumber.Text, out _BidId);
            List<BidUI> app_users = BidController.GetAllBids(filterSearch, from, to, txtSearchPhone.Text, txtCustomerId.Text, _BidId, txtCityName.Text);

            foreach (BidUI Bid in app_users)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                var appUser = AppUser.FetchByID(Bid.CustomerId);
                row[i++] = Bid.CustomerId;
                row[i++] = Bid.CustomerType == CustomerType.Temp ? BidString.GetText("Temp") : appUser.FirstName;
                row[i++] = Bid.CustomerType == CustomerType.Temp ? BidString.GetText("Temp") : appUser.LastName;
                row[i++] = Bid.CustomerType == CustomerType.Temp ? BidString.GetText("Temp") : appUser.Email;
                row[i++] = Bid.CustomerType == CustomerType.Temp ? BidString.GetText("Temp") : "=\"" + appUser.Phone + "\"";
                row[i++] = Bid.CustomerType == CustomerType.Temp ? BidString.GetText("Temp") : appUser.Street +" " + appUser.HouseNum;
                row[i++] = Bid.City;
                row[i++] = Bid.CustomerType == CustomerType.Temp ? BidString.GetText("Temp") : appUser.CreateDate.ToString();
                row[i++] = Bid.BidId;
                row[i++] = Bid.Products;
                row[i++] = Bid.Price;
                row[i++] = Bid.OrderDate;
                row[i++] = "רכישה";
                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=BidsExportForCRM_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }

        #endregion
        protected void btnCancelBid_Click(object sender, EventArgs e)
        {
            Button btn = (Button)(sender);
            var arguments = btn.CommandArgument;
            long bidId;

            long.TryParse(arguments, out bidId);

            if (!cancelBid(bidId))
            {
                Master.MessageCenter.DisplayErrorMessage(BidString.GetText(@"ErrorCancelBid"));
                //LoadItems(false);
            }
            else
            {
                Master.MessageCenter.DisplaySuccessMessage(BidString.GetText(@"SuccessCancelBid"));
                LoadItems();
            }
            Page.Response.Redirect(Page.Request.Url.AbsoluteUri);
        }

        private bool cancelBid(long bidId)
        {
            var bid = Bid.FetchByID(bidId);
            if (bid == null || bid.AppUserId <= 0)
                return false;

            Order order = Order.FetchByBidId(bidId);
            if (order.SupplierId > 0)
                return false;

            bid.IsActive = false;
            bid.Save();

            List<BidMessage> bidMessage = BIdMessageController.GetAllActiveMessagesByBidId(bidId);
            if (bidMessage.Count > 0)
            {
                foreach (BidMessage message in bidMessage)
                {
                    if (message.IsActive == true)
                    {
                        message.IsActive = false;
                        message.Save();
                    }
                }
            }
            return true;
        }


        private bool deleteBid(long bidId)
        {
            var bid = Bid.FetchByID(bidId);
            if (bid == null || bid.AppUserId <= 0)
                return false;
            bid.IsActive = false;
            bid.Deleted = DateTime.Now;
            bid.Save();       
            return true;
        }

 
    }

}
