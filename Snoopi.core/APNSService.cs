using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using dg.Utilities.Apns;
using dg.Utilities;

namespace Snoopi.core
{
    public static class APNSService
    {
        public static bool APNS_Sandbox = true;
        public static string APNS_P12 = null;
        public static string APNS_P12PWD = @"";
        public static int APNS_TIMER_MS = 500;

        static APNSService()
        {
            if (AppConfig.GetBoolean(@"APNS.Certificates.Sandbox", false))
            {
                APNS_P12 = AppConfig.GetString(@"APNS.Certificates.Development", @"");
                APNS_P12PWD = AppConfig.GetString(@"APNS.Certificates.Development.Pwd", @"");
                APNS_Sandbox = true;
            }
            else
            {
                APNS_P12 = AppConfig.GetString(@"APNS.Certificates.Production", @"");
                APNS_P12PWD = AppConfig.GetString(@"APNS.Certificates.Production.Pwd", @"");
                APNS_Sandbox = false;
            }

            if (APNS_P12.StartsWith(@"~\"))
            {
                using (Page mapPathPage = new Page())
                {
                    var newStr = APNS_P12.TrimStart(new[] { '~', '\\' });
                    APNS_P12 = AppDomain.CurrentDomain.BaseDirectory + newStr; ;
                }
            }

            APNS_TIMER_MS = AppConfig.GetInt32(@"APNS.Certificates.Timing", 500);

            apnsService = new NotificationService(APNS_Sandbox, APNS_P12, APNS_P12PWD, APNS_TIMER_MS);
        }

        private static NotificationService apnsService = null;

        public static NotificationService SharedInstance
        {
            get { return apnsService; }
        }
    }
}
