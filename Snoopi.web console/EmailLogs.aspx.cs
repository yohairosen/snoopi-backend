using System;
using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using dg.Utilities;
using dg.Utilities.Spreadsheet;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core;

namespace Snoopi.web
{
    public partial class EmailLogs : AdminPageBase
    {
        string action = string.Empty;

        protected override string[] AllowedPermissions
        {
            get { return new string[] { Permissions.PermissionKeys.sys_perm }; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            dgLogs.PageIndexChanged += dgLogs_PageIndexChanging;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgLogs.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgLogs.CurrentPageIndex = CurrentPageIndex;

            action = Request.QueryString[@"action"];
            if (action == @"preview")
            {
                Int64 EmailLogId = 0;
                Int64.TryParse(Request.QueryString[@"EmailLogId"], out EmailLogId);
                EmailLog logItem = EmailLogController.GetLogItem(EmailLogId);
                if (logItem != null)
                {
                    Response.Clear();
                    Response.Write(logItem.Body);
                    Response.End();
                    return;
                }
            }

            LoadItems();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = EmailTemplatesStrings.GetText(@"EmailLogsPageTitle");
            Master.ActiveMenu = "EmailLogs";

            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            dgLogs.VirtualItemCount = (int)EmailLogController.GetItemCount();
            if (dgLogs.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;

                int CurrentPageIndex = 0;
                if (!int.TryParse(hfCurrentPageIndex_dgLogs.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
                if (CurrentPageIndex < 0) CurrentPageIndex = 0;
                dgLogs.CurrentPageIndex = CurrentPageIndex;

                int limit = dgLogs.PageSize;
                int offset = dgLogs.CurrentPageIndex * dgLogs.PageSize;

                EmailLogCollection items = EmailLogController.GetLogItems(
                    EmailLog.Columns.DeliveryDate, dg.Sql.SortDirection.DESC,
                    limit, offset);

                BindList(items);
            }
        }
        protected void BindList(EmailLogCollection coll)
        {
            dgLogs.ItemDataBound += dgLogs_ItemDataBound;
            dgLogs.DataSource = coll;
            dgLogs.DataBind();
            Master.DisableViewState(dgLogs);
            lblTotal.Text = dgLogs.VirtualItemCount.ToString();
        }

        protected void dgLogs_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgLogs.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgLogs.Value = dgLogs.CurrentPageIndex.ToString();
            LoadItems();
        }
        protected void dgLogs_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || 
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                LinkButton lbDelete = e.Item.FindControl("lbDelete") as LinkButton;
                lbDelete.Visible = Permissions.UserHasAnyPermissionIn(SessionHelper.UserId(), "sys_edit_emails");
                if (lbDelete.Visible)
                {
                    lbDelete.Attributes.Add("onclick", @"return confirm('" + EmailTemplatesStrings.GetText("MessageConfirmDeleteLog").ToJavaScript('\'', false) + @"');return false;");
                }

                HyperLink hlPreview = e.Item.FindControl(@"hlPreview") as HyperLink;
                hlPreview.NavigateUrl = "EmailLogs.aspx?action=preview&EmailLogId=" + ((EmailLog)e.Item.DataItem).EmailLogId;
            }
        }

        protected string GetStatus(object status)
        {
            if (!(status is EmailLogStatus)) return string.Empty;
            switch ((EmailLogStatus)status)
            {
                case EmailLogStatus.Sent:
                    return EmailTemplatesStrings.GetText(@"LogStatus_Sent");
                case EmailLogStatus.Failed:
                    return EmailTemplatesStrings.GetText(@"LogStatus_Failed");
                default:
                case EmailLogStatus.Unknown:
                    return EmailTemplatesStrings.GetText(@"LogStatus_Unknown");
            }
        }
        protected string GetMailPriority(object mailPriority)
        {
            if (!(mailPriority is System.Net.Mail.MailPriority)) return string.Empty;
            switch ((System.Net.Mail.MailPriority)mailPriority)
            {
                case System.Net.Mail.MailPriority.Low:
                    return EmailTemplatesStrings.GetText(@"MailPriority_Low");
                case System.Net.Mail.MailPriority.High:
                    return EmailTemplatesStrings.GetText(@"MailPriority_High");
                default:
                case System.Net.Mail.MailPriority.Normal:
                    return EmailTemplatesStrings.GetText(@"MailPriority_Normal");
            }
        }
        protected void lbDelete_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName.Equals("doDelete"))
            {
                if (Permissions.UserHasAnyPermissionIn(SessionHelper.UserId(), "sys_edit_emails"))
                {
                    Int64 EmailLogId = Convert.ToInt64(e.CommandArgument);
                    EmailLogController.DeleteLog(EmailLogId);
                    Master.MessageCenter.DisplaySuccessMessage(EmailTemplatesStrings.GetText("MessageLogDeleted"));
                }
                else
                {
                    Master.MessageCenter.DisplayWarningMessage(GlobalStrings.GetText(@"NoPermissionsForAction"));
                }
            }
        }

        #region Excel
        protected void hlExportAll_Click(object sender, EventArgs e)
        {
            ExportToExcel(0, 0);
        }
        protected void hlExportCurrent_Click(object sender, EventArgs e)
        {
            ExportToExcel(dgLogs.PageSize, dgLogs.PageSize * dgLogs.CurrentPageIndex);
        }
        protected void ExportToExcel(int limit, int offset)
        {
            EmailLogCollection coll = EmailLogController.GetLogItems(EmailLog.Columns.DeliveryDate, dg.Sql.SortDirection.DESC, limit, offset);
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("LogNumber"), typeof(Int64)));
            dt.Columns.Add(new System.Data.DataColumn(GlobalStrings.GetText("Date"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(GlobalStrings.GetText("Status"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("Subject"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("From"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("Recipient"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("CC"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("BCC"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("Priority"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("Content"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(EmailTemplatesStrings.GetText("Exception"), typeof(string)));

            foreach (EmailLog item in coll)
            {
                System.Data.DataRow row = dt.NewRow();
                row[0] = item.EmailLogId;
                row[1] = item.DeliveryDate.ToString(@"dd/MM/yyyy HH:mm:ss");
                row[2] = GetStatus(item.Status);
                row[3] = item.Subject;
                row[4] = item.FromEmail + (string.IsNullOrEmpty(item.FromName) ? string.Empty : @" - " + item.FromName);
                row[5] = item.ToList;
                row[6] = item.CcList;
                row[7] = item.BccList;
                row[8] = GetMailPriority(item.MailPriority);
                row[9] = item.Body.Replace(@"<br />", "\r\n").StripHtml();
                row[10] = item.Exception;
                dt.Rows.Add(row);
            }

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, true, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=EmailLogsExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
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
