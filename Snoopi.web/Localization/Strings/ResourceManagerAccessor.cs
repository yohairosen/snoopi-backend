using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace Snoopi.web.Localization
{
    public static class ResourceManagerAccessor
    {
        public static string GetText(string className, string key)
        {
            Type type = Type.GetType(@"Snoopi.web.Resources." + className, false);
            if (type != null)
            {
                PropertyInfo pi = type.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (pi != null)
                {
                    System.Resources.ResourceManager ResourceManager = (System.Resources.ResourceManager)pi.GetValue(type, null);
                    if (ResourceManager != null)
                    {
                        return ResourceManager.GetString(key);
                    }
                }
            }
            return null;
        }
        public static string GetText(string className, string key, CultureInfo culture)
        {
            Type type = Type.GetType(@"Snoopi.web.Resources." + className, false);
            if (type != null)
            {
                PropertyInfo pi = type.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (pi != null)
                {
                    System.Resources.ResourceManager ResourceManager = (System.Resources.ResourceManager)pi.GetValue(type, null);
                    if (ResourceManager != null)
                    {
                        return ResourceManager.GetString(key, culture);
                    }
                }
            }
            return null;
        }
    }
}
