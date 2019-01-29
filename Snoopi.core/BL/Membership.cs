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
using Snoopi.core.DAL.Entities;

namespace Snoopi.core.BL
{
    public static class Membership
    {
        public enum UserCreateResults
        {
            Success,
            InvalidEmailAddress,
            AlreadyExists,
            UnknownError
        }
        public enum UserAuthenticateResults
        {
            Success,
            LoginError,
            NoMatch,
            Locked
        }
        public enum UserPasswordChangeResults
        {
            Success,
            PasswordDoNotMatch,
            UserDoesNotExist
        }
        public enum UserDeleteResults
        {
            Success,
            PasswordDoNotMatch,
            UserDoesNotExist
        }
        public enum UserRecoveryResults
        {
            Success,
            KeyDoNotMatch,
            Expired,
            UserDoesNotExist
        }
        public enum UserPermissionChangeResults
        {
            Success,
            NoPermission,
            UserMorePrivilegable,
            UserDoesNotExist
        }
        public enum AccessMode
        {
            None,
            ReadOnly,
            ReadWrite,
        }

        public static int MaximumBadLoginTries = AppConfig.GetInt32(@"Authentication.MaximumBadLoginTries.UserId", 9);
        public static int RecoveryKeyLifeInHours = AppConfig.GetInt32(@"Authentication.RecoveryKeyLifeInHours.UserId", 72);

        public static UserCollection GetUserList(bool includeLocked, int limit, int offset)
        {
            Query qry = new Query(User.TableSchema);
            if (!includeLocked) qry.AND(User.Columns.IsLocked, false);
            qry.OrderBy(User.Columns.Email, SortDirection.ASC);
            qry.LimitRows(limit).OffsetRows(offset);
            return UserCollection.FetchByQuery(qry);
        }
        public static UserCreateResults CreateUser(string email, string password, out User user)
        {
            user = null;
            if (!email.IsValidEmail()) return UserCreateResults.InvalidEmailAddress;
            user = User.FetchByEmail(email);
            if (user != null)
            {
                return UserCreateResults.AlreadyExists;
            }
            user = new User();
            user.Email = email;
            user.UniqueEmail = email.NormalizeEmail();

            string pwd, salt;
            EncodePassword(password, out pwd, out salt);
            user.Password = pwd;
            user.PasswordSalt = salt;

            try
            {
                user.Save();
                UserProfile userProfile = new UserProfile();
                userProfile.UserId = user.UserId;
                userProfile.DefaultLangCode = "he-IL";
                userProfile.Save();
                UserPermissionMap upm = new UserPermissionMap();
                upm.UserId = user.UserId;
                upm.PermissionId = 10;
                upm.Save();
                return UserCreateResults.Success;
            }
            catch
            {
                user = null;
                return UserCreateResults.UnknownError;
            }
        }

        public static UserCreateResults TestUser(string email, string password, out User user)
        {
            user = null;
            if (!email.IsValidEmail()) return UserCreateResults.InvalidEmailAddress;
            user = User.FetchByEmail(email);
            if (user != null)
            {
                return UserCreateResults.AlreadyExists;
            }
            user = new User();
            user.Email = email;
            user.UniqueEmail = email.NormalizeEmail();

            string pwd, salt;
            EncodePassword(password, out pwd, out salt);
            user.Password = pwd;
            user.PasswordSalt = salt;

            try
            {
                user.Save();
                UserProfile userProfile = new UserProfile();
                userProfile.UserId = user.UserId;
                userProfile.DefaultLangCode = "he-IL";
                userProfile.Save();
                UserPermissionMap upm = new UserPermissionMap();
                upm.UserId = user.UserId;
                upm.PermissionId = 10;
                upm.Save();
                return UserCreateResults.Success;
            }
            catch
            {
                user = null;
                return UserCreateResults.UnknownError;
            }
        }
        public static UserCreateResults CreateSupplier(string email, string password, Int64 CityId, out AppSupplier supplier)
        {
            supplier = null;
            if (!email.IsValidEmail()) return UserCreateResults.InvalidEmailAddress;
            supplier = AppSupplier.FetchByEmail(email);
            if (supplier != null)
            {
                return UserCreateResults.AlreadyExists;
            }
            supplier = new AppSupplier();
            supplier.Email = email;
            supplier.UniqueIdString = email.NormalizeEmail();

            string pwd, salt;
            EncodePassword(password, out pwd, out salt);
            supplier.Password = pwd;
            supplier.PasswordSalt = salt;

            try
            {
                supplier.AddressLocation = new Geometry.Point(0, 0);//TODO
                supplier.CityId = CityId;
                supplier.Save();
                return UserCreateResults.Success;
            }
            catch
            {
                supplier = null;
                return UserCreateResults.UnknownError;
            }
        }

