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
    public static class SuppliersSessionHelper
    {
        static readonly string RememberMeCookieEncryptionKey = @"Snoopi-authtoken";
        static readonly int AuthTokenTimeSpan = AppConfig.GetInt32(@"Authentication.AuthCookieLifeInHours", 72);
        static public Membership.UserAuthenticateResults Login(string Email, string Password, bool GenerateRememberMeCookie)
        {
            Int64 SupplierId;
            Membership.UserAuthenticateResults results = Membership.AuthenticateSupplier(Email, Password, out SupplierId);
            if (results != Membership.UserAuthenticateResults.Success) return results;

            AppSupplierAuthToken token = AuthTokens.GenerateAuthTokenForAppSupplierId(SupplierId, GenerateRememberMeCookie ? AuthTokenTimeSpan : 0);
            if (token == null) return Membership.UserAuthenticateResults.LoginError;

            if (GenerateRememberMeCookie)
            {
                HttpCookie cookie = new HttpCookie(@"auth-token", TeaEncryptor.Encrypt(token.Secret.ToString(@"N") + @":" + token.Key, RememberMeCookieEncryptionKey));
                cookie.Expires = token.Expiry;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            HttpContext.Current.Session[@"Authenticated"] = true;
            HttpContext.Current.Session[@"AuthTokenId"] = token.AppSupplierAuthTokenId;
            HttpContext.Current.Session[@"SupplierId"] = SupplierId;
            AppSupplier supplier = AppSupplier.FetchByID(SupplierId);
            HttpContext.Current.Session[@"IsProductSupplier"] = (supplier != null ? supplier.IsProduct : false);
            //HttpContext.Current.Session[@"LangCode"] = dg.Sql.Query.New<AppSupplier>().Select(AppSupplier.Columns.LangCode).Where(AppSupplier.Columns.SupplierId, SupplierId).ExecuteScalar() as string;

            return results;
        }
        static public bool IsAuthenticated()
        {
            if (HttpContext.Current.Session[@"Authenticated"] != null && (bool)HttpContext.Current.Session[@"Authenticated"])
            {
                return true;
            }
            else
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[@"auth-token"];
                if (cookie != null)
                {
                    string[] auth = TeaEncryptor.Decrypt(cookie.Value, RememberMeCookieEncryptionKey).Split(':');
                    if (auth.Length == 2)
                    {
                        Int64 SupplierId;
                        Int64 AuthTokenId;
                        if (AuthTokens.ValidateAppSupplierAuthToken(auth[0], auth[1], false, out SupplierId, out AuthTokenId))//TODO
                        {
                            Membership.UserAuthenticateResults results = Membership.SupplierLoggedInAction(SupplierId);
                            if (results == Membership.UserAuthenticateResults.Success)
                            {
                                HttpContext.Current.Session[@"Authenticated"] = true;
                                HttpContext.Current.Session[@"AuthTokenId"] = AuthTokenId;
                                HttpContext.Current.Session[@"SupplierId"] = SupplierId;
                                AppSupplier supplier = AppSupplier.FetchByID(SupplierId);
                                HttpContext.Current.Session[@"IsProductSupplier"] = (supplier != null ? supplier.IsProduct : false);
                                //HttpContext.Current.Session[@"LangCode"] = dg.Sql.Query.New<AppSupplier>().Select(AppSupplier.Columns.LangCode).Where(AppSupplier.Columns.SupplierId, SupplierId).ExecuteScalar() as string;
                                return true;
                            }
                            else
                            {
                                AppSupplierAuthToken.Delete(AuthTokenId);
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
        static public Int64 SupplierId()
        {
            if (HttpContext.Current.Session[@"SupplierId"] != null) return Convert.ToInt32(HttpContext.Current.Session[@"SupplierId"]);
            return 0;
        }

        static public bool IsProductSupplier()
        {
            if (HttpContext.Current.Session[@"IsProductSupplier"] != null) return Convert.ToBoolean(HttpContext.Current.Session[@"IsProductSupplier"]);
            return false;
            
        }
        static public Int64 AuthTokenId()
        {
            if (HttpContext.Current.Session[@"AuthTokenId"] != null) return Convert.ToInt64(HttpContext.Current.Session[@"AuthTokenId"]);
            return 0;
        }
        static public string SupplierEmail()
        {
            if (HttpContext.Current.Session[@"SupplierEmail"] != null)
            {
                return HttpContext.Current.Session[@"SupplierEmail"] as string;
            }
            else
            {
                Int64 supplierId = SupplierId();
                if (supplierId == 0) return null;
                else
                {
                    string SupplierEmail = "";//AppSupplier.EmailForUserId(supplierId);
                    HttpContext.Current.Session[@"SupplierEmail"] = SupplierEmail;
                    return SupplierEmail;
                }
            }
        }
        static public string SupplierName()
        {
            Int64 supplierId = SupplierId();
            if (supplierId == 0) return null;
                else
                {
                    return AppSupplier.FetchByID(supplierId).ContactName;
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
        static public void SetSupplierEmail(string Email)
        {
            HttpContext.Current.Session[@"SupplierEmail"] = Email;
        }
        static public void Logout()
        {
            HttpContext.Current.Response.Cookies.Set(new HttpCookie(@"auth-token", @""));
            HttpContext.Current.Session[@"Authenticated"] = false;
            HttpContext.Current.Session[@"AuthTokenId"] = null;
            HttpContext.Current.Session[@"SupplierId"] = null;
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
