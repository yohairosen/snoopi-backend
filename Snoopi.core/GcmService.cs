using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using dg.Utilities.Apns;
using dg.Utilities;
using dg.Utilities.GoogleCloudMessaging;

namespace Snoopi.core
{
    public static class GcmService
    {
        public static string PackageName { get; set; }

        static GcmService()
        {
            int timerMs = AppConfig.GetInt32(@"Gcm.Timing", 500);            
            
            // application certificates
            PackageName = AppConfig.GetString(@"Gcm.PackageName", @"");
            string GcmApiKey = AppConfig.GetString(@"Gcm.ApiKey", @"");
            gcmService = new HttpNotificationService(GcmApiKey, timerMs);

        }

        private static HttpNotificationService gcmService = null;

        public static HttpNotificationService SharedInstance
        {
            get { return gcmService; }
        }
    }
}
