
using dg.Sql;
using dg.Utilities;
using FirebaseNet.Messaging;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Snoopi.core
{
    public class FcmService
    {
        private static string _key;
        private static ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
       // private static ConcurrentQueue<string> _unreadQueue = new ConcurrentQueue<long>();
        private static string GetServerKey()
        {
            if (_key == null)
                _key = AppConfig.GetString(@"Fcm.ApiKey", @"");

            return _key;
        }

        public static async Task SendTemplateToMany(string pushName, string template, List<NotificationUser> users)
        {         
            var fcmUsers = users.Where(x => !String.IsNullOrEmpty(x.FcmToken));
            var apnUsers = users.Where(x => !String.IsNullOrEmpty(x.ApnToken));
            int numOfFcnTasks = fcmUsers.Count() / 250 + 1;
            var listOfFcnTasks = new List<Task>();
            for (int i = 0; i < numOfFcnTasks; i++)
            {
                var usersToSend = fcmUsers.Skip(i * 250).Take(250).ToList();
                listOfFcnTasks.Add(Task.Run(() => sendFcmMessages(pushName, template, usersToSend)));            
            }
          //  int numOfApnTasks = apnUsers.Count() / 250 + 1;
         ////   var listOfApnTasks = new List<Task<List<long>>>();
         //   for (int i = 0; i < numOfApnTasks; i++)
         //   {
         //       var usersToSend = apnUsers.Skip(i * 250).Take(250).ToList();
         //       listOfApnTasks.Add(Task.Run
         //           (() => sendApnMessages(pushName, template, usersToSend)));
         //   }
            await Task.WhenAll(listOfFcnTasks.ToArray());
         //   await Task.WhenAll(listOfApnTasks.ToArray());

            using (System.IO.StreamWriter sw = System.IO.File.AppendText(AppDomain.CurrentDomain.BaseDirectory + @"\Output\push-log.txt"))
            {
                foreach(string log in _logQueue)
                    sw.WriteLine(log);
                _logQueue = new ConcurrentQueue<string>();
            }          
        }

        public static int SendSameMessageToMany(string body, List<NotificationUser> users)
        {
            var fcmUsers = users.Where(x => x.FcmToken != null);
            var apnUsers = users.Where(x => x.ApnToken != null);

            int num = SendFcmBatchMessages(body, fcmUsers.ToList()); //TODO send as parallel
            var userIds = users.Select(x => x.AppUserId).ToList();
            Notification.UpdateUnreadNotificationCountOfUsers(userIds);
            return num;
        }

        private static async Task sendFcmMessages(string pushName, string template, List<NotificationUser> users)
        {
            var usersIds = users.Select(x => x.AppUserId).ToList();
            var tokens = AppUserGcmToken.FetchByIDs(usersIds);
            var tasks = new List<Task>();
            foreach (var user in users)
            {
                if (tokens.ContainsKey(user.AppUserId) && tokens[user.AppUserId] != null)
                {
                    foreach (string token in tokens[user.AppUserId])
                    {
                        string body = template.Replace("--firstname--", user.FirstName).Replace("--lastname--", user.LastName);                      
                        try
                        {
                           await Task.Run(() => SendFcmMessage(pushName, body, user, token));
                        }
                        catch (Exception ex)
                        {
                            _logQueue.Enqueue(@" ------------" + DateTime.Now + "--------------------" + '\n' + "Exception :  " + '\n' + ex.Message + '\n' + "  StackTrace :  " + ex.StackTrace + '\n');
                        }
                    }
                }
            }
        }

        private static List<long> sendApnMessages(string pushName, string template, List<NotificationUser> users)
        {
            foreach (var user in users)
            {
                string body = template.Replace("--firstname--", user.FirstName).Replace("--lastname--", user.LastName);
                Notification.SendGenerealNotificationToApnUser(body, user.AppUserId, user.IsTempUser, user.ApnToken);
            }
            var userIds = users.Select(x => x.AppUserId).ToList();
            return userIds;
        }

        private static int SendFcmBatchMessages(string body, List<NotificationUser> users) // restricted to 1000 
        {
            FCMClient client = new FCMClient(GetServerKey());
            var message = new FirebaseNet.Messaging.Message()
            {
                RegistrationIds = users.Select(x => x.FcmToken).ToList(),
                Data = new Dictionary<string, string>
                {
                    {"app-action",  @"new-message"},
                    {"message", body }
                }
            };
            var res = new List<DownstreamMessageResponse>();
            var response = client.SendMessageAsync(message);
            var contiuation = response.ContinueWith(t => res.Add((DownstreamMessageResponse)t.Result));
            Notification.UpdateUnreadNotificationCountOfUsers(users.Select(x => x.AppUserId).ToList());
            contiuation.Wait();
            return (int)res.Sum(x => x.Success);
        }

        private static async Task SendFcmMessage(string pushName, string body, NotificationUser user, string token)
        {
            FCMClient client = new FCMClient(GetServerKey());
            var message = new FirebaseNet.Messaging.Message()
            {
                To = token,
                Data = new Dictionary<string, string>
                {
                    {"badge", (user.UnreadNotificationCount + 1).ToString() },
                    {"app-action",  @"new-message"},
                    {"message", body }
                }
            };
            var response = client.SendMessageAsync(message);
            await response.ContinueWith(t =>
            {
                var results = t.Result as DownstreamMessageResponse;
                _logQueue.Enqueue(@" ------------" + DateTime.Now + "--------------------" + '\n' + "sent to  " + user.AppUserId + " is success : " + results.Success + '\n' + "errors : " + String.Join(" , ", results.Results.Select(x => x.Error)));
            });
        }

        private void WriteStatistics(DownstreamMessageResponse response)
        {

        }
    }
}