        public static UserCreateResults CreateAdCompany(string email, out AdCompany company)
        {
            company = null;
            if (!email.IsValidEmail()) return UserCreateResults.InvalidEmailAddress;
            company = AdCompany.FetchByEmail(email);
            if (company != null)
            {
                return UserCreateResults.AlreadyExists;
            }
            company = new AdCompany();
            company.Email = email;

            try
            {

                company.Save();
                return UserCreateResults.Success;
            }
            catch
            {
                company = null;
                return UserCreateResults.UnknownError;
            }
        }

        public static bool IsUserEmailNotRegistered(string email)
        {
            Query qry = new Query(User.TableSchema).Where(User.Columns.UniqueEmail, email.NormalizeEmail()).LimitRows(1);
            return qry.GetCount(User.Columns.UserId) == 0;
        }
        public static string UserFirstNameOrEmail(Int64 UserId)
        {
            object ret = Query.New<UserProfile>().Select(UserProfile.Columns.FirstName).Where(UserProfile.Columns.UserId, UserId).ExecuteScalar();
            if (ret == null)
            {
                ret = Query.New<User>().Select(User.Columns.Email).Where(User.Columns.UserId, UserId).ExecuteScalar();
            }
            return ret as string;
        }

        public static UserAuthenticateResults AuthenticateUser(string Email, string Password, out Int64 UserId)
        {
            UserId = 0;
            List<object> user = Query.New<User>()
                .Select(User.Columns.UserId)
                .AddSelect(User.Columns.IsLocked)
                .AddSelect(User.Columns.BadLoginTries)
                .AddSelect(User.Columns.Password)
                .AddSelect(User.Columns.PasswordSalt)
                .Where(User.Columns.UniqueEmail, Email.NormalizeEmail())
                .ExecuteOneRowToList();
            if (user == null)
            {
                user = null;
                return UserAuthenticateResults.NoMatch;
            }
            if (Convert.ToBoolean(user[1]))
            {
                user = null;
                return UserAuthenticateResults.Locked;
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
                    Query.New<User>()
                        .Update(User.Columns.BadLoginTries, BadLoginTries)
                        .Update(User.Columns.IsLocked, IsLocked)
                        .Where(User.Columns.UserId, user[0])
                        .Execute();
                }
                return UserAuthenticateResults.NoMatch;
            }
            UserId = Convert.ToInt64(user[0]);
            Query.New<User>()
                .Update(User.Columns.LastLogin, DateTime.UtcNow)
                .Update(User.Columns.BadLoginTries, 0)
                .Where(User.Columns.UserId, user[0])
                .Execute();
            return UserAuthenticateResults.Success;
        }

