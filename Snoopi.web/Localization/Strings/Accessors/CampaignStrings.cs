using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using dg.Utilities;
using Snoopi.core.DAL;

namespace Snoopi.web.Localization
{
    public static class CampaignStrings
    {
        public static string GetText(string key)
        {
            return Resources.Campaign.ResourceManager.GetString(key);
        }
        public static string GetText(string key, CultureInfo culture)
        {
            return Resources.Campaign.ResourceManager.GetString(key, culture);
        }
        public static string GetHtml(string key)
        {
            return Resources.Campaign.ResourceManager.GetString(key).ToHtml();
        }
        public static string GetHtml(string key, CultureInfo culture)
        {
            return Resources.Campaign.ResourceManager.GetString(key, culture).ToHtml();
        }
      
    }
}
