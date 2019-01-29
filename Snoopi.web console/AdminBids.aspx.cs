using Snoopi.core.DAL;
using Snoopi.core.BL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Snoopi.core;

namespace Snoopi.web
{
    public partial class AdminBids : AdminPageBase
    {
        bool HasEditPermission = false;
        string filterSearch = null;
        List<BidUI> au = new List<BidUI>();
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
            if (Request.QueryString["Filter"] != null)
            {
                filterSearch = Request.QueryString["Filter"];
            }
            dgBids.PageIndexChanged += dgBids_PageIndexChanging;

        }

        protected void dgBids_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgBids.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgBids.Value = dgBids.CurrentPageIndex.ToString();
            LoadItems(true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgBids.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgBids.CurrentPageIndex = CurrentPageIndex;
            if (IsPostBack)
            {
                au = (List<BidUI>) ViewState["au"];
                LoadItems(false);
            }
            else LoadItems(true);
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (filterSearch != null)
            {
                linksSearch.Visible = false;
                BidsCountLabel.Visible = true;
            }
            else
            {
                BidsCountLabel.Visible = false;
                linksSearch.Visible = true;
                Master.PageTitle = BidString.GetText(@"AdminRejectTitle");
            }
            Master.ActiveMenu = "UntakedBids";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems(bool fromDb)
        {
            if (!HasEditPermission)
            {
                dgBids.Columns[dgBids.Columns.Count - 1].Visible = false;
            }
            DateTime from = new DateTime(), to = new DateTime();
            DateTime.TryParse(dpSearchCreateDateFrom.Value.ToString(), out from);
            DateTime.TryParse(dpSearchCreateDateTo.Value.ToString(), out to);
            Int64 _BidId = -1;
            if (txtBidNumber.Text != "")
                Int64.TryParse(txtBidNumber.Text, out _BidId);
            if (fromDb){
          //    au = BidController.GetUntakenBidsTempForCampaign(filterSearch, from, to, txtSearchPhone.Text, txtCustomerId.Text, _BidId, txtCityName.Text);
                au = BidController.GetUntakenBids(filterSearch, from, to, txtSearchPhone.Text, txtCustomerId.Text, _BidId, txtCityName.Text);

                ViewState["au"] = au;
            }
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
                    //lblSumAbandonedBidsWithOffers.Text = au.Count(b => b.OfferNum > 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now).ToString();
                    //lblSumAbandonedBidsWithOutOffers.Text = au.Count(b => b.OfferNum == 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now).ToString();
                    //lblSumActiveBids.Text = au.Count(b => b.EndDate >= DateTime.Now).ToString();
                    //lblSumPurchaseBids.Text = au.Count(b => b.OrderDate != (DateTime?)null).ToString();
                }
                else
                {
                    dgBids.VirtualItemCount = au.Count();
                }
        //        List<BidUI> app_users = BidController.GetUntakenBids(filterSearch, from, to, txtSearchPhone.Text, txtCustomerId.Text, _BidId, txtCityName.Text, dgBids.PageSize, dgBids.CurrentPageIndex);
                BindList(au);
            }



        }

        protected void approveDeal(object sender, EventArgs e)
        {
            Button btn = (Button)(sender);
            var arguments = btn.CommandArgument;
            int bidId;

            Int32.TryParse(arguments, out bidId);
            if (!saveDeal(bidId))
            {
                Master.MessageCenter.DisplayErrorMessage(BidString.GetText(@"ErrorMakeDeal"));
                //LoadItems(false);
            }
            else
            {
                Master.MessageCenter.DisplaySuccessMessage(BidString.GetText(@"SuccessMakeDeal"));
                LoadItems(true);
            }
        }

        protected void rejectDeal(object sender, EventArgs e)
        {
            Button btn = (Button)(sender);
            var arguments = btn.CommandArgument;
            int bidId;

            Int32.TryParse(arguments, out bidId);
            if (!saveRejectDeal(bidId))
            {
                Master.MessageCenter.DisplayErrorMessage(BidString.GetText(@"ErrorCancelBid"));
                //LoadItems(false);
            }
            else
            {
                Master.MessageCenter.DisplaySuccessMessage(BidString.GetText(@"SuccessCancelBid"));
                LoadItems(true);
            }
        }

        protected void resendDeal(object sender, EventArgs e)
        {
            Button btn = (Button)(sender);
            var arguments = btn.CommandArgument;
            //int bidId;

           // Int32.TryParse(arguments, out bidId);


            Response.Redirect(@"ResendDeal.aspx?bidId=" + arguments.ToString(), true);
            
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

        bool saveRejectDeal(Int64 bidId)
        {
            var order = Order.FetchByBidId(bidId);
            if (order == null || order.AppUserId <= 0)
                return false;
            Notification.SendNotificationAppUserAdminRejected(order.AppUserId, bidId);
            order.SupplierId = 0;
            order.Save();

            var bid = Bid.FetchByID(bidId);
            bid.IsActive = false;
            bid.Save();

            return true;
        }

       bool saveDeal(Int64 bidId)
        {
            var order = Order.FetchByBidId(bidId);
            if (order != null && order.SupplierId > 0)
                return false;

            decimal TotalPrice = order.TotalPrice;
            var discount = BidController.GetDiscountForUser(TotalPrice, order.AppUserId);
            var supplier = AppSupplier.FetchByID(312);
            string response = "";
           
            decimal PriceAfterDiscount = Convert.ToDecimal(discount["PriceAfterDiscount"]);
            decimal PrecentDiscount = Convert.ToDecimal(discount["PrecentDiscount"]);
            Int64? CampaignId = Convert.ToInt64(discount["CampaignId"]);
            var paymentDetails = new PaymentDetails
            {
                Amount = (float)PriceAfterDiscount * 100,
                CreditId = order.Transaction,
                Exp = order.ExpiryDate,
                AuthNumber = order.AuthNumber,
                NumOfPayments = order.NumOfPayments,
                SupplierToken = supplier.MastercardCode
            };
            try
            {
                response = CreditGuardManager.CreateMPITransaction(paymentDetails);
            }
            catch
            {
                Notification.SendNotificationAppUserCreditRejected(order.AppUserId, bidId);
                return false;
            }
            if (response != "000")
            {
                Notification.SendNotificationAppUserCreditRejected(order.AppUserId, bidId);
                return false;
            }
            order.IsSendRecived = false;
            if (CampaignId != 0)
                order.CampaignId = CampaignId;
            order.TotalPrice = TotalPrice;
            order.PriceAfterDiscount = PriceAfterDiscount;
            order.PrecentDiscount = PrecentDiscount;
            // order.SpecialInstructions = special_instructions;
            order.UserPaySupplierStatus = UserPaymentStatus.Payed;
            order.SupplierId = 312;
            order.Save();
            var bid = Bid.FetchByID(bidId);
            bid.IsActive = false;
            bid.Save();
            Notification.SendNotificationAppUserSupplierApproved(Snoopi.web.Localization.PushStrings.GetText("SupplierApproved"), bid.AppUserId.Value, order.OrderId);
            return true;       
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems(false);
        }

    }
}