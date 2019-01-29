using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using Snoopi.core.DAL;
using dg.Sql;
using System.Security.Cryptography;

namespace Snoopi.core.BL
{
    public static class AuthTokens
    {
        static readonly bool AuthTokenSlidingExpiration_UserId = AppConfig.GetBoolean(@"Authentication.SlidingExpiration.UserId", true);
        static readonly bool AuthTokenSlidingExpiration_AppUserId = AppConfig.GetBoolean(@"Authentication.SlidingExpiration.AppUserId", true);
        static readonly int AuthTokenLifeSpan_UserId = AppConfig.GetInt32(@"Authentication.AuthTokenLifeSpan.UserId", 72);
        static readonly int AuthTokenLifeSpan_AppUserId = AppConfig.GetInt32(@"Authentication.AuthTokenLifeSpan.AppUserId", 99720);
        static readonly byte[] AuthTokenKeySalt_UserId = Encoding.UTF8.GetBytes(AppConfig.GetString(@"Authentication.AuthTokenSalt.UserId", @"authtoken"));
        static readonly byte[] AuthTokenKeySalt_AppUserId = Encoding.UTF8.GetBytes(AppConfig.GetString(@"Authentication.AuthTokenSalt.AppUserId", @"authtoken"));
        static public UserAuthToken GenerateAuthTokenForUserId(Int64 UserId, int LifeTimeInHours)
        {
            int tries = 3;
            UserAuthToken token = new UserAuthToken();
            token.UserId = UserId;
            token.CreatedOn = DateTime.UtcNow;
            token.Expiry = token.CreatedOn.AddHours(LifeTimeInHours > 0 ? LifeTimeInHours : AuthTokenLifeSpan_UserId);
            token.Key = EncodeKey(UserId, AuthTokenKeySalt_UserId);
            while (tries > 0)
            {
                try
                {
                    token.Secret = Guid.NewGuid();
                    token.Save();
                    return token;
                }
                catch (System.Data.Common.DbException)
                {
                    tries--;
                }
            }
            return null;
        }
        static public AppUserAuthToken GenerateAuthTokenForAppUserId(Int64 AppUserId, int LifeTimeInHours)
        {
            int tries = 3;
            AppUserAuthToken token = new AppUserAuthToken();
            token.AppUserId = AppUserId;
            token.CreatedDate = DateTime.UtcNow;
            token.Expiry = token.CreatedDate.AddHours(LifeTimeInHours > 0 ? LifeTimeInHours : AuthTokenLifeSpan_AppUserId);
            token.Key = EncodeKey(AppUserId, AuthTokenKeySalt_AppUserId);
            while (tries > 0)
            {
                try
                {
                    token.Secret = Guid.NewGuid();
                    token.Save();
                    return token;
                }
                catch (System.Data.Common.DbException)
                {
                    tries--;
                }
            }
            return null;
        }

        static public AppSupplierAuthToken GenerateAuthTokenForAppSupplierId(Int64 AppSupplierId, int LifeTimeInHours)
        {
            int tries = 3;
            AppSupplierAuthToken token = new AppSupplierAuthToken();
            token.SupplierId = AppSupplierId;
            token.CreatedDate = DateTime.UtcNow;
            token.Expiry = token.CreatedDate.AddHours(AuthTokenLifeSpan_AppUserId);
            token.Key = EncodeKey(AppSupplierId, AuthTokenKeySalt_AppUserId);
            while (tries > 0)
            {
                try
                {
                    token.Secret = Guid.NewGuid();
                    token.Save();
                    return token;
                }
                catch (System.Data.Common.DbException)
                {
                    tries--;
                }
            }
            return null;
        }
        static public bool ValidateAuthToken(string Secret, string Key, out Int64 UserId, out Int64 AuthTokenId)
        {
            try
            {
                Query qry = new Query(UserAuthToken.TableSchema).Where(UserAuthToken.Columns.Secret, Secret).AND(UserAuthToken.Columns.Key, Key);
                UserAuthTokenCollection coll = UserAuthTokenCollection.FetchByQuery(qry);
                if (coll.Count == 1)
                {
                    UserAuthToken token = coll[0];
                    if (token.Expiry < DateTime.UtcNow || token.Key != EncodeKey(token.UserId, AuthTokenKeySalt_UserId))
                    {
                        UserAuthToken.Delete(token.UserAuthTokenId);
                        UserId = 0;
                        AuthTokenId = 0;
                        return false;
                    }
                    else
                    {
                        UserId = token.UserId;
                        AuthTokenId = token.UserAuthTokenId;
                        DateTime newExpiry = DateTime.UtcNow.AddHours(AuthTokenLifeSpan_UserId);
                        if (newExpiry > token.Expiry) token.Expiry = newExpiry;
                        token.Save();
                        return true;
                    }
                }
                else
                {
                    UserId = 0;
                    AuthTokenId = 0;
                    return false;
                }
            }
            catch
            {
                UserId = 0;
                AuthTokenId = 0;
                return false;
            }
        }

