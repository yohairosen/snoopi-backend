using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using Snoopi.core.DAL;
using dg.Sql;
using System.Security.Cryptography;
using Snoopi.core.Caching.Manager;
using System.Web.Caching;
using System.Net.Mail;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Snoopi.core.BL
{
    public static class EmailTemplateController
    {
        private static string CACHE_KEY_BASE = @"EMAIL_TMP_";
        private static string CACHE_KEY_BY_ID = CACHE_KEY_BASE + @"{0}";

        public static string DOCTYPE_XHTML_1_TRANS_EN = @"html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""";
        public static string HTMLTAGS_OFFICE_SUPPORT = @"xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:w=""urn:schemas-microsoft-com:office:word"" xmlns:m=""http://schemas.microsoft.com/office/2004/12/omml"" xmlns=""http://www.w3.org/TR/REC-html40""";

        /* settings */
        private static string[] allowedDocExts = new string[] { 
            @".doc", 
            @".docx", 
            @".pdf", 
            @".rtf", 
            @".txt" 
        };
        private static string[] allowedDocContentTypes = new string[] { 
            @"application/msword", 
            @"application/vnd.openxmlformats-officedocument.wordprocessingml.document", 
            @"application/pdf", 
            @"application/msword", 
            @"text/plain" 
        };

        public static void ClearCache_All()
        {
            CacheHelper.RemoveAllCacheObjectStartWith(CACHE_KEY_BASE);
        }
        public static void ClearCache_Item(Int64 EmailTemplateId)
        {
            CacheHelper.RemoveCacheObject<EmailTemplate>(string.Format(CACHE_KEY_BY_ID, EmailTemplateId));
        }

        public static EmailTemplate GetItem(Int64 EmailTemplateId)
        {
            return CacheHelper.CacheObject<EmailTemplate>(
                delegate
                {
                    return EmailTemplate.FetchByID(EmailTemplateId);
                },
                string.Format(CACHE_KEY_BY_ID, EmailTemplateId), CacheHelper.DefaultCacheTime, CacheItemPriority.BelowNormal);
        }
        public static void Delete(Int64 EmailTemplateId)
        {
            Delete(GetItem(EmailTemplateId));
        }
        public static void Delete(EmailTemplate item)
        {
            if (item == null) return;
            EmailTemplate.Delete(item.EmailTemplateId);
            ClearCache_Item(item.EmailTemplateId);
            item.EmailTemplateId = 0;
            item.MarkNew();
        }
        public static bool Save(EmailTemplate item)
        {
            item.Save();
            if (item.EmailTemplateId > 0)
            {
                ClearCache_Item(item.EmailTemplateId);
                return true;
            }
            return false;
        }

        public static Int64 GetItemCount()
        {
            return new Query(EmailTemplate.TableSchema).GetCount(EmailTemplate.Columns.EmailTemplateId);
        }
        public static EmailTemplateCollection GetItems(string OrderBy, SortDirection OrderDirection, int Limit, int Offset)
        {
            Query qry = new Query(EmailTemplate.TableSchema)
                .OrderBy(OrderBy, OrderDirection)
                .LimitRows(Limit).OffsetRows(Offset);
            return EmailTemplateCollection.FetchByQuery(qry);
        }
        public static string GetItemName(Int64 EmailTemplateId)
        {
            object ret = new Query(EmailTemplate.TableSchema)
                .Select(EmailTemplate.Columns.Name)
                .Where(EmailTemplate.Columns.EmailTemplateId, EmailTemplateId)
                .LimitRows(1)
                .ExecuteScalar();
            return (string)ret;
        }

        /************************************************************************/
        /* Sending emails by regular code                                       */
        /************************************************************************/
        public static MailMessage BuildMailMessage(
            string fromEmail, string fromName,
            string replyToEmail, string replyToName,
            string to, string cc, string bcc,
            string subject, string body,
            string[] attachmentPaths,
            MailPriority? mailPriority, List<string> lstTo = null)
        {
    
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromEmail, fromName);
            if (!string.IsNullOrEmpty(replyToEmail))
            {
                message.ReplyToList.Add(new MailAddress(replyToEmail, replyToName));
            }
            if (lstTo != null)
            {
                foreach (string item in lstTo)
                {
                    Mail.AddAddress(item, message.Bcc);
                }
            }
            else
            {
                Mail.AddAddress(to, message.To);
            }
            if (!string.IsNullOrEmpty(cc))
            {
                Mail.AddAddress(cc, message.CC);
            }
            if (!string.IsNullOrEmpty(bcc))
            {
                Mail.AddAddress(bcc, message.Bcc);
            }
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Priority = mailPriority ?? MailPriority.Normal;
            message.IsBodyHtml = true;
            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.UTF8;

            // Create attachments
            if ((attachmentPaths != null) && (attachmentPaths.Length > 0))
            {
                foreach (string attachmentPath in attachmentPaths)
                {
                    // MailAttachment likes fully-qualified file names, use FileInfo to 
                    // get them.
                    FileInfo fileInfo = new FileInfo(attachmentPath);

                    if (fileInfo.Exists)
                    {
                        message.Attachments.Add(new Attachment(fileInfo.FullName));
                    }
                    else
                    {
                        Trace.TraceError("Attachment not found: {0}", fileInfo.FullName);
                    }
                }
            }
            return message;
        }
        public static SmtpClient CreateSmtpClient()
        {
            SmtpClient smtpClient = new SmtpClient();
            string HostName = Settings.GetSetting(Settings.Keys.MailSettings.MAIL_SERVER_HOSTNAME);
            if (!string.IsNullOrEmpty(HostName))
            {
                smtpClient.Host = HostName;
                smtpClient.Port = Settings.GetSettingInt32(Settings.Keys.MailSettings.MAIL_SERVER_PORT, 25);
            }

            if (Settings.GetSettingBool(Settings.Keys.MailSettings.MAIL_SERVER_AUTHENTICAION, false))
            {
                smtpClient.Credentials = new NetworkCredential(Settings.GetSetting(Settings.Keys.MailSettings.MAIL_SERVER_USERNAME), Settings.GetSetting(Settings.Keys.MailSettings.MAIL_SERVER_PASSWORD));
            }
            if (Settings.GetSettingBool(Settings.Keys.MailSettings.MAIL_SERVER_SSL, false))
            {
                smtpClient.EnableSsl = true;
            }
            return smtpClient;
        }
        public static bool Send(MailMessage objMail, EmailLogController.EmailLogType logEmail, bool rethrowErrors, int timeOutInSeconds = 0)
        {
            try
            {
                string smtpUserName = Settings.GetSetting(Settings.Keys.SMTP_USER_NAME);
                string smtpPassword = Settings.GetSetting(Settings.Keys.SMTP_PASSWORD);

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = credentials;
                //  submitBtn.Style.Add("display", "none");
                if (timeOutInSeconds > 0)
                    smtpClient.Timeout = timeOutInSeconds * 1000;
 
                smtpClient.Send(objMail);

                if ((logEmail & EmailLogController.EmailLogType.OnSuccess) == EmailLogController.EmailLogType.OnSuccess)
                {
                    try
                    {
                        EmailLogController.LogEmail(objMail, true, DateTime.UtcNow, null);
                    }
                    catch (System.Exception ex)
                    {
                        Trace.TraceError(ex.ToString());

                        if (rethrowErrors)
                        {
                            throw ex;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());

                if ((logEmail & EmailLogController.EmailLogType.OnError) == EmailLogController.EmailLogType.OnError)
                {
                    EmailLogController.LogEmail(objMail, false, DateTime.UtcNow, ex.ToString());
                }

                if (rethrowErrors)
                {
                    throw ex;
                }
            }
            return false;
        }
        /************************************************************************/

        /************************************************************************/
        public static string SetHtmlFullPage(string htmlPage, bool fullPage, string docType, string htmlTags, string baseHref)
        {
            if (fullPage)
            {
                StringBuilder sb = new StringBuilder();
                int idxHtml = htmlPage.IndexOf(@"<html");
                if (idxHtml < 0)
                {
                    if (docType != null && docType.Length > 0)
                    {
                        sb.Append(docType);
                    }
                    if (htmlTags != null && htmlTags.Length > 0)
                    {
                        sb.AppendFormat(@"<html {0}>", htmlTags);
                    }
                    else sb.Append(@"<html>");
                    if (baseHref != null && baseHref.Length > 0)
                    {
                        sb.AppendFormat(@"<head><base href=""{0}"" /></head>", baseHref.ToHtml());
                    }
                    sb.Append(@"<body>");
                    sb.Append(htmlPage);
                    sb.Append(@"</body></html>");
                }
                else
                {
                    int idxNext = idxHtml;
                    if (docType != null)
                    {
                        if (docType.Length > 0) sb.AppendFormat(@"<!DOCTYPE {0}>", docType);
                        int idxDocType = htmlPage.IndexOf(@"<!DOCTYPE");
                        if (idxDocType > -1) idxDocType = htmlPage.IndexOf(@">", idxDocType);
                        if (idxDocType > -1) idxNext = idxDocType + 1;
                    }
                    if (idxNext < idxHtml) sb.Append(htmlPage.Substring(idxNext, idxHtml - idxNext));

                    idxNext = htmlPage.IndexOf(@">", idxHtml) + 1;
                    if (htmlTags != null && htmlTags.Length > 0)
                    {
                        sb.Append(htmlPage.Substring(idxHtml, idxNext - idxHtml - 1));
                        sb.Append(' ');
                        sb.Append(htmlTags);
                        sb.Append('>');
                    }
                    else
                    {
                        sb.Append(htmlPage.Substring(idxHtml, idxNext - idxHtml));
                    }
                    if (baseHref != null)
                    {
                        bool addNewHead = baseHref.Length > 0;

                        // Find the HEAD and BASE
                        int idxHead = htmlPage.IndexOf(@"<head", idxNext);
                        int idxHeadEnd = idxHead > -1 ? htmlPage.IndexOf(@"</head>", idxHead) : -1;
                        int idxBody = htmlPage.IndexOf(@"<body", idxNext);
                        int idxBase = idxHead > -1 ? htmlPage.IndexOf(@"<base", idxHead) : -1;
                        if (idxHead > idxBody) idxHead = idxBase = -1;
                        if (idxHeadEnd == -1 || idxHeadEnd < idxBase) idxBase = idxHead = -1;

                        if (idxBase > -1)
                        {
                            addNewHead = false;

                            int idxBaseEnd1 = htmlPage.IndexOf(@"</base>", idxBase);
                            if (idxBaseEnd1 > -1) idxBaseEnd1 += 7;
                            int idxBaseEnd2 = htmlPage.IndexOf(@">", idxBase);
                            if (idxBaseEnd2 > -1) idxBaseEnd2++;
                            if (idxBaseEnd1 == -1 || (idxBaseEnd2 > -1 && idxBaseEnd2 < idxBaseEnd1))
                                idxBaseEnd1 = idxBaseEnd2;

                            sb.Append(htmlPage.Substring(idxNext, idxBase - idxNext));
                            if (baseHref.Length > 0)
                            {
                                sb.Append(string.Format(@"<base href=""{0}"" />", baseHref.ToHtml()));
                            }
                            if (idxBaseEnd1 > -1) idxNext = idxBaseEnd1;
                        }

                        if (addNewHead)
                        {
                            // No head and we need one, make a new one
                            sb.AppendFormat(@"<head><base href=""{0}"" /></head>", baseHref.ToHtml());
                        }
                    }
                    // Put in the rest of the html page
                    if (htmlPage.Length > idxNext) sb.Append(htmlPage.Substring(idxNext));
                }
                return sb.ToString();
            }
            return SetBaseHref(htmlPage, baseHref);
        }
        public static string SetBaseHref(string htmlPage, string href)
        {
            int idxHead = htmlPage.IndexOf(@"<head>");
            int idxBody = htmlPage.IndexOf(@"<body>");
            int idxBase = idxHead > -1 ? htmlPage.IndexOf(@"<base", idxHead) : -1;
            if (idxHead > idxBody) idxHead = idxBase = -1;

            if (idxBase > -1)
            {
                int idxBaseBeg = htmlPage.IndexOf(@"href=""", idxBase);
                if (idxBaseBeg > -1)
                {
                    idxBaseBeg += 6;
                    int idxBaseEnd = htmlPage.IndexOf(@"""", idxBaseBeg);
                    StringBuilder sb = new StringBuilder(htmlPage);
                    sb.Remove(idxBaseBeg, idxBaseEnd - idxBaseBeg);
                    sb.Insert(idxBaseBeg, href.ToHtml());
                    return sb.ToString();
                }
            }
            if (idxHead > -1)
            {
                StringBuilder sb = new StringBuilder(htmlPage);
                sb.Insert(idxHead + 6, string.Format(@"<base href=""{0}"" />", href.ToHtml()));
                return sb.ToString();
            }
            if (idxBody > -1)
            {
                StringBuilder sb = new StringBuilder(htmlPage);
                sb.Insert(idxBody, string.Format(@"<head><base href=""{0}"" /></head>", href.ToHtml()));
                return sb.ToString();
            }
            return htmlPage;
        }
        /************************************************************************/

        public static string ReplaceSharpsInString(string text, Dictionary<string, string> dictFieldHtml)
        {
            if (text == null || text.Length == 0) return text;

            // sharp replacing
            StringBuilder sb = new StringBuilder();
            int firstSharp = -1;
            bool sharpOk = false;
            char c;
            for (int j = 0; j < text.Length; j++)
            {
                c = text[j];
                if (c == '#')
                {
                    if (firstSharp == -1)
                    {
                        firstSharp = j;
                    }
                    else if (firstSharp == j - 1)
                    { // Convert ## to #, to allow escaping those #
                        firstSharp = -1;
                        sb.Append(c);
                    }
                    else
                    {
                        sharpOk = false;
                        if (j - firstSharp > 1)
                        {
                            string val;
                            dictFieldHtml.TryGetValue(text.Substring(firstSharp + 1, j - firstSharp - 1), out val);
                            sb.Append(val);
                            sharpOk = true;
                        }
                        if (!sharpOk)
                        {
                            sb.Append(text.Substring(firstSharp, j - firstSharp + 1));
                        }
                        firstSharp = -1;
                    }
                }
                else if (firstSharp == -1)
                {
                    sb.Append(c);
                }
            }
            if (firstSharp > -1)
            {
                sb.Append(text.Substring(firstSharp));
            }
            text = sb.ToString();
            sb = null;
            // end of sharp replacing
            return text;
        }
    }
}
