using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using dg.Utilities;

namespace Snoopi.web.Localization
{
    public static class GlobalStrings
    {
        public static string GetText(string key)
        {
            return Resources.Global.ResourceManager.GetString(key);
        }
        public static string GetText(string key, CultureInfo culture)
        {
            return Resources.Global.ResourceManager.GetString(key, culture);
        }
        public static string GetHtml(string key)
        {
            return Resources.Global.ResourceManager.GetString(key).ToHtml();
        }
        public static string GetHtml(string key, CultureInfo culture)
        {
            return Resources.Global.ResourceManager.GetString(key, culture).ToHtml();
        }

        public static string GetYesNo(bool value)
        {
            return GetText(value ? @"Yes" : @"No");
        }
    }
}