        static public bool ValidateAppUserAuthToken(string accessToken, bool slideExpiration, out Int64 appUserId, out Int64 appUserAuthTokenId)
        {
            string secret, key;
            if (AccessToken(accessToken, out secret, out key))
            {
                return ValidateAppUserAuthToken(secret, key, slideExpiration, out appUserId, out appUserAuthTokenId);
            }
            else
            {
                appUserId = 0L;
                appUserAuthTokenId = 0L;
            }
            return false;
        }

        static public bool ValidateAppSupplierAuthToken(string accessToken, bool slideExpiration, out Int64 appSupplierId, out Int64 appSupplierAuthTokenId)
        {
            string secret, key;
            if (AccessToken(accessToken, out secret, out key))
            {
                return ValidateAppSupplierAuthToken(secret, key, slideExpiration, out appSupplierId, out appSupplierAuthTokenId);
            }
            else
            {
                appSupplierId = 0L;
                appSupplierAuthTokenId = 0L;
            }
            return false;
        }

        static public bool ValidateAppUserAuthToken(string secret, string key, bool slideExpiration, out Int64 appUserId, out Int64 appUserAuthTokenId)
        {
            try
            {
                List<object> token = new Query(AppUserAuthToken.TableSchema)
                    .Select(AppUserAuthToken.Columns.AppUserAuthTokenId)
                    .AddSelect(AppUserAuthToken.Columns.AppUserId)
                    .AddSelect(AppUserAuthToken.Columns.Expiry)
                    .Where(AppUserAuthToken.Columns.Secret, secret)
                    .AND(AppUserAuthToken.Columns.Key, key)
                    .LimitRows(1)
                    .ExecuteOneRowToList();
                if (token != null)
                {
                    DateTime expiry = Convert.ToDateTime(token[2]);
                    if (expiry < DateTime.UtcNow || key != EncodeKey(Convert.ToInt64(token[1]), AuthTokenKeySalt_AppUserId))
                    {
                        AppUserAuthToken.Delete(Convert.ToInt64(token[0]));
                        appUserId = appUserAuthTokenId = 0;
                        return false;
                    }
                    else
                    {
                        appUserId = Convert.ToInt64(token[1]);
                        appUserAuthTokenId = Convert.ToInt64(token[0]);
                        if (slideExpiration && AuthTokenSlidingExpiration_AppUserId)
                        {
                            DateTime newExpiry = DateTime.UtcNow.AddHours(AuthTokenLifeSpan_AppUserId);
                            if (newExpiry > expiry)
                            {
                                Query.New<AppUserAuthToken>()
                                    .Update(AppUserAuthToken.Columns.Expiry, newExpiry)
                                    .Where(AppUserAuthToken.Columns.AppUserAuthTokenId, appUserAuthTokenId)
                                    .Execute();
                            }
                        }
                        return true;
                    }
                }
                else
                {
                    appUserId = appUserAuthTokenId = 0;
                    return false;
                }
            }
            catch
            {
                appUserId = appUserAuthTokenId = 0;
                return false;
            }
        }

