using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snoopi.core.DAL;
using dg;
using dg.Sql;
using dg.Utilities;
using System.Web;
using System.Collections;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Globalization;

namespace Snoopi.core.BL
{
    public static class Settings
    {
        public struct Keys
        {
            // Paths and urls
            public const string TEMP_UPLOAD_FOLDER = @"temp-upload-folder";
            public const string APPUSERS_UPLOAD_FOLDER = @"appusers-upload-folder";
            public const string API_TEMP_UPLOAD_FOLDER = @"api-temp-upload-folder";
            public const string API_APPUSERS_UPLOAD_FOLDER = @"api-appusers-upload-folder";
            public const string API_APPUSERS_NO_IMAGE = @"api-appusers-no-image";
            public const string SECURE_UPLOAD_FOLDER = @"secure-upload-folder";
            public const string WEB_ROOT_URL = @"web-root-url";
            public const string API_ROOT_URL = @"api-root-url";

            public const string CONTACT_FOLDER = @"contact-folder";

            public const string PRIVACY_POLICY_URL = @"privacy-policy-url";

            // App-user settings
            public const string APPUSER_VERIFY_EMAIL = @"appuser-verify-email"; // Do the app-user need to verify email address?

            // App-user related emails
            public const string EMAIL_TEMPLATE_NEW_APPUSER_WELCOME = @"email-template-new-appuser-welcome";
            public const string EMAIL_TEMPLATE_NEW_APPUSER_WELCOME_VERIFY_EMAIL = @"email-template-new-appuser-welcome-verify-email";
            public const string EMAIL_TEMPLATE_APPUSER_FORGOT_PASSWORD = @"email-template-appuser-forgot-password";
            public const string EMAIL_TEMPLATE_APPUSER_GIFT = @"email-tamplate-appuser-gift";

            public const string EMAIL_TEMPLATE_SUPPLIER_FORGOT_PASSWORD = @"email-template-supplier-forgot-password";
            public const string EMAIL_NEW_PRODUCT = @"email-new-product";
            public const string EMAIL_UNTAKEN_BID = @"email-untaken-bid";
            // User related emails
            public const string EMAIL_TEMPLATE_USER_FORGOT_PASSWORD = @"email-template-user-forgot-password";

            public const string EMAIL_TEMPLATE_SUPPLIER_NEW_BID = @"email-template-suplier-new-bid";

            // Tremp match limits
            public const string SUPPLIER_RADIUS = @"supplier-radius";

            // Email settings
            public const string ADMIN_EMAIL = @"admin-email";
            public const string DEFAULT_EMAIL_FROM = @"default-email-from";
            public const string DEFAULT_EMAIL_FROM_NAME = @"default-email-from-name";
            public const string DEFAULT_EMAIL_REPLYTO = @"default-email-replyto";
            public const string DEFAULT_EMAIL_REPLYTO_NAME = @"default-email-replyto-name";
            public const string SMTP_PASSWORD = @"smtp_username";
            public const string SMTP_USER_NAME = @"smtp_password";



            public struct MailSettings
            {
                public const string MAIL_SERVER_AUTHENTICAION = @"mail-server-authentication";
                public const string MAIL_SERVER_SSL = @"mail-server-ssl";
                public const string MAIL_SERVER_USERNAME = @"mail-server-username";
                public const string MAIL_SERVER_PASSWORD = @"mail-server-password";
                public const string MAIL_SERVER_HOSTNAME = @"mail-server-hostname";
                public const string MAIL_SERVER_PORT = @"mail-server-port";
            }

            public const string END_BID_TIME_MIN = @"end-bid-time-min";
            public const string EXPIRY_OFFER_TIME_HOURS = @"expiry-offer-service-time-hours";
            public const string RATE_SUPPLIER_AFTER_ORDER_HOUR = @"rate-supplier-after-order-hour";
            public const string SUPPLIED_WITHIN_HOUR = @"supplied-within-hour";
            public const string YAD_2_EXPIRY_DAY = @"yad-2-expiry-day";
            public const string MIN_PRICE_FOR_OFFER_BIDS = @"min-price-for-offer-bids";
            public const string DEVIATION_LOWEST_THRESHOLD = @"deviation-lowest-threshold";
            public const string DEVIATION_PERCENTAGE = @"deviation-percentage";
            public const string START_WORKING_TIME = @"start-working-time";
            public const string END_WORKING_TIME = @"end-working-time";
            public const string IS_SYSTEM_ACTIVE = @"is-system-active";
            public const string MESSAGE_EXPIRATION_SUPPLIER = @"message-expiration-supplier";
            public const string MESSAGE_EXPIRATION_PREMIUM = @"message-expiration-premium";
            public const string MESSAGE_EXPIRATION_SPECIAL_DEAL = @"message-expiration-special-deal";

