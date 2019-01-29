using System;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core;
using dg.Utilities;
using Snoopi.core.DAL;
using System.Collections.Generic;
using dg.Sql;
using dg.Sql.Connector;
using System.Web.UI;
using System.Web;
using System.Globalization;
using System.Threading.Tasks;

namespace Snoopi.web
{
    public partial class EditMessage : AdminPageBase
    {
        Int64 MessageId;

        bool IsNewMode = false;
        protected override void VerifyAccessToThisPage()
        {
            //string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            //if (!permissions.Contains(Permissions.PermissionKeys.sys_edit_users))
            //{
            //    Master.LimitAccessToPage();
            //}
            IsNewMode = Request.QueryString[@"New"] != null;

            if (!IsNewMode)
            {
                if (!Int64.TryParse(Request.QueryString[@"ID"], out MessageId))
                {
                    MessageId = 0;
                }
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(MessagesStrings.GetText(@"NewMessageButton"), @"EditMessage.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
                       
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //fill isSendReceived dropdown
                if (ddlSendTo.Items.Count == 0)
                {
                    ddlSendTo.Items.Add(new ListItem(OrdersStrings.GetText(@"Suppliers"), "Suppliers"));
                    ddlSendTo.Items.Add(new ListItem(OrdersStrings.GetText(@"AppUsers"), "AppUsers"));
                }
                LoadView();
            }
            else
            {
                if (hfOriginalMessageId.Value.Length > 0 && hfOriginalMessageId.Value != MessageId.ToString())
                {
                    Http.Respond404(true);
                }
            }
        }
        protected void LoadView()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["message-success"]))
            {
                pnlEditMessage.Visible = false;
                //MessageUI Message = MessagesController.GetMessageById(MessageId);
                //txtAddress.Text = Message.Address;
                //txtLongitude.Text = Message.Longitude;
                //txtLatitude.Text = Message.Latitude;
            }
            else
            {
                pnlEditMessage.Visible = true;
            }


        }
      
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = MessagesStrings.GetText(IsNewMode ? @"NewMessagePageTitle" : @"EditMessagePageTitle");
            Master.ActiveMenu = IsNewMode ? "EditMessage" : "Messages";

        }

        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            MessageUI MessageUI = new MessageUI();
            MessageUI.Description = txtDescription.Text;            

            if (IsNewMode)
            {
                List<string> SendTo = new List<string>();
                foreach (ListItem item in ddlSendTo.Items)
                {
                    if (item.Selected)
                    {
                        SendTo.Add(item.Value);
                    }
                }

                MessageId = MessagesController.CreateNewMessage(MessageUI);

                if ((SendTo.Count == 1 && SendTo[0] == "Suppliers") || SendTo.Count == 0 || SendTo.Count == 2)
                {
                    SupplierNotification.SendNotificationNewMessage(MessageUI.Description);
                }
                if ((SendTo.Count == 1 && SendTo[0] == "AppUsers") || SendTo.Count == 0 || SendTo.Count == 2)
                {
                  //  int results = FcmService.SendFcmNotification(MessageUI.Description);                  
                    // Notification.SendNotificationNewMessageToAllDevices(MessageUI.Description);
                    //var result =  FcmService.SendFcmNotification(MessageUI.Description);
                }
            }

            if (IsNewMode)
            {
                string successMessage = MessagesStrings.GetText(@"MessageMessageCreated");
                string url = @"EditMessage.aspx?ID=" + MessageId.ToString();
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);
            }
            else
            {
                string successMessage = MessagesStrings.GetText(@"MessageMessageSaved");
                
                    string url = @"EditMessage.aspx?message-success=" + Server.UrlEncode(successMessage);
                    url += @"&ID=" + MessageId.ToString();
                    Response.Redirect(url, true);
                
            }
        }

}
}
