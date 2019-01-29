using Snoopi.core.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.BL
{
    public class NotificationsFiltersCache
    {
        private static List<NotificationFilter> _notificationsCache;
        private static object _lock = new object();
        private static DateTime _lastUpdated;

        public static List<NotificationFilter> GetAllNotifications()
        {
            if (_notificationsCache == null)
            {
                lock (_lock)
                {
                    if (_notificationsCache == null)
                    {
                        _notificationsCache = new List<NotificationFilter>();
                        _lastUpdated = DateTime.MinValue;
                    }
                }

            }
            var now = DateTime.Now;
            if (_lastUpdated.AddSeconds(60) < now)
            {
                lock (_lock)
                {
                    if (_lastUpdated.AddSeconds(60) < now)
                    {
                        _notificationsCache = NotificationFilterCollection.FetchAll().Where(x => x.Deleted == null && !x.IsAuto).ToList();
                        _lastUpdated = DateTime.Now;
                    }
                }
            }
            return _notificationsCache;
        }
    }
}