            public const string MIN_ANDROID_VERSION = @"min-android-version";
            public const string MIN_IOS_VERSION = @"min-ios-version";

            public const string SUPPLIER_MIN_ANDROID_VERSION = @"supplier-min-android-version";
            public const string SUPPLIER_MIN_IOS_VERSION = @"supplier-min-ios-version";

            public const string ADMIN_PHONE = @"admin-phone";

            public const string BANNER_CATEGORY = @"banner-category";
            public const string BANNER_HOME = @"banner-home";
            public const string BANNER_SUB_CATEGORY = @"banner-sub-category";

            //titles
            public const string TITLE_PRICES = @"title-prices";
            public const string TITLE_CITIES = @"title-cities";
            public const string TITLE_CATEGORIES = @"title-categories";
            
        }

        static ConcurrentDictionary<string, string> SysCache = new ConcurrentDictionary<string, string>();
        public static string GetSetting(string key)
        {      
                Query q = new Query(Setting.TableSchema).Select(Setting.Columns.Value).Where(Setting.Columns.Key, key);
                var tryValue = new Query(Setting.TableSchema).Select(Setting.Columns.Value).Where(Setting.Columns.Key, key).LimitRows(1).ExecuteScalar() as string;
                SysCache[key] = tryValue;
                return tryValue;     
        }
        public static void SetSetting(string key, string value)
        {
            if (key == Keys.TEMP_UPLOAD_FOLDER || key == Keys.APPUSERS_UPLOAD_FOLDER ||
                key == Keys.API_TEMP_UPLOAD_FOLDER || key == Keys.API_APPUSERS_UPLOAD_FOLDER ||
                key == Keys.SECURE_UPLOAD_FOLDER)
            {
                if (!value.EndsWith(@"/") && !value.EndsWith(@"\"))
                {
                    if (value.IndexOf('/') > -1) value += '/';
                    else value += '\\';
                }
            }
            else if (key == Keys.WEB_ROOT_URL || key == Keys.API_ROOT_URL)
            {
                if (!value.EndsWith(@"/")) value += "/";
            }

            try
            {
                new Query(Setting.TableSchema)
                    .Insert(Setting.Columns.Key, key)
                    .Insert(Setting.Columns.Value, value).Execute();
            }
            catch (DbException)
            {
                new Query(Setting.TableSchema)
                    .Update(Setting.Columns.Value, value)
                    .Where(Setting.Columns.Key, key)
                    .Execute();
            }
            SysCache[key] = value;
        }
        public static void ClearSysCache()
        {
            SysCache.Clear();
        }
        
        public static void SetSetting(string key, bool value)
        {
            SetSetting(key, value ? @"1" : @"0");
        }
        public static void SetSetting(string key, Int32 value)
        {
            SetSetting(key, value.ToString());
        }
        public static void SetSetting(string key, Int64 value)
        {
            SetSetting(key, value.ToString());
        }
        public static void SetSetting(string key, decimal value)
        {
            SetSetting(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public static Int32 GetSettingInt32(string key, Int32 defaultValue)
        {
            Int32 value;
            if (!Int32.TryParse(GetSetting(key), out value)) value = defaultValue;
            return value;
        }
        public static Int64 GetSettingInt64(string key, Int64 defaultValue)
        {
            Int64 value;
            if (!Int64.TryParse(GetSetting(key), out value)) value = defaultValue;
            return value;
        }
        public static decimal GetSettingDecimal(string key, decimal defaultValue)
        {
            decimal value;
            if (!decimal.TryParse(GetSetting(key), NumberStyles.Float, CultureInfo.InvariantCulture, out value)) value = defaultValue;
            return value;
        }
        public static bool GetSettingBool(string key, bool defaultValue)
        {
            string value = GetSetting(key);
            if (value == null) return defaultValue;
            if (value == @"1" || value == @"true") return true;
            if (value == @"0" || value == @"false") return false;
            return defaultValue;
        }
    }
}