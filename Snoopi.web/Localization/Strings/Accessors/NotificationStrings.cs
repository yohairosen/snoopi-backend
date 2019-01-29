using System.Globalization;
using dg.Utilities;

namespace Snoopi.web.Localization.Strings.Accessors
{
    public class NotificationStrings
    {
        public static string GetText(string key)
        {
            return Resources.Notifications.ResourceManager.GetString(key);
        }
        public static string GetText(string key, CultureInfo culture)
        {
            return Resources.Notifications.ResourceManager.GetString(key, culture);
        }
        public static string GetHtml(string key)
        {
            return Resources.Notifications.ResourceManager.GetString(key).ToHtml();
        }
        public static string GetHtml(string key, CultureInfo culture)
        {
            return Resources.Notifications.ResourceManager.GetString(key, culture).ToHtml();
        }
    }
}
