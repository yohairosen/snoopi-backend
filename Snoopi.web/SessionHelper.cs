using System;
using System.Collections.Generic;
using System.Text;
using Snoopi.core;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using System.Web;
using dg.Utilities;
using dg.Utilities.Encryption;

namespace Snoopi.web
{
    public static class SessionHelper
    {
        static readonly string RememberMeCookieEncryptionKey = @"Snoopi-authtoken";
        static readonly int AuthTokenTimeSpan = AppConfig.GetInt32(@"Authentication.AuthCookieLifeInHours", 72);
        static public Membership.UserAuthenticateResults Login(string Email, string Password, bool GenerateRememberMeCookie)
        {
            Int64 UserId;
            Membership.UserAuthenticateResults results = Membership.AuthenticateUser(Email, Password, out UserId);
            if (results != Membership.UserAuthenticateResults.Success) return results;

            UserAuthToken token = AuthTokens.GenerateAuthTokenForUserId(UserId, GenerateRememberMeCookie ? AuthTokenTimeSpan : 0);
            if (token == null) return Membership.UserAuthenticateResults.LoginError;

            if (GenerateRememberMeCookie)
            {
                HttpCookie cookie = new HttpCookie(@"auth-token", TeaEncryptor.Encrypt(token.Secret.ToString(@"N") + @":" + token.Key, RememberMeCookieEncryptionKey));
                cookie.Expires = token.Expiry;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            HttpContext.Current.Session[@"Authenticated"] = true;
            HttpContext.Current.Session[@"AuthTokenId"] = token.UserAuthTokenId;
            HttpContext.Current.Session[@"UserId"] = UserId;
            HttpContext.Current.Session[@"LangCode"] = dg.Sql.Query.New<UserProfile>().Select(UserProfile.Columns.DefaultLangCode).Where(UserProfile.Columns.UserId, UserId).ExecuteScalar() as string;

            return results;
        }

        public static bool IsLockOrDelete()
        {
            Int64 userId = UserId();
            User user = User.FetchByID(userId);
            return (user != null && user.IsLocked || user == null) ? true : false;
        }
        static public bool IsAuthenticated()
        {
            if (HttpContext.Current.Session[@"Authenticated"] != null && (bool)HttpContext.Current.Session[@"Authenticated"])
            {
                return !(IsLockOrDelete());
            }
            else
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[@"auth-token"];
                if (cookie != null)
                {
                    string[] auth = TeaEncryptor.Decrypt(cookie.Value, RememberMeCookieEncryptionKey).Split(':');
                    if (auth.Length == 2)
                    {
                        Int64 UserId;
                        Int64 AuthTokenId;
                        if (AuthTokens.ValidateAuthToken(auth[0], auth[1], out UserId, out AuthTokenId))
                        {
                            Membership.UserAuthenticateResults results = Membership.UserLoggedInAction(UserId);
                            if (results == Membership.UserAuthenticateResults.Success)
                            {
                                HttpContext.Current.Session[@"Authenticated"] = true;
                                HttpContext.Current.Session[@"AuthTokenId"] = AuthTokenId;
                                HttpContext.Current.Session[@"UserId"] = UserId;
                                HttpContext.Current.Session[@"LangCode"] = dg.Sql.Query.New<UserProfile>().Select(UserProfile.Columns.DefaultLangCode).Where(UserProfile.Columns.UserId, UserId).ExecuteScalar() as string;
                                return true;
                            }
                            else
                            {
                                UserAuthToken.Delete(AuthTokenId);
                                HttpContext.Current.Response.Cookies.Set(new HttpCookie(@"auth-token", @""));
                            }
                        }
                        else
                        {
                            HttpContext.Current.Response.Cookies.Set(new HttpCookie(@"auth-token", @""));
                        }
                    }
                }
            }
            return false;
        }
        static public Int64 UserId()
        {
            if (HttpContext.Current.Session[@"UserId"] != null) return Convert.ToInt32(HttpContext.Current.Session[@"UserId"]);
            return 0;
        }
        static public Int64 AuthTokenId()
        {
            if (HttpContext.Current.Session[@"AuthTokenId"] != null) return Convert.ToInt64(HttpContext.Current.Session[@"AuthTokenId"]);
            return 0;
        }
        static public string UserEmail()
        {
            if (HttpContext.Current.Session[@"UserEmail"] != null)
            {
                return HttpContext.Current.Session[@"UserEmail"] as string;
            }
            else
            {
                Int64 userId = UserId();
                if (userId == 0) return null;
                else
                {
                    string UserEmail = User.EmailForUserId(userId);
                    HttpContext.Current.Session[@"UserEmail"] = UserEmail;
                    return UserEmail;
                }
            }
        }
        static public string UserName()
        {
                Int64 userId = UserId();
                if (userId == 0) return null;
                else
                {
                    return UserProfile.FetchByID(userId).FirstName;
                }
        }
        static public string LangCode()
        {
            if (HttpContext.Current.Session[@"LangCode"] != null) return HttpContext.Current.Session[@"LangCode"] as string;
            return @"he-IL";
        }
        static public void SetLangCode(string LangCode)
        {
            HttpContext.Current.Session[@"LangCode"] = LangCode;
        }
        static public void SetUserEmail(string Email)
        {
            HttpContext.Current.Session[@"UserEmail"] = Email;
        }
        static public void Logout()
        {
            HttpContext.Current.Response.Cookies.Set(new HttpCookie(@"auth-token", @""));
            HttpContext.Current.Session[@"Authenticated"] = false;
            HttpContext.Current.Session[@"AuthTokenId"] = null;
            HttpContext.Current.Session[@"UserId"] = null;
            HttpContext.Current.Session[@"LangCode"] = null;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();

            foreach (string cookie in HttpContext.Current.Request.Cookies.AllKeys)
            {
                HttpContext.Current.Response.Cookies.Set(new HttpCookie(cookie, @""));
                HttpContext.Current.Response.Cookies[cookie].Expires = DateTime.Now.AddMonths(-1);
            }
        }
    }
}
