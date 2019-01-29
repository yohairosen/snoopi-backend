using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using dg.Utilities;

namespace Snoopi.web.Localization
{
    public static class UsersStrings
    {
        public static string GetText(string key)
        {
            return Resources.Users.ResourceManager.GetString(key);
        }
        public static string GetText(string key, CultureInfo culture)
        {
            return Resources.Users.ResourceManager.GetString(key, culture);
        }
        public static string GetHtml(string key)
        {
            return Resources.Users.ResourceManager.GetString(key).ToHtml();
        }
        public static string GetHtml(string key, CultureInfo culture)
        {
            return Resources.Users.ResourceManager.GetString(key, culture).ToHtml();
        }
    }
}