        static public bool ValidateAppSupplierAuthToken(string secret, string key, bool slideExpiration, out Int64 AppSupplierId, out Int64 AppSupplierAuthTokenId)
        {
            try
            {
                List<object> token = new Query(AppSupplierAuthToken.TableSchema)
                    .Select(AppSupplierAuthToken.Columns.AppSupplierAuthTokenId)
                    .AddSelect(AppSupplierAuthToken.Columns.SupplierId)
                    .AddSelect(AppSupplierAuthToken.Columns.Expiry)
                    .Where(AppSupplierAuthToken.Columns.Secret, secret)
                    .AND(AppSupplierAuthToken.Columns.Key, key)
                    .LimitRows(1)
                    .ExecuteOneRowToList();
                if (token != null)
                {
                    DateTime expiry = Convert.ToDateTime(token[2]);
                    if (expiry < DateTime.UtcNow || key != EncodeKey(Convert.ToInt64(token[1]), AuthTokenKeySalt_AppUserId))
                    {
                        AppSupplierAuthToken.Delete(Convert.ToInt64(token[0]));
                        AppSupplierId = AppSupplierAuthTokenId = 0;
                        return false;
                    }
                    else
                    {
                        AppSupplierId = Convert.ToInt64(token[1]);
                        AppSupplierAuthTokenId = Convert.ToInt64(token[0]);
                        if (slideExpiration && AuthTokenSlidingExpiration_AppUserId)
                        {
                            DateTime newExpiry = DateTime.UtcNow.AddHours(AuthTokenLifeSpan_AppUserId);
                            if (newExpiry > expiry)
                            {
                                Query.New<AppSupplierAuthToken>()
                                    .Update(AppSupplierAuthToken.Columns.Expiry, newExpiry)
                                    .Where(AppSupplierAuthToken.Columns.AppSupplierAuthTokenId, AppSupplierAuthTokenId)
                                    .Execute();
                            }
                        }
                        return true;
                    }
                }
                else
                {
                    AppSupplierId = AppSupplierAuthTokenId = 0;
                    return false;
                }
            }
            catch
            {
                AppSupplierId = AppSupplierAuthTokenId = 0;
                return false;
            }
        }
        
        
        static public int DeleteAllExpired()
        {
            return new Query(UserAuthToken.TableSchema).Delete().Where(UserAuthToken.Columns.Expiry, WhereComparision.LessThan, DateTime.UtcNow).Execute() +
                 new Query(AppSupplierAuthToken.TableSchema).Delete().Where(AppSupplierAuthToken.Columns.Expiry, WhereComparision.LessThan, DateTime.UtcNow).Execute() +
             new Query(AppUserAuthToken.TableSchema).Delete().Where(AppUserAuthToken.Columns.Expiry, WhereComparision.LessThan, DateTime.UtcNow).Execute();
        }
        static public string EncodeKey(Int64 UserId, byte[] Salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(UserId.ToString());
            byte[] dst = new byte[Salt.Length + bytes.Length];
            byte[] inArray = null;
            Buffer.BlockCopy(Salt, 0, dst, 0, Salt.Length);
            Buffer.BlockCopy(bytes, 0, dst, Salt.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create(@"SHA512");
            inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }

        static public string AccessToken(UserAuthToken authToken)
        {
            return AccessToken(authToken.Secret.ToString(), authToken.Key);
        }

        static public string AccessToken(AppUserAuthToken authToken)
        {
            return AccessToken(authToken.Secret.ToString(), authToken.Key);
        }
        static public string AccessToken(AppSupplierAuthToken authToken)
        {
            return AccessToken(authToken.Secret.ToString(), authToken.Key);
        }

        static public string AccessToken(string secret, string key)
        {
            return Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(secret + ":" + key));
        }

        static public bool AccessToken(string accessToken, out string secret, out string key)
        {
            try
            {
                string[] parts = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(accessToken)).Split(':');
                secret = parts[0];
                key = parts[1];
                return true;
            }
            catch
            {
                secret = key = null;
                return false;
            }
        }

        static public int DeleteAuthToken(string Secret, string Key)
        {
            return new Query(UserAuthToken.TableSchema).Delete().Where(UserAuthToken.Columns.Secret, Secret).AND(UserAuthToken.Columns.Key, Key).Execute();
        }
        static public int DeleteAppAuthToken(string Secret, string Key)
        {
            return new Query(AppUserAuthToken.TableSchema).Delete().Where(AppUserAuthToken.Columns.Secret, Secret).AND(AppUserAuthToken.Columns.Key, Key).Execute();
        }
        static public int DeleteAppSupplierAuthToken(string Secret, string Key)
        {
            return new Query(AppSupplierAuthToken.TableSchema).Delete().Where(AppUserAuthToken.Columns.Secret, Secret).AND(AppUserAuthToken.Columns.Key, Key).Execute();
        }
        static public int DeleteAppAuthToken(string accessToken)
        {
            string secret, key;
            AccessToken(accessToken, out secret, out key);
            return new Query(AppUserAuthToken.TableSchema).Delete().Where(AppUserAuthToken.Columns.Secret, secret).AND(AppUserAuthToken.Columns.Key, key).Execute();
        }

        static public int DeleteAppSupplierAuthToken(string accessToken)
        {
            string secret, key;
            AccessToken(accessToken, out secret, out key);
            return new Query(AppSupplierAuthToken.TableSchema).Delete().Where(AppSupplierAuthToken.Columns.Secret, secret).AND(AppSupplierAuthToken.Columns.Key, key).Execute();
        }
    }
}
