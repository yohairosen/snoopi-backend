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
    public static class GcmServiceSupplier
    {
        public static string PackageName { get; set; }

        static GcmServiceSupplier()
        {
            int timerMs = AppConfig.GetInt32(@"Gcm.TimingSupplier", 500);            
            
            // application certificates
            PackageName = AppConfig.GetString(@"Gcm.PackageNameSupplier", @"");
            string GcmApiKey = AppConfig.GetString(@"Gcm.ApiKeySupplier", @"");
            gcmService = new HttpNotificationService(GcmApiKey, timerMs);

        }

        private static HttpNotificationService gcmService = null;

        public static HttpNotificationService SharedInstance
        {
            get { return gcmService; }
        }
    }
}
