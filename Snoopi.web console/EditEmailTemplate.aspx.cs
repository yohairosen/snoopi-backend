using System;
using System.Collections;
using System.Configuration;
using Snoopi.core.DAL;
using System.Web.UI.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using dg.Utilities;
using Snoopi.web.WebControls;

namespace Snoopi.web
{
    public partial class EditEmailTemplate : AdminPageBase
    {
        Int64 EmailTemplateId = -1;
        EmailTemplate thisItem = null;

        protected override string[] AllowedPermissions
        {
            get { return new string[] { Permissions.PermissionKeys.sys_perm }; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(EmailTemplatesStrings.GetText(@"NewTemplateButton"), @"EditEmailTemplate.aspx", new string[] { Permissions.PermissionKeys.sys_perm });

            ddlMailPriority.Items.Add(new ListItem(EmailTemplatesStrings.GetText(@"PriorityLow"), @"Low"));
            ddlMailPriority.Items.Add(new ListItem(EmailTemplatesStrings.GetText(@"PriorityNormal"), @"Normal"));
            ddlMailPriority.Items.Add(new ListItem(EmailTemplatesStrings.GetText(@"PriorityHigh"), @"High"));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ckBody.FullPage = true;
            Master.SetDirectionByCurrentLang(ckBody);

            Int64.TryParse(Request.QueryString["EmailTemplateId"], out EmailTemplateId);

            if (EmailTemplateId > 0)
            {
                thisItem = EmailTemplateController.GetItem(EmailTemplateId);
                if (thisItem == null) Http.Respond404();
            }
            else thisItem = new EmailTemplate();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (thisItem.IsNewRecord)
            {
                Master.PageTitle = EmailTemplatesStrings.GetText(@"TitleNewTemplate");
                Master.ActiveMenu = "NewEmailTemplate";
                pnlEdit.GroupingText = EmailTemplatesStrings.GetText(@"PanelNewTemplate");
            }
            else
            {
                Master.PageTitle = string.Format(EmailTemplatesStrings.GetText(@"TitleEditTemplate"), thisItem.Name);
                Master.ActiveMenu = "EmailTemplates";
                pnlEdit.GroupingText = EmailTemplatesStrings.GetText(@"PanelEditTemplate");
            }

            if (thisItem != null)
            {
                txtName.Text = thisItem.Name;
                ckBody.Text = thisItem.Body;
                txtSubject.Text = thisItem.Subject;
                txtTo.Text = thisItem.ToList;
                txtCc.Text = thisItem.CcList;
                txtBcc.Text = thisItem.BccList;
                txtFromEmail.Text = thisItem.FromEmail;
                txtFromName.Text = thisItem.FromName;
                txtReplyToEmail.Text = thisItem.ReplyToEmail;
                txtReplyToName.Text = thisItem.ReplyToName;
                ddlMailPriority.SelectedValue = thisItem.MailPriority.ToString();
                switch (thisItem.MailPriority)
                {
                    case System.Net.Mail.MailPriority.Low:
                        ddlMailPriority.SelectedValue = @"Low";
                        break;
                    case System.Net.Mail.MailPriority.Normal:
                        ddlMailPriority.SelectedValue = @"Normal";
                        break;
                    case System.Net.Mail.MailPriority.High:
                        ddlMailPriority.SelectedValue = @"High";
                        break;
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                thisItem.Name = txtName.Text;
                thisItem.Body = ckBody.Text;
                thisItem.Subject = txtSubject.Text;
                thisItem.ToList = txtTo.Text;
                thisItem.CcList = txtCc.Text;
                thisItem.BccList = txtBcc.Text;
                thisItem.FromEmail = txtFromEmail.Text;
                thisItem.FromName = txtFromName.Text;
                thisItem.ReplyToEmail = txtReplyToEmail.Text;
                thisItem.ReplyToName = txtReplyToName.Text;
                switch (ddlMailPriority.SelectedValue)
                {
                    case @"Low":
                        thisItem.MailPriority = System.Net.Mail.MailPriority.Low;
                        break;
                    case @"Normal":
                        thisItem.MailPriority = System.Net.Mail.MailPriority.Normal;
                        break;
                    case @"High":
                        thisItem.MailPriority = System.Net.Mail.MailPriority.High;
                        break;
                }

                bool isNewRecord = thisItem.IsNewRecord;
                if (EmailTemplateController.Save(thisItem))
                {
                    if (isNewRecord)
                    {
                        Response.Redirect(string.Format(@"EditEmailTemplate.aspx?message-success={0}", Server.UrlEncode(EmailTemplatesStrings.GetText(@"CreatedMessage"))), true);
                    }
                    else
                    {
                        Master.MessageCenter.DisplaySuccessMessage(EmailTemplatesStrings.GetText(@"SavedMessage"));
                    }
                }
                else
                {
                    Master.MessageCenter.DisplayErrorMessage(EmailTemplatesStrings.GetText(@"NotSavedMessage"));
                }
            }
        }
    }
}
