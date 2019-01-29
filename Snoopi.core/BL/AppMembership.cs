using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using Snoopi.core.DAL;
using dg.Sql;
using System.Security.Cryptography;
using System.Collections;
using dg.Sql.Connector;
using System.Web;
using System.Web.Caching;
using System.Data.Common;
using Facebook;
using System.Diagnostics;
using System.Globalization;

namespace Snoopi.core.BL
{
    public static class AppMembership
    {
        public enum AppUserCreateResults
        {
            Success,
            InvalidEmailAddress,
            AlreadyExists,
            UnknownError
        }
        public enum AppUserAuthenticateResults
        {
            Success,
            LoginError,
            NoMatch,
            Locked,
            NotVerified
        }
        public enum AppUserFacebookConnectResults
        {
            Success,
            LoginError
        }
        public enum AppUserPasswordChangeResults
        {
            Success,
            PasswordDoNotMatch,
            AppUserDoesNotExist
        }
        public enum AppUserDeleteResults
        {
            Success,
            PasswordDoNotMatch,
            AppUserDoesNotExist
        }
        public enum AppUserRecoveryResults
        {
            Success,
            KeyDoNotMatch,
            Expired,
            AppUserDoesNotExist
        }

        public enum OsType
        {
            Android = 1,
            IOS = 2
        }


        public static bool AuthenticateDeviceVersion(string version, OsType osType)
        {
            if (osType == OsType.Android)
            {
                return Convert.ToDouble(version) < Convert.ToDouble(Settings.GetSetting(Settings.Keys.MIN_ANDROID_VERSION));
            }
            else
            {
                return Convert.ToDouble(version) < Convert.ToDouble(Settings.GetSetting(Settings.Keys.MIN_IOS_VERSION));
            }

        }
        public static bool AuthenticateDeviceVersionSupplier(string version, OsType osType)
        {
            if (osType == OsType.Android)
            {
                return Convert.ToDouble(version) < Convert.ToDouble(Settings.GetSetting(Settings.Keys.SUPPLIER_MIN_ANDROID_VERSION));
            }
            else
            {
                return Convert.ToDouble(version) < Convert.ToDouble(Settings.GetSetting(Settings.Keys.SUPPLIER_MIN_IOS_VERSION));
            }

        }

        public static int MaximumBadLoginTries = AppConfig.GetInt32(@"Authentication.MaximumBadLoginTries.AppUserId", 0);
        public static int RecoveryKeyLifeInHours = AppConfig.GetInt32(@"Authentication.RecoveryKeyLifeInHours.AppUserId", 72);

