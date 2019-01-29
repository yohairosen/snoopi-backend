using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using Snoopi.core.DAL;
using dg.Sql;
using System.Security.Cryptography;
using Snoopi.core.Caching.Manager;
using System.Net.Mail;

namespace Snoopi.core.BL
{
    public static class EmailLogController
    {
        public static Int64 GetItemCount()
        {
            return new Query(EmailLog.TableSchema).GetCount(EmailLog.Columns.EmailLogId);
        }
        public static EmailLogCollection GetLogItems(string OrderBy, SortDirection OrderDirection, int limit, int offset)
        {
            Query qry = new Query(EmailLog.TableSchema)
                .OrderBy(OrderBy, OrderDirection)
                .LimitRows(limit).OffsetRows(offset);
            return EmailLogCollection.FetchByQuery(qry);
        }
        public static EmailLog GetLogItem(Int64 emailLogId)
        {
            return EmailLog.FetchByID(emailLogId);
        }
        public static void DeleteLog(Int64 emailLogId)
        {
            DeleteLog(GetLogItem(emailLogId));
        }
        public static void DeleteLog(EmailLog item)
        {
            if (item == null) return;
            EmailLog.Delete(item.EmailLogId);
            item.EmailLogId = 0;
            item.MarkNew();
        }
        public static bool SaveLog(EmailLog item)
        {
            item.Save();
            if (item.EmailLogId > 0)
            {
                return true;
            }
            return false;
        }

        public static void LogEmail(MailMessage message, bool sent, DateTime deliveryDate, string exception)
        {
            EmailLog item = new EmailLog();
            item.Subject = message.Subject;
            item.Body = message.Body;
            item.FromEmail = message.From.Address ?? string.Empty;
            item.FromName = message.From.DisplayName ?? string.Empty;
            item.MailPriority = message.Priority;
            item.ToList = message.To.ToString();
            item.CcList = message.CC.ToString();
            item.BccList = message.Bcc.ToString();
            item.DeliveryDate = deliveryDate;
            item.Status = sent ? EmailLogStatus.Sent : EmailLogStatus.Failed;
            item.Exception = exception;

            if (message.Attachments.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"<div style=""direction:ltr;text-align:left;""><br />Found {0} attachments in this email:<br />", message.Attachments.Count);
                int idx = 0;
                foreach (Attachment att in message.Attachments)
                {
                    Mail.AttachmentEx attEx = att as Mail.AttachmentEx;
                    if (attEx != null)
                    {
                        item.Body = item.Body.Replace(@"""cid:" + attEx.ContentId + @"""", @"""" + attEx.AttachmentFileUrl.ToHtml() + @"""");
                        sb.AppendFormat(@"<a href=""{1}"">Attachment {0}</a><br/>", idx + 1, attEx.AttachmentFileUrl.ToHtml());
                    }
                    idx++;
                }
                sb.Append(@"</div>");
                idx = item.Body.LastIndexOf(@"</body>");
                if (idx > -1)
                {
                    item.Body = item.Body.Insert(idx, sb.ToString());
                }
                else
                {
                    item.Body = item.Body + sb.ToString();
                }
            }

            SaveLog(item);
        }

        public enum EmailLogType : int
        {
            None = 0,
            DoNotLog = 0,
            OnError = 1,
            OnSuccess = 2,
            Always = OnError | OnSuccess
        }
    }
}
