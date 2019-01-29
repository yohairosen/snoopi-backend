using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ResendDeal : AdminPageBase

{
    bool HasEditPermission = false;

    protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);



        if (ddlSuppliers.Items.Count == 0)
        {
            List<AppSupplier> suppliers = AppSupplierCollection.FetchAll();
            foreach (var item in suppliers)
            {
                ddlSuppliers.Items.Add(new ListItem(item.BusinessName, item.SupplierId.ToString()));
            }

        }
        ddlSuppliers.DataBind();

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = BidString.GetText(@"ResendDealTitle");
        Master.ActiveMenu = "UntakedBids";

    }

    protected void bt_resend_Click(object sender, EventArgs e)
    {
        try
        {

        string arguments = Request.QueryString["bidId"];
        long bidId;

        long.TryParse(arguments, out bidId);

        List<BidMessage> bidMessages = BIdMessageController.GetAllMessagesByBidId(bidId);
        foreach (BidMessage message in bidMessages)
        {
            message.IsActive = false;
            message.Save();
        }

            BIdMessageController.AddNewMessage(bidId, long.Parse(ddlSuppliers.SelectedValue), bidMessages[0].OriginalSupplierId);

            Bid bid = Bid.FetchByID(bidId);
            bid.IsActive = true;
            bid.EndDate = DateTime.UtcNow.AddHours(Convert.ToDouble(Settings.GetSettingDecimal(Settings.Keys.END_BID_TIME_MIN, 15)));
            bid.Save();
        Master.MessageCenter.DisplaySuccessMessage(BidString.GetText(@"ResendDealSucceed").Replace("XXX", ddlSuppliers.SelectedItem.Text));
      

    }

        catch (Exception)
        {
            Master.MessageCenter.DisplayErrorMessage(BidString.GetText(@"ResendDealFail"));

            throw;
        }
        

             
       
    }

    protected void bt_back_to_admin_bids_Click(object sender, EventArgs e)
    {
        Response.Redirect(@"AdminBids.aspx", true);
    }
}