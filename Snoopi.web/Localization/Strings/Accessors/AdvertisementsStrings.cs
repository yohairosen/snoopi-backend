using dg.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.web.Localization
{
    public class AdvertisementsStrings
    {
        public static string GetText(string key)
        {
            return Resources.Advertisements.ResourceManager.GetString(key);
        }
        public static string GetText(string key, CultureInfo culture)
        {
            return Resources.Advertisements.ResourceManager.GetString(key, culture);
        }
        public static string GetHtml(string key)
        {
            return Resources.Advertisements.ResourceManager.GetString(key).ToHtml();
        }
        public static string GetHtml(string key, CultureInfo culture)
        {
            return Resources.Advertisements.ResourceManager.GetString(key, culture).ToHtml();
        }

        public static string GetYesNo(bool value)
        {
            return GetText(value ? @"Yes" : @"No");
        }
    }
}
