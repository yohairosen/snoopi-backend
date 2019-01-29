using dg.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.web.Localization.Strings.Accessors
{
        public static class SupplierEventStrings
        {
            public static string GetText(string key)
            {
                return Resources.SupplierEvent.ResourceManager.GetString(key);
            }
            public static string GetText(string key, CultureInfo culture)
            {
                return Resources.SupplierEvent.ResourceManager.GetString(key, culture);
            }
            public static string GetHtml(string key)
            {
                return Resources.SupplierEvent.ResourceManager.GetString(key).ToHtml();
            }
            public static string GetHtml(string key, CultureInfo culture)
            {
                return Resources.SupplierEvent.ResourceManager.GetString(key, culture).ToHtml();
            }
        }
}
