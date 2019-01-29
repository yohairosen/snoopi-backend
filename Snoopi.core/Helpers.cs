using Snoopi.infrastructure.Loggers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Snoopi.core
{
    public class Helpers
    {
        static private ILogger logger = new FileLogger();

        public static void LogState(object obj)
        {
            try
            {
                string location = "C:/Logs/";
                string userName = HttpContext.Current.Session[@"UserId"].ToString();
                var callStackArray = new StackTrace();
                var stackFrames = callStackArray.GetFrames();
                string callStackString = String.Join(", ", stackFrames.Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x.GetMethod())));
                string stringObj = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                logger.LogText(obj.GetType().Name, stringObj, location);
                logger.LogText("userName", userName, location);
                logger.LogText("callStack", callStackString, location);
            }
            catch (Exception e)
            {

            }
        }

        public static void LogProcessing(string header, string text, bool isPrintStacktrace)
        {
            try
            {
                string callStackString = "";
                string location = "C:/ProcessingLogs/";
                if (isPrintStacktrace)
                {
                    var callStackArray = new StackTrace();
                    var stackFrames = callStackArray.GetFrames();
                    callStackString = "\n CallStack: " + String.Join(", ", stackFrames.Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x.GetMethod())));
                }
                logger.LogText(header, text + callStackString, location);
            }
            catch (Exception e)
            {

            }
        }
    }
}
