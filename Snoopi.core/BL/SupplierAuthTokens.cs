using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using Snoopi.core.DAL;
using dg.Sql;
using System.Security.Cryptography;

namespace Snoopi.core.BL
{
    public static class SupplierAuthTokens
    {
        static readonly int AuthTokenLifeSpan_UserId = AppConfig.GetInt32(@"Authentication.AuthTokenLifeSpan.UserId", 72);
        static readonly int AuthTokenLifeSpan_AppUserId = AppConfig.GetInt32(@"Authentication.AuthTokenLifeSpan.AppUserId", 72);
        static readonly byte[] AuthTokenKeySalt_UserId = Encoding.UTF8.GetBytes(AppConfig.GetString(@"Authentication.AuthTokenSalt.UserId", @"authtoken"));
        static readonly byte[] AuthTokenKeySalt_AppUserId = Encoding.UTF8.GetBytes(AppConfig.GetString(@"Authentication.AuthTokenSalt.AppUserId", @"authtoken"));
   
        static public AppSupplierAuthToken GenerateAuthTokenForAppSupplierId(Int64 AppSupplierId, int LifeTimeInHours)
        {
            int tries = 3;
            AppSupplierAuthToken token = new AppSupplierAuthToken();
            token.SupplierId = AppSupplierId;
            token.CreatedDate = DateTime.UtcNow;
            token.Expiry = token.CreatedDate.AddHours(LifeTimeInHours > 0 ? LifeTimeInHours : AuthTokenLifeSpan_AppUserId);
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
        static public bool ValidateAppSupplierAuthToken(string Secret, string Key, out Int64 AppSupplierId, out Int64 AppSupplierAuthTokenId)
        {
            try
            {
                Query qry = new Query(AppSupplierAuthToken.TableSchema).Where(AppSupplierAuthToken.Columns.Secret, Secret).AND(AppSupplierAuthToken.Columns.Key, Key);
                AppSupplierAuthTokenCollection coll = AppSupplierAuthTokenCollection.FetchByQuery(qry);
                if (coll.Count == 1)
                {
                    AppSupplierAuthToken token = coll[0];
                    if (token.Expiry < DateTime.UtcNow || token.Key != EncodeKey(token.SupplierId, AuthTokenKeySalt_AppUserId))
                    {
                        AppSupplierAuthToken.Delete(token.AppSupplierAuthTokenId);
                        AppSupplierId = AppSupplierAuthTokenId = 0;
                        return false;
                    }
                    else
                    {
                        AppSupplierId = token.SupplierId;
                        AppSupplierAuthTokenId = token.AppSupplierAuthTokenId;
                        DateTime newExpiry = DateTime.UtcNow.AddHours(AuthTokenLifeSpan_AppUserId);
                        if (newExpiry > token.Expiry) token.Expiry = newExpiry;
                        token.Save();
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
            return 
             new Query(AppSupplierAuthToken.TableSchema).Delete().Where(AppSupplierAuthToken.Columns.Expiry, WhereComparision.LessThan, DateTime.UtcNow).Execute();
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
        static public int DeleteAppAuthToken(string Secret, string Key)
        {
            return new Query(AppSupplierAuthToken.TableSchema).Delete().Where(AppSupplierAuthToken.Columns.Secret, Secret).AND(AppSupplierAuthToken.Columns.Key, Key).Execute();
        }
    }
}
