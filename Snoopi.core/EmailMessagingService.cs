using System;
using System.Collections.Generic;
using dg.Utilities;
using System.Text;
using Snoopi.core.DAL;
using Snoopi.core.BL;
using System.Web;
using System.Linq;
using dg.Sql;
namespace Snoopi.core
{
    static public class EmailMessagingService
    {
        static public Int32 GetEmailTemplateIdFromSettingKey(string Key, string LangCode)
        {
            int id = Settings.GetSettingInt32(Key + @"_" + LangCode, 0);
            if (id == 0)
            {
                if (LangCode.Length == 5) // xx-XX
                {
                    id = Settings.GetSettingInt32(Key + @"_" + LangCode.Substring(0, 2), 0);
                    if (id == 0)
                    {
                        id = Settings.GetSettingInt32(Key + @"_" + AppConfig.GetString(@"AvailableLanguages", @"").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim().Split(':')[0], 0);
                    }
                }
                else
                {
                    string[] AvailableLanguages = AppConfig.GetString(@"AvailableLanguages", @"").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string lang in AvailableLanguages)
                    {
                        if (lang.Trim().StartsWith(LangCode))
                        {
                            id = Settings.GetSettingInt32(Key + @"_" + lang.Trim().Split(':')[0], 0);
                            if (id > 0) break; ;
                        }
                    }
                }
            }
            return id;
        }