        public static AppUserCreateResults CreateAppUser(string Email, string Password, string LangCode, out AppUser AppUser)
        {
            AppUser = null;
            if (!Email.IsValidEmail()) return AppUserCreateResults.InvalidEmailAddress;
            AppUser = AppUser.FetchByEmail(Email);
            if (AppUser != null)
            {
                return AppUserCreateResults.AlreadyExists;
            }
            AppUser = new AppUser();
            AppUser.IsVerified = !Settings.GetSettingBool(Settings.Keys.APPUSER_VERIFY_EMAIL, false);
            AppUser.Email = Email;
            AppUser.UniqueIdString = Email.NormalizeEmail();
            AppUser.AddressLocation = new Geometry.Point(0, 0);
            if (!string.IsNullOrEmpty(LangCode))
            {
                AppUser.LangCode = LangCode;
            }

            string pwd, salt;
            EncodePassword(Password, out pwd, out salt);
            AppUser.Password = pwd;
            AppUser.PasswordSalt = salt;

            try
            {
                AppUser.Save();

                if (!AppUser.IsVerified)
                {
                    try
                    {
                        string key = GenerateRecoveryKey(AppUser.Email);
                        EmailMessagingService.SendWelcomeMailWithVerificationForAppUser(AppUser, key, null);
                    }
                    catch { }
                }

                return AppUserCreateResults.Success;
            }
            catch
            {
                AppUser = null;
                return AppUserCreateResults.UnknownError;
            }
        }
        public static AppUserFacebookConnectResults ConnectAppUserToFacebook(string accessToken, out AppUser AppUser)
        {
            AppUser = null;
            if (accessToken == "") return AppUserFacebookConnectResults.LoginError;

            try
            {
                Int64 foundFacebookId = 0;
                string firstName = string.Empty, lastName = string.Empty, email = string.Empty, birthday = string.Empty;
                AppUserGender gender = AppUserGender.Unknown;
                try
                {
                    FacebookClient fbClient = new FacebookClient(accessToken);
                    dynamic result = fbClient.Get("me?fields=id,name,email,first_name,last_name,gender");
                    foundFacebookId = Convert.ToInt64(result.id);
                    firstName = result.first_name;
                    lastName = result.last_name;
                    email = result.email;
                    birthday = result.birth_day as string;

                    switch (result.gender as string)
                    {
                        case @"male":
                            gender = AppUserGender.Male;
                            break;
                        case @"female":
                            gender = AppUserGender.Female;
                            break;
                        default:
                            gender = AppUserGender.Unknown;
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(@"Facebook error: " + ex.ToString());
                }

                if (foundFacebookId != 0)
                {
                    if (email!=null)
                        AppUser = AppUser.FetchByEmail(email);
                    if (AppUser == null) AppUser = AppUser.FetchByFacebookId(foundFacebookId);
                    if (AppUser == null) AppUser = new AppUser();

                    AppUser.IsVerified = true;
                    AppUser.FacebookId = foundFacebookId;
                    AppUser.FacebookToken = accessToken;
                    AppUser.LastLogin = DateTime.UtcNow;
                    AppUser.BadLoginTries = 0;
                    if (AppUser.IsNewRecord)
                    {
                        DateTime? birthdate = null;
                        if (!string.IsNullOrEmpty(birthday))
                        {
                            DateTime dt;
                            if (DateTime.TryParseExact(birthday, @"MM/dd/yyyy", null, DateTimeStyles.AssumeLocal, out dt))
                            {
                                birthdate = dt;
                            }
                        }

                        AppUser.FirstName = firstName;
                        AppUser.LastName = lastName;
                        AppUser.ProfileImage = @"fb:";
                    }
                    if (!string.IsNullOrEmpty(email))
                    {
                        if (string.IsNullOrEmpty(AppUser.Email)) AppUser.Email = email;
                        AppUser.UniqueIdString = email.NormalizeEmail();
                    }
                    try
                    {
                        Query.New<AppUser>()
                            .Update(AppUser.Columns.FacebookId, null)
                            .Where(AppUser.Columns.FacebookId, foundFacebookId)
                            .Execute();

                        //TODO - real address location
                        AppUser.AddressLocation = new Geometry.Point(0, 0);
                        AppUser.Save();

                        return AppUserFacebookConnectResults.Success;
                    }
                    catch
                    {
                        AppUser = null;
                        return AppUserFacebookConnectResults.LoginError;
                    }
                }
                else
                {
                    return AppUserFacebookConnectResults.LoginError;
                }
            }
            catch
            {
                return AppUserFacebookConnectResults.LoginError;
            }
        }

        public static AppUserAuthenticateResults AuthenticateAppUser(string Email, string Password, out Int64 AppUserId)
        {
            AppUserId = 0;
            if (!Email.NormalizeEmail().IsValidEmail() || string.IsNullOrEmpty(Password)) return AppUserAuthenticateResults.LoginError;

            List<object> user = Query.New<AppUser>()
                .Select(AppUser.Columns.AppUserId)
                .AddSelect(AppUser.Columns.IsLocked)
                .AddSelect(AppUser.Columns.BadLoginTries)
                .AddSelect(AppUser.Columns.Password)
                .AddSelect(AppUser.Columns.PasswordSalt)
                .AddSelect(AppUser.Columns.IsVerified)
                .Where(AppUser.Columns.UniqueIdString, Email.NormalizeEmail())
                .AddWhere(AppUser.Columns.IsDeleted, false)
                .ExecuteOneRowToList();
            if (user == null)
            {
                user = null;
                return AppUserAuthenticateResults.NoMatch;
            }
            if (!Convert.ToBoolean(user[5]))
            {
                if (Settings.GetSettingBool(Settings.Keys.APPUSER_VERIFY_EMAIL, false))
                {
                    try
                    {
                        string key = GenerateRecoveryKey(Email);
                        EmailMessagingService.SendWelcomeMailWithVerificationForAppUser(AppUser.FetchByID(user[0]), key, null);
                    }
                    catch { }
                    user = null;
                    return AppUserAuthenticateResults.NotVerified;
                }
            }
            if (Convert.ToBoolean(user[1]))
            {
                user = null;
                return AppUserAuthenticateResults.Locked;
            }
            string comparePassword = EncodePassword(Password, user[4] as string);
            if (!comparePassword.Equals(user[3] as string, StringComparison.Ordinal))
            {
                if (MaximumBadLoginTries > 0)
                {
                    Int32 BadLoginTries = Convert.ToInt32(user[2]);
                    BadLoginTries++;
                    bool IsLocked = false;
                    if (BadLoginTries >= MaximumBadLoginTries)
                    {
                        IsLocked = true;
                    }
                    Query.New<AppUser>()
                        .Update(AppUser.Columns.BadLoginTries, BadLoginTries)
                        .Update(AppUser.Columns.IsLocked, IsLocked)
                        .Where(AppUser.Columns.AppUserId, user[0])
                        .Execute();
                }
                return AppUserAuthenticateResults.NoMatch;
            }
            AppUserId = Convert.ToInt64(user[0]);
            Query.New<AppUser>()
                .Update(AppUser.Columns.LastLogin, DateTime.UtcNow)
                .Update(AppUser.Columns.BadLoginTries, 0)
                .Where(AppUser.Columns.AppUserId, user[0])
                .Execute();
            return AppUserAuthenticateResults.Success;
        }

        public static AppUserAuthenticateResults AuthenticateAppSupplier(string Email, string Password, out Int64 AppSupplierId)
        {
            AppSupplierId = 0;
            if (!Email.NormalizeEmail().IsValidEmail() || string.IsNullOrEmpty(Password)) return AppUserAuthenticateResults.LoginError;

            List<object> user = Query.New<AppSupplier>()
                .Select(AppSupplier.Columns.SupplierId)
                .AddSelect(AppSupplier.Columns.IsLocked)
                .AddSelect(AppSupplier.Columns.BadLoginTries)
                .AddSelect(AppSupplier.Columns.Password)
                .AddSelect(AppSupplier.Columns.PasswordSalt)
                .AddSelect(AppSupplier.Columns.IsVerified)
                .Where(AppSupplier.Columns.UniqueIdString, Email.NormalizeEmail())
                .AddWhere(AppSupplier.Columns.IsDeleted, false)
                .ExecuteOneRowToList();
            if (user == null)
            {
                user = null;
                return AppUserAuthenticateResults.NoMatch;
            }
            if (!Convert.ToBoolean(user[5]))
            {
                if (Settings.GetSettingBool(Settings.Keys.APPUSER_VERIFY_EMAIL, false))
                {
                    try
                    {
                        string key = GenerateRecoveryKey(Email);
                        EmailMessagingService.SendWelcomeMailWithVerificationForAppSupplier(AppSupplier.FetchByID(user[0]), key, null);
                    }
                    catch { }
                    user = null;
                    return AppUserAuthenticateResults.NotVerified;
                }
            }
            string comparePassword = EncodePassword(Password, user[4] as string);
            if (!comparePassword.Equals(user[3] as string, StringComparison.Ordinal))
            {
                if (MaximumBadLoginTries > 0)
                {
                    Int32 BadLoginTries = Convert.ToInt32(user[2]);
                    BadLoginTries++;
                    bool IsLocked = false;
                    if (BadLoginTries >= MaximumBadLoginTries)
                    {
                        IsLocked = true;
                    }
                    Query.New<AppSupplier>()
                        .Update(AppSupplier.Columns.BadLoginTries, BadLoginTries)
                        .Update(AppSupplier.Columns.IsLocked, IsLocked)
                        .Where(AppSupplier.Columns.SupplierId, user[0])
                        .Execute();
                }
                return AppUserAuthenticateResults.NoMatch;
            }
            AppSupplierId = Convert.ToInt64(user[0]);
            Query.New<AppSupplier>()
                .Update(AppSupplier.Columns.LastLogin, DateTime.UtcNow)
                .Update(AppSupplier.Columns.BadLoginTries, 0)
                .Where(AppSupplier.Columns.SupplierId, user[0])
                .Execute();
            return AppUserAuthenticateResults.Success;
        }

        /// <summary>
        /// Call this when AppUser is logged in from a cookie, and need to update LastLogin time.
        /// Verifies also that AppUser is not locked
        /// This will NOT reset BadLoginTries
        /// This will return a AppUser object on success
        /// </summary>
        /// <param name="AppUserId">AppUserId</param>
        /// <param name="AppUser">returns a <typeparamref name="AppUser"/> object on success</param>
        /// <returns><typeparamref name="AppUser"/></returns>
        public static AppUserAuthenticateResults AppUserLoggedInAction(Int64 AppUserId)
        {
            List<object> user = Query.New<AppUser>()
                .Select(AppUser.Columns.AppUserId)
                .AddSelect(AppUser.Columns.IsLocked)
                .AddSelect(AppUser.Columns.LastLogin)
                .Where(AppUser.Columns.AppUserId, AppUserId)
                .AddWhere(AppUser.Columns.IsDeleted, false)
                .ExecuteOneRowToList();

            if (user == null) return AppUserAuthenticateResults.LoginError;
            if (Convert.ToBoolean(user[1]))
            {
                user = null;
                return AppUserAuthenticateResults.Locked;
            }

            DateTime PreviousLastLogin = user[2] == null ? DateTime.MinValue : Convert.ToDateTime(user[2]);

            Query.New<AppUser>()
                .Update(AppUser.Columns.LastLogin, DateTime.UtcNow)
                .Where(AppUser.Columns.AppUserId, user[0])
                .Execute();

            AppUserActivityController.UserLoggedIn(AppUserId, PreviousLastLogin);

            return AppUserAuthenticateResults.Success;
        }


        public static AppUserAuthenticateResults AppSupplierLoggedInAction(Int64 SupplierId, out List<object> Supplier)
        {
            List<object> user = Query.New<AppSupplier>()
                .Select(AppSupplier.Columns.SupplierId)
                .AddSelect(AppSupplier.Columns.IsLocked)
                .AddSelect(AppSupplier.Columns.LastLogin)
                .AddSelect(AppSupplier.Columns.Status)
                .AddSelect(AppSupplier.Columns.AllowChangeStatusJoinBid)
                .AddSelect(AppSupplier.Columns.StatusJoinBid)
                .AddSelect(AppSupplier.Columns.MaxWinningsNum)
                .AddSelect(AppSupplier.Columns.IsService)

                .Where(AppSupplier.Columns.SupplierId, SupplierId)
                .AddWhere(AppSupplier.Columns.IsDeleted, false)
                .ExecuteOneRowToList();
            
            Supplier = new List<object>();
            if (user == null) return AppUserAuthenticateResults.LoginError;   

            DateTime PreviousLastLogin = user[2] == null ? DateTime.MinValue : Convert.ToDateTime(user[2]);

            Query.New<AppSupplier>()
                .Update(AppSupplier.Columns.LastLogin, DateTime.UtcNow)
                .Where(AppSupplier.Columns.SupplierId, user[0])
                .Execute();

            
            Supplier.Add(user[3] != null ?  Convert.ToBoolean(user[3]) : false);
            Supplier.Add(user[4] != null ? Convert.ToBoolean(user[4]) : false);
            Supplier.Add(user[5] != null ? Convert.ToBoolean(user[5]) : false);
            Supplier.Add(user[6] != null ? Convert.ToInt32(user[6]) : 0);
            Supplier.Add(user[7] != null ? Convert.ToBoolean(user[7]) : false);

            //AppUserActivityController.UserLoggedIn(SupplierId, PreviousLastLogin);

            return AppUserAuthenticateResults.Success;
        }

        public static AppUserPasswordChangeResults ChangeAppUserPassword(string email, string newPassword)
        {
            AppUser AppUser = AppUser.FetchByEmail(email);
            if (AppUser == null) return AppUserPasswordChangeResults.AppUserDoesNotExist;
            AppUser.Password = EncodePassword(newPassword, AppUser.PasswordSalt);
            AppUser.Save();
            return AppUserPasswordChangeResults.Success;
        }
        public static AppUserPasswordChangeResults ChangeAppUserPassword(string email, string oldPassword, string newPassword)
        {
            AppUser AppUser = AppUser.FetchByEmail(email);
            if (AppUser == null) return AppUserPasswordChangeResults.AppUserDoesNotExist;
            string comparePassword = EncodePassword(oldPassword, AppUser.PasswordSalt);
            if (comparePassword != AppUser.Password)
            {
                return AppUserPasswordChangeResults.PasswordDoNotMatch;
            }
            AppUser.Password = EncodePassword(newPassword, AppUser.PasswordSalt);
            AppUser.Save();
            return AppUserPasswordChangeResults.Success;
        }
        public static string GenerateRecoveryKey(string email)
        {
            List<object> user = Query.New<AppUser>()
                .Select(AppUser.Columns.AppUserId)
                .AddSelect(AppUser.Columns.PasswordSalt)
                .Where(AppUser.Columns.Email, email)
                .ExecuteOneRowToList();

            if (user == null) return null;

            string key, salt = (string)user[1];
            if (string.IsNullOrEmpty(salt))
            {
                EncodePassword(email.RandomString(32), out key, out salt);
            }
            else
            {
                key = EncodePassword(email.RandomString(32), salt);
            }

            Query.New<AppUser>()
                .Update(AppUser.Columns.PasswordRecoveryKey, key)
                .Update(AppUser.Columns.PasswordRecoveryDate, DateTime.UtcNow)
                .Update(AppUser.Columns.PasswordSalt, salt)
                .Where(AppUser.Columns.AppUserId, user[0])
                .Execute();

            return key;
        }


        public static string GenerateRecoveryKeySupplier(string email)
        {
            List<object> user = Query.New<AppSupplier>()
                .Select(AppSupplier.Columns.SupplierId)
                .AddSelect(AppSupplier.Columns.PasswordSalt)
                .Where(AppSupplier.Columns.Email, email)
                .ExecuteOneRowToList();

            if (user == null) return null;

            string key, salt = (string)user[1];
            if (string.IsNullOrEmpty(salt))
            {
                EncodePassword(email.RandomString(32), out key, out salt);
            }
            else
            {
                key = EncodePassword(email.RandomString(32), salt);
            }

            Query.New<AppSupplier>()
                .Update(AppSupplier.Columns.PasswordRecoveryKey, key)
                .Update(AppSupplier.Columns.PasswordRecoveryDate, DateTime.UtcNow)
                .Update(AppSupplier.Columns.PasswordSalt, salt)
                .Where(AppSupplier.Columns.SupplierId, user[0])
                .Execute();

            return key;
        }
        public static AppUserRecoveryResults VerifyRecoveryKey(string email, string key)
        {
            return VerifyRecoveryKey(email, key, null);
        }
        public static AppUserRecoveryResults VerifyRecoveryKey(string email, string key, string newPassword)
        {
            AppUser user = AppUser.FetchByEmail(email);
            if (user == null) return AppUserRecoveryResults.AppUserDoesNotExist;

            if (user.PasswordRecoveryKey != key) return AppUserRecoveryResults.KeyDoNotMatch;

            if (user.PasswordRecoveryDate.AddHours(RecoveryKeyLifeInHours) < DateTime.UtcNow) return AppUserRecoveryResults.Expired;

            if (newPassword == null) return AppUserRecoveryResults.Success;
            else
            {
                string pwd, salt;
                EncodePassword(newPassword, out pwd, out salt);
                user.Password = pwd;
                user.PasswordSalt = salt;
                user.PasswordRecoveryKey = @"";
                user.PasswordRecoveryDate = DateTime.MinValue;
                user.IsVerified = true;
                user.IsLocked = false;
                user.Save();

                return AppUserRecoveryResults.Success;
            }
        }

        public static AppUserDeleteResults DeleteAppUser(Int64 AppUserId)
        {
            AppUser AppUser = AppUser.FetchByID(AppUserId);
            if (AppUser == null) return AppUserDeleteResults.AppUserDoesNotExist;
            AppUser.Delete(AppUser.Columns.AppUserId, AppUser.AppUserId);
            return AppUserDeleteResults.Success;
        }

        public static string EncodePassword(string pass, string salt)
        {
            byte[] src = Convert.FromBase64String(salt);
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] dst = new byte[src.Length + bytes.Length];
            byte[] inArray = null;
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create(@"SHA512");
            inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }
        public static void EncodePassword(string pass, out string encoded, out string salt)
        {
            byte[] saltData = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(saltData);
            salt = Convert.ToBase64String(saltData);
            encoded = EncodePassword(pass, salt);
        }
    }
}
