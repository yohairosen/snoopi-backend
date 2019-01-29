using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using Snoopi.web.Resources;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using Snoopi.core.BL;

namespace Snoopi.web
{
    public partial class OrderDetails : SupplierPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //Master.PageTitle = GlobalStrings.GetText(@"DefaultPageTitle");
            //ltDescription.Text = GlobalStrings.GetText(@"DefaultPageBody");
            if (!IsPostBack)
            {
                lblBidIdLabel.Text = SupplierProfileStrings.GetText("BidIdLabel", new System.Globalization.CultureInfo("he-IL"));
                lblOrderDateLabel.Text = SupplierProfileStrings.GetText("OrderDateLabel", new System.Globalization.CultureInfo("he-IL"));
                lblDealsIncludeLabel.Text = SupplierProfileStrings.GetText("DealsIncludeLabel", new System.Globalization.CultureInfo("he-IL"));
                lblPriceLabel.Text = SupplierProfileStrings.GetText("PriceLabel", new System.Globalization.CultureInfo("he-IL"));
                lblOrderIdlabel.Text = SupplierProfileStrings.GetText("OrderIdlabel", new System.Globalization.CultureInfo("he-IL"));
                LblUserLabel.Text = SupplierProfileStrings.GetText("AppUserLabel", new System.Globalization.CultureInfo("he-IL"));
                LblUserPhoneLabel.Text = SupplierProfileStrings.GetText("AppUserPhoneLabel", new System.Globalization.CultureInfo("he-IL"));
                Int64 OrderId= Request.QueryString["OrderId"]!= null ? Convert.ToInt64(Request.QueryString["OrderId"]) :0;
                OrderUI order = OrderController.GetOrderById(OrderId);
                if (order != null)
                {
                    //order.user.
                    lblOrderDate.Text = order.OrderDate.ToShortDateString() ;
                    lblOrderId.Text = order.OrderId.ToString();
                    lblPrice.Text = order.Price.ToString();
                    lblBidId.Text = order.BidId.ToString();
                    LblUser.Text = order.user.Email;
                    LblUserPhone.Text = order.user.Phone;
                    lvDealsInclude.DataSource = order.LstProduct;
                    lvDealsInclude.DataBind();
                }
                
            }
        }

    }
}
