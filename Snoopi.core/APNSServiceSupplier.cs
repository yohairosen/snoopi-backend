using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using dg.Utilities.Apns;
using dg.Utilities;
using System.IO;

namespace Snoopi.core
{
    public static class APNSServiceSupplier
    {
        public static bool APNS_Sandbox = true;
        public static string APNS_P12 = null;
        public static string APNS_P12PWD = @"";
        public static int APNS_TIMER_MS = 500;

        private static void SetAPNSServiceSupplier()
        {
            if (AppConfig.GetBoolean(@"APNSSupplier.Certificates.Sandbox", false))
            {
                APNS_P12 = AppConfig.GetString(@"APNSSupplier.Certificates.Development", @"");
                APNS_P12PWD = AppConfig.GetString(@"APNSSupplier.Certificates.Development.Pwd", @"");
                APNS_Sandbox = true;
            }
            else
            {
                APNS_P12 = AppConfig.GetString(@"APNSSupplier.Certificates.Production", @"");
                APNS_P12PWD = AppConfig.GetString(@"APNSSupplier.Certificates.Production.Pwd", @"");
                APNS_Sandbox = false;
            }
            if (APNS_P12.StartsWith(@"~\"))
               {
                   using (Page mapPathPage = new Page())
                   {
                      var newStr = APNS_P12.TrimStart(new[] { '~', '\\'});
                       APNS_P12 = AppDomain.CurrentDomain.BaseDirectory + newStr;
                      // APNS_P12 = mapPathPage.Server.MapPath(newStr);
                   }
               }

            //if (APNS_P12.StartsWith(@"~/"))
            //{
            //    using (Page mapPathPage = new Page())
            //    {
            //        var newStr = APNS_P12.TrimStart(new[] { '~' });
            //        APNS_P12 = AppDomain.CurrentDomain.BaseDirectory + newStr;
            //       // APNS_P12 = mapPathPage.Server.MapPath(newStr);
            //    }
            //}
                 try
              {
                  if (!File.Exists(APNS_P12))
                      using (System.IO.StreamWriter sw = System.IO.File.AppendText(AppDomain.CurrentDomain.BaseDirectory + @"\Output\log.txt"))
                      {
                          sw.WriteLine(@"P12 file does not exist: " + APNS_P12);
                      }
  
                  else
                      using (System.IO.StreamWriter sw = System.IO.File.AppendText(AppDomain.CurrentDomain.BaseDirectory + @"\Output\log.txt"))
                      {
                          sw.WriteLine(@"P12 exist! " + APNS_P12);
                      }
              }
              catch { }
  

            APNS_TIMER_MS = AppConfig.GetInt32(@"APNSSupplier.Certificates.Timing", 500);

            apnsService = new NotificationService(APNS_Sandbox, APNS_P12, APNS_P12PWD, APNS_TIMER_MS);
        }

        private static NotificationService apnsService;

        public static NotificationService SharedInstance
        {
            get {
                if (apnsService == null)
                    SetAPNSServiceSupplier();
                return apnsService;

            }
        }
    }
}
