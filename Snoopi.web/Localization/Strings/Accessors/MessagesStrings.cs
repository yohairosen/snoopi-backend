using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using dg.Utilities;

namespace Snoopi.web.Localization
{
    public static class MessagesStrings
    {
        public static string GetText(string key)
        {
            return Resources.Messages.ResourceManager.GetString(key);
        }
        public static string Messages(string key, CultureInfo culture)
        {
            return Resources.Messages.ResourceManager.GetString(key, culture);
        }
        public static string GetHtml(string key)
        {
            return Resources.Messages.ResourceManager.GetString(key).ToHtml();
        }
        public static string GetHtml(string key, CultureInfo culture)
        {
            return Resources.Messages.ResourceManager.GetString(key, culture).ToHtml();
        }

        public static string GetYesNo(bool value)
        {
            return GetText(value ? @"Yes" : @"No");
        }
    }
}