        public static UserAuthenticateResults AuthenticateSupplier(string Email, string Password, out Int64 SupplierId)
        {
            SupplierId = 0;
            List<object> supplier = Query.New<AppSupplier>()
                .Select(AppSupplier.Columns.SupplierId)
                .AddSelect(AppSupplier.Columns.IsLocked)
                .AddSelect(AppSupplier.Columns.BadLoginTries)
                .AddSelect(AppSupplier.Columns.Password)
                .AddSelect(AppSupplier.Columns.PasswordSalt)
                .Where(AppSupplier.Columns.Email, Email.NormalizeEmail())
                .AddWhere(AppSupplier.Columns.IsDeleted, false)
                .ExecuteOneRowToList();
            if (supplier == null)
            {
                supplier = null;
                return UserAuthenticateResults.NoMatch;
            }
            
            string comparePassword = EncodePassword(Password, supplier[4] as string);
            if (!comparePassword.Equals(supplier[3] as string, StringComparison.Ordinal))
            {
                if (MaximumBadLoginTries > 0)
                {
                    Int32 BadLoginTries = Convert.ToInt32(supplier[2]);
                    BadLoginTries++;
                    bool IsLocked = false;
                    if (BadLoginTries >= MaximumBadLoginTries)
                    {
                        IsLocked = true;
                    }
                    Query.New<AppSupplier>()
                        .Update(AppSupplier.Columns.BadLoginTries, BadLoginTries)
                        .Update(AppSupplier.Columns.IsLocked, IsLocked)
                        .Where(AppSupplier.Columns.SupplierId, supplier[0])
                        .Execute();
                }
                return UserAuthenticateResults.NoMatch;
            }
            SupplierId = Convert.ToInt64(supplier[0]);
            Query.New<AppSupplier>()
                .Update(AppSupplier.Columns.LastLogin, DateTime.UtcNow)
                .Update(AppSupplier.Columns.BadLoginTries, 0)
                .Where(AppSupplier.Columns.SupplierId, supplier[0])
                .Execute();
            return UserAuthenticateResults.Success;
        }

        /// <summary>
        /// Call this when user is logged in from a cookie, and need to update LastLogin time.
        /// Verifies also that user is not locked
        /// This will NOT reset BadLoginTries
        /// This will return a User object on success
        /// </summary>
        /// <param name="UserId">UserId</param>
        /// <param name="user">returns a <typeparamref name="User"/> object on success</param>
        /// <returns><typeparamref name="User"/></returns>
        public static UserAuthenticateResults UserLoggedInAction(Int64 UserId)
        {
            List<object> user = Query.New<User>()
                .Select(User.Columns.UserId)
                .AddSelect(User.Columns.IsLocked)
                .Where(User.Columns.UserId, UserId)
                .ExecuteOneRowToList();

            if (user == null) return UserAuthenticateResults.LoginError;
            if (Convert.ToBoolean(user[1]))
            {
                user = null;
                return UserAuthenticateResults.Locked;
            }
            Query.New<User>()
                .Update(User.Columns.LastLogin, DateTime.UtcNow)
                .Where(User.Columns.UserId, user[0])
                .Execute();
            return UserAuthenticateResults.Success;
        }

        public static UserAuthenticateResults SupplierLoggedInAction(Int64 SupplierId)
        {
            List<object> supplier = Query.New<AppSupplier>()
                .Select(AppSupplier.Columns.SupplierId)
                .AddSelect(AppSupplier.Columns.IsLocked)
                .Where(AppSupplier.Columns.SupplierId, SupplierId)
                .ExecuteOneRowToList();

            if (supplier == null) return UserAuthenticateResults.LoginError;
            if (Convert.ToBoolean(supplier[1]))
            {
                supplier = null;
                return UserAuthenticateResults.Locked;
            }
            Query.New<AppSupplier>()
                .Update(AppSupplier.Columns.LastLogin, DateTime.UtcNow)
                .Where(AppSupplier.Columns.SupplierId, supplier[0])
                .Execute();
            return UserAuthenticateResults.Success;
        }