        static public void SendPasswordRecoveryMailForAppUser(AppUser user, string RecoveryKey, string LangCode)
        {
            string Key = Settings.Keys.EMAIL_TEMPLATE_APPUSER_FORGOT_PASSWORD;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, string.IsNullOrEmpty(LangCode) ? user.LangCode : LangCode);
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                string toList = template.ToList + @";" + user.Email;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                if (string.IsNullOrEmpty(replyToName)) replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"USERFIRSTNAME", user.FirstName);
                dictFieldHtml.Add(@"USERLASTNAME", user.LastName);
                dictFieldHtml.Add(@"USERFULLNAME", (user.FirstName + @" " + user.LastName).Trim());
                dictFieldHtml.Add(@"USEREMAIL", user.Email);
                dictFieldHtml.Add(@"PASSWORDKEY", HttpUtility.UrlEncode(RecoveryKey));

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                foreach (string key in dictFieldHtml.Keys.ToList())
                {
                    dictFieldHtml[key] = dictFieldHtml[key].ToHtml().Replace("\n", @"<br />");
                }
                string body = EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);

                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName,
                    toList, template.CcList, template.BccList, subject, body, null, template.MailPriority);
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
            }
        }

        static public void SendMailNoOffersToAdmin(AppUserUI user,DateTime StartBidDate,List<BidProductUI> products, string subject, string body)
        {
           
                string fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                string fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                string replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                string replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);
                string toList = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"{NAME}", (user.FirstName + @" " + user.LastName).Trim());
                dictFieldHtml.Add(@"{PHONE}", user.Phone);
                dictFieldHtml.Add(@"{CITY}", user.CityName);
                string str="";
                foreach (BidProductUI item in products)
	            {
		            str += item.Amount + " "+item.ProductName+@" <br /> ";
	            }
                dictFieldHtml.Add(@"{PRODUCTS}", str);
                dictFieldHtml.Add(@"{DATE}",StartBidDate.ToShortDateString());
                dictFieldHtml.Add(@"{TIME}", StartBidDate.ToShortTimeString());


                foreach (string key in dictFieldHtml.Keys.ToList())
                {
                    dictFieldHtml[key] = dictFieldHtml[key].ToHtml().Replace("\n", @"<br />");
                }
                body = EmailTemplateController.ReplaceSharpsInString(body, dictFieldHtml);

                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName, toList, null, null, subject, body, null, System.Net.Mail.MailPriority.Normal);
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
            
        }

        static public void SendWelcomeMailForAppUser(AppUser user, string LangCode)
        {
            string Key = Settings.Keys.EMAIL_TEMPLATE_NEW_APPUSER_WELCOME;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, string.IsNullOrEmpty(LangCode) ? user.LangCode : LangCode);
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                string toList = template.ToList + @";" + user.Email;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                if (string.IsNullOrEmpty(replyToName)) replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"USERFIRSTNAME", user.FirstName);
                dictFieldHtml.Add(@"USERLASTNAME", user.LastName);
                dictFieldHtml.Add(@"USERFULLNAME", (user.FirstName + @" " + user.LastName).Trim());
                dictFieldHtml.Add(@"USEREMAIL", user.Email);

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                foreach (string key in dictFieldHtml.Keys)
                {
                    dictFieldHtml[key] = dictFieldHtml[key].ToHtml().Replace("\n", @"<br />");
                }
                string body = EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);

                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName,
                    toList, template.CcList, template.BccList, subject, body, null, template.MailPriority);
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
            }
        }
        static public void SendWelcomeMailWithVerificationForAppUser(AppUser user, string VerifyKey, string LangCode)
        {
            string Key = Settings.Keys.EMAIL_TEMPLATE_NEW_APPUSER_WELCOME_VERIFY_EMAIL;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, string.IsNullOrEmpty(LangCode) ? user.LangCode : LangCode);
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                string toList = template.ToList + @";" + user.Email;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                if (string.IsNullOrEmpty(replyToName)) replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"USERFIRSTNAME", user.FirstName);
                dictFieldHtml.Add(@"USERLASTNAME", user.LastName);
                dictFieldHtml.Add(@"USERFULLNAME", (user.FirstName + @" " + user.LastName).Trim());
                dictFieldHtml.Add(@"USEREMAIL", user.Email);
                dictFieldHtml.Add(@"PASSWORDKEY", VerifyKey);

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                foreach (string key in dictFieldHtml.Keys)
                {
                    dictFieldHtml[key] = dictFieldHtml[key].ToHtml().Replace("\n", @"<br />");
                }
                string body = EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);

                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName,
                    toList, template.CcList, template.BccList, subject, body, null, template.MailPriority);
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
            }
        }


        static public void SendWelcomeMailWithVerificationForAppSupplier(AppSupplier user, string VerifyKey, string LangCode)
        {
            string Key = Settings.Keys.EMAIL_TEMPLATE_NEW_APPUSER_WELCOME_VERIFY_EMAIL;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, string.IsNullOrEmpty(LangCode) ? user.LangCode : LangCode);
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                string toList = template.ToList + @";" + user.Email;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                if (string.IsNullOrEmpty(replyToName)) replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"USERFULLNAME", user.ContactName.Trim());
                dictFieldHtml.Add(@"USEREMAIL", user.Email);
                dictFieldHtml.Add(@"PASSWORDKEY", VerifyKey);

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                foreach (string key in dictFieldHtml.Keys)
                {
                    dictFieldHtml[key] = dictFieldHtml[key].ToHtml().Replace("\n", @"<br />");
                }
                string body = EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);

                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName,
                    toList, template.CcList, template.BccList, subject, body, null, template.MailPriority);
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
            }
        }

        static public void SendNewBidToSupplier(BidMessage msg)
        {
            string Key = Settings.Keys.EMAIL_TEMPLATE_SUPPLIER_NEW_BID;
            AppSupplier supplier = SupplierUI.FetchByID(msg.SupplierId);
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, supplier.LangCode);
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                if (string.IsNullOrEmpty(replyToName)) replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"BIDID", msg.BidId.ToString());

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                string body =  EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);

                bool isProduction = Convert.ToBoolean(AppConfig.GetString(@"IsProduction", @"false"));
                string emailTo = isProduction ? supplier.Email:AppConfig.GetString(@"DevMailAddress", @"");
                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName,
                   emailTo, template.CcList, template.BccList, subject, body, null, template.MailPriority);
                
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true, 5);
            }
        }

        static public void SendPasswordRecoveryMailForUser(User user, string RecoveryKey, string LangCode)
        {
            UserProfile profile = UserProfile.FetchByID(user.UserId);

            string Key = Settings.Keys.EMAIL_TEMPLATE_USER_FORGOT_PASSWORD;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, string.IsNullOrEmpty(LangCode) ? profile.DefaultLangCode : LangCode);
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                string toList = template.ToList + @";" + user.Email;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                if (string.IsNullOrEmpty(replyToName)) replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"USERFIRSTNAME", profile.FirstName);
                dictFieldHtml.Add(@"USERLASTNAME", profile.LastName);
                dictFieldHtml.Add(@"USERFULLNAME", (profile.FirstName + @" " + profile.LastName).Trim());
                dictFieldHtml.Add(@"USEREMAIL", user.Email);
                dictFieldHtml.Add(@"PASSWORDKEY", System.Net.WebUtility.HtmlEncode(RecoveryKey));

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                //foreach (string key in dictFieldHtml.Keys)
                //{
                //    dictFieldHtml[key] = dictFieldHtml[key].ToHtml().Replace("\n", @"<br />");
                //}
                string body = EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);

                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName,
                    toList, template.CcList, template.BccList, subject, body, null, template.MailPriority);
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
            }
        }


        static public void SendPasswordRecoveryMailForSupplier(AppSupplier user, string RecoveryKey, string LangCode = "he-IL")
        {

            string Key = Settings.Keys.EMAIL_TEMPLATE_SUPPLIER_FORGOT_PASSWORD;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, LangCode);
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                string toList = template.ToList + @";" + user.Email;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                if (string.IsNullOrEmpty(replyToName)) replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"USERFIRSTNAME", user.ContactName);
                dictFieldHtml.Add(@"USEREMAIL", user.Email);
                dictFieldHtml.Add(@"PASSWORDKEY", System.Net.WebUtility.HtmlEncode(RecoveryKey));

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                //foreach (string key in dictFieldHtml.Keys)
                //{
                //    dictFieldHtml[key] = dictFieldHtml[key].ToHtml().Replace("\n", @"<br />");
                //}
                string body = EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);

                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName,
                    toList, template.CcList, template.BccList, subject, body, null, template.MailPriority);
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
            }
        }

        static public void SendEmailNewProductToSupplier(Product product)
        {

            string Key = Settings.Keys.EMAIL_NEW_PRODUCT;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, "he-IL");
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"PRODUCTNAME", product.ProductName);
                dictFieldHtml.Add(@"PRODUCTDESCIPTION", product.Description);
                dictFieldHtml.Add(@"PRODUCTCODE", product.ProductCode);

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                string body = EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);
               
                string to = "";
                Query q = Query.New<AppSupplier>().Where(AppSupplier.Columns.IsDeleted, false);
                AppSupplierCollection col = AppSupplierCollection.FetchByQuery(q);
                List<string> lstTo = null;
                if (col != null)
	            {
                    lstTo = col.Where(r=> r.Email != null).Select(r=> r.Email).ToList<string>();	                
                }
                if (to != "" || lstTo != null)
                {    
                    System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                        fromEmail, fromName, replyToEmail, replyToName,
                        to, template.CcList, template.BccList , subject, body, null, template.MailPriority, lstTo);
                    EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
                }
            }
        }

        static public void SendEmailUntakenBidToAdmin(BidMessage msg)
        {

            string Key = Settings.Keys.EMAIL_UNTAKEN_BID;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, "he-IL");
            string fromEmail =Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
            string fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);;
            string replyToEmail = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL);
            string replyToName = "Admin";

            string subject = "Transaction number " + msg.BidId + " has not completed!";
            string body =  "Nobody responded to BidId: " + msg.BidId;
            string to = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL);
            System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                        fromEmail, fromName, replyToEmail, replyToName,
                        to, "horizonb1@gmail.com", "", subject, body, null, null, new List<string>{to});
              EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);         
        }


        static public void SendGiftToAdmin(Int64 AppUserId, Int64 CampaignId, string CampaignName)
        {
            AppUser user = AppUser.FetchByID(AppUserId);

            string Key = Settings.Keys.EMAIL_TEMPLATE_APPUSER_GIFT;
            int TemplateId = GetEmailTemplateIdFromSettingKey(Key, "he-IL");
            EmailTemplate template = TemplateId == 0 ? null : EmailTemplateController.GetItem(TemplateId);
            if (template != null)
            {
                string fromEmail = template.FromEmail;
                string fromName = template.FromName;
                string replyToEmail = template.ReplyToEmail;
                string replyToName = template.ReplyToName;
                if (string.IsNullOrEmpty(fromEmail)) fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                if (string.IsNullOrEmpty(fromName)) fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                if (string.IsNullOrEmpty(replyToEmail)) replyToEmail = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL);

                Dictionary<string, string> dictFieldHtml = new Dictionary<string, string>();
                dictFieldHtml.Add(@"USERFULLNAME", (user.FirstName + @" " + user.LastName).Trim());
                dictFieldHtml.Add(@"CAMPAIGNNUMBER", CampaignId.ToString());
                dictFieldHtml.Add(@"GIFT", CampaignName);
                dictFieldHtml.Add(@"USERID", user.AppUserId.ToString());

                string subject = EmailTemplateController.ReplaceSharpsInString(template.Subject, dictFieldHtml);

                string body = EmailTemplateController.ReplaceSharpsInString(template.Body, dictFieldHtml);

                System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                    fromEmail, fromName, replyToEmail, replyToName,
                    replyToEmail, template.CcList, template.BccList, subject, body, null, template.MailPriority);
                EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
            }
        }
    }
}
