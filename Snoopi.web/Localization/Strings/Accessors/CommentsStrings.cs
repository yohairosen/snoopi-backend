using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using dg.Utilities;
using Snoopi.core.DAL;

namespace Snoopi.web.Localization
{
    public static class CommentsStrings
    {
        public static string GetText(string key)
        {
            return Resources.Comments.ResourceManager.GetString(key);
        }
        public static string GetText(string key, CultureInfo culture)
        {
            return Resources.Comments.ResourceManager.GetString(key, culture);
        }
        public static string GetHtml(string key)
        {
            return Resources.Comments.ResourceManager.GetString(key).ToHtml();
        }
        public static string GetHtml(string key, CultureInfo culture)
        {
            return Resources.Comments.ResourceManager.GetString(key, culture).ToHtml();
        }
        public static string GetStatus(CommentStatus status)
        {
            switch (status)
            {
                case CommentStatus.Approved: return GetText(@"Approved");
                case CommentStatus.Denied: return GetText(@"Denied");
                case CommentStatus.Wait:
                default: return GetText(@"Wait");
            }
        }
    }
}