        public static UserPasswordChangeResults ChangeUserPassword(string email, string newPassword)
        {
            User user = User.FetchByEmail(email);
            if (user == null) return UserPasswordChangeResults.UserDoesNotExist;
            if (string.IsNullOrEmpty(user.PasswordSalt))
            {
                string pass, salt;
                EncodePassword(newPassword, out pass, out salt);
                user.Password = pass;
                user.PasswordSalt = salt;
            }
            else
            {
                user.Password = EncodePassword(newPassword, user.PasswordSalt);
            }
            user.Save();
            return UserPasswordChangeResults.Success;
        }
        public static UserPasswordChangeResults ChangeSupplierPassword(string email, string newPassword)
        {
            AppSupplier supplier = AppSupplier.FetchByEmail(email);
            if (supplier == null) return UserPasswordChangeResults.UserDoesNotExist;
            if (string.IsNullOrEmpty(supplier.PasswordSalt))
            {
                string pass, salt;
                EncodePassword(newPassword, out pass, out salt);
                supplier.Password = pass;
                supplier.PasswordSalt = salt;
            }
            else
            {
                supplier.Password = EncodePassword(newPassword, supplier.PasswordSalt);
            }
            supplier.Save();
            return UserPasswordChangeResults.Success;
        }
        public static UserPasswordChangeResults ChangeUserPassword(string email, string oldPassword, string newPassword)
        {
            User user = User.FetchByEmail(email);
            if (user == null) return UserPasswordChangeResults.UserDoesNotExist;
            string comparePassword = EncodePassword(oldPassword, user.PasswordSalt);
            if (comparePassword != user.Password)
            {
                return UserPasswordChangeResults.PasswordDoNotMatch;
            }
            user.Password = EncodePassword(newPassword, user.PasswordSalt);
            user.Save();
            return UserPasswordChangeResults.Success;
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

        public static string GenerateRecoveryKey(string email)
        {
            List<object> user = Query.New<User>()
                .Select(User.Columns.UserId)
                .AddSelect(User.Columns.PasswordSalt)
                .Where(User.Columns.Email, email)
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

            Query.New<User>()
                .Update(User.Columns.PasswordRecoveryKey, key)
                .Update(User.Columns.PasswordRecoveryDate, DateTime.UtcNow)
                .Update(User.Columns.PasswordSalt, salt)
                .Where(User.Columns.UserId, user[0])
                .Execute();

            return key;
        }
        public static UserRecoveryResults VerifyRecoveryKey(string email, string key)
        {
            return VerifyRecoveryKey(email, key, null);
        }

        public static UserRecoveryResults SupplierVerifyRecoveryKey(string email, string key, string newPassword)
        {
            AppSupplier user = AppSupplier.FetchByEmail(email);
            if (user == null) return UserRecoveryResults.UserDoesNotExist;

            if (user.PasswordRecoveryKey != key) return UserRecoveryResults.KeyDoNotMatch;

            if (user.PasswordRecoveryDate.AddHours(RecoveryKeyLifeInHours) < DateTime.UtcNow) return UserRecoveryResults.Expired;

            if (newPassword == null) return UserRecoveryResults.Success;
            else
            {
                string pwd, salt;
                EncodePassword(newPassword, out pwd, out salt);
                user.Password = pwd;
                user.PasswordSalt = salt;
                user.PasswordRecoveryKey = @"";
                user.PasswordRecoveryDate = DateTime.UtcNow;
                user.IsLocked = false;
                user.Save();

                return UserRecoveryResults.Success;
            }
        }

        public static UserRecoveryResults VerifyRecoveryKey(string email, string key, string newPassword)
        {
            User user = User.FetchByEmail(email);
            if (user == null) return UserRecoveryResults.UserDoesNotExist;

            if (user.PasswordRecoveryKey != key) return UserRecoveryResults.KeyDoNotMatch;

            if (user.PasswordRecoveryDate.AddHours(RecoveryKeyLifeInHours) < DateTime.UtcNow) return UserRecoveryResults.Expired;

            if (newPassword == null) return UserRecoveryResults.Success;
            else
            {
                string pwd, salt;
                EncodePassword(newPassword, out pwd, out salt);
                user.Password = pwd;
                user.PasswordSalt = salt;
                user.PasswordRecoveryKey = @"";
                user.PasswordRecoveryDate = DateTime.UtcNow;
                user.IsLocked = false;
                user.Save();

                return UserRecoveryResults.Success;
            }
        }
        public static UserDeleteResults DeleteUser(string email)
        {
            User user = User.FetchByEmail(email);
            if (user == null) return UserDeleteResults.UserDoesNotExist;
            User.Delete(User.Columns.UserId, user.UserId);
            return UserDeleteResults.Success;
        }
        public static UserDeleteResults DeleteUser(string email, string oldPassword)
        {
            User user = User.FetchByEmail(email);
            if (user == null) return UserDeleteResults.UserDoesNotExist;
            string comparePassword = EncodePassword(oldPassword, user.PasswordSalt);
            if (comparePassword != user.Password)
            {
                return UserDeleteResults.PasswordDoNotMatch;
            }
            User.Delete(User.Columns.UserId, user.UserId);
            return UserDeleteResults.Success;
        }
        public static UserDeleteResults DeleteUser(Int64 UserId)
        {
            User user = User.FetchByID(UserId);
            if (user == null) return UserDeleteResults.UserDoesNotExist;
            User.Delete(User.Columns.UserId, user.UserId);
            return UserDeleteResults.Success;
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

        public static bool UserCanAffectUser(Int64 UserId, Int64 OtherUserId)
        {
            if (UserId == OtherUserId) return true;

            if (!Permissions.UserHasAnyPermissionIn(UserId, Permissions.PermissionKeys.sys_perm)) return false;
            string[] permissions = Permissions.PermissionsForUser(UserId);
            if (!permissions.Contains(Permissions.PermissionKeys.sys_perm)) return false;
            return true;
        }

        /// <summary>
        /// Adds permission to user
        /// Checks permission levels
        /// </summary>
        /// <param name="CurrentUser">Current user for validation, or null</param>
        /// <param name="PermissionId">PermissionId of target permission</param>
        /// <param name="UserId">UserId of target user</param>
        /// <returns><typeparamref name="UserRoleChangeResults"/></returns>
        public static UserPermissionChangeResults AddPermissionToUser(User CurrentUser, Int32 PermissionId, Int64 UserId)
        {
            User User = User.FetchByID(UserId);
            if (User == null) return UserPermissionChangeResults.UserDoesNotExist;
            if (CurrentUser != null)
            {
                bool skip = false;
                if (CurrentUser.UserId == UserId)
                {
                    // Self user
                    skip = true;
                }
                if (skip)
                {
                    string[] permissions = Permissions.PermissionsForUser(CurrentUser.UserId);
                    if (!permissions.Contains(Permissions.PermissionKeys.sys_perm)) return UserPermissionChangeResults.NoPermission;
                }
            }
            Query qry = new Query(UserPermissionMap.TableSchema)
                .Where(UserPermissionMap.Columns.PermissionId, PermissionId)
                .AND(UserPermissionMap.Columns.UserId, UserId);
            if (qry.GetCount(UserPermissionMap.Columns.PermissionId) == 0)
            {
                try
                {
                    UserPermissionMap map = new UserPermissionMap();
                    map.PermissionId = PermissionId;
                    map.UserId = UserId;
                    map.Save();
                }
                catch (DbException)
                {
                	// Ignore this. Must have been that the record exists already, because of simultaneous action.
                }
            }
            return UserPermissionChangeResults.Success;
        }

        /// <summary>
        /// Removes permission from user
        /// Checks permission levels
        /// Prevents changing own permissions if needed
        /// </summary>
        /// <param name="CurrentUser">Current user for validation, or null</param>
        /// <param name="PermissionId">PermissionId of target permission</param>
        /// <param name="UserId">UserId of target user</param>
        /// <returns><typeparamref name="UserRoleChangeResults"/></returns>
        public static UserPermissionChangeResults RemovePermissionFromUser(User CurrentUser, Int32 PermissionId, Int64 UserId)
        {
            User User = User.FetchByID(UserId);
            if (User == null) return UserPermissionChangeResults.UserDoesNotExist;
            if (CurrentUser != null)
            {
                if (CurrentUser.UserId == UserId)
                {
                    // Self user
                    return UserPermissionChangeResults.Success; // Do not actually remove permissions... Let them keep it!
                }

                string[] permissions = Permissions.PermissionsForUser(CurrentUser.UserId);
                if (!permissions.Contains(Permissions.PermissionKeys.sys_perm)) return UserPermissionChangeResults.NoPermission;
            }
            UserPermissionMap.Delete(UserId, PermissionId);
            return UserPermissionChangeResults.Success;
        }
    }
}
