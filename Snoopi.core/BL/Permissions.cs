using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Utilities;
using Snoopi.core.DAL;
using System.Collections;
using dg.Sql.Connector;

/* 
 * -- Permissions --
 * -----------------
 * sys_view_users
 * sys_edit_users
 * sys_view_appusers
 * sys_edit_appusers
 * sys_settings
 * sys_backups
 * sys_view_emails
 * sys_edit_emails
 * */

namespace Snoopi.core.BL
{
    public static class Permissions
    {
        public static bool UserHasPermission(Int64 UserId, string PermissionKey)
        {
            return new Query(UserPermissionMap.TableSchema)
                .Join(JoinType.InnerJoin, Permission.TableSchema, @"map", new JoinColumnPair(UserPermissionMap.TableSchema, UserPermissionMap.Columns.PermissionId, Permission.Columns.PermissionId))
                .Where(UserPermissionMap.TableSchema.SchemaName, UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId)
                .AND(@"map", Permission.Columns.Key, WhereComparision.EqualsTo, PermissionKey)
                .GetCount(UserPermissionMap.TableSchema.SchemaName, UserPermissionMap.Columns.PermissionId) > 0;
        }
        public static bool UserHasAnyPermissionIn(Int64 UserId, params string[] PermissionKeys)
        {
            if (PermissionKeys.Length == 0) return true;
            return new Query(UserPermissionMap.TableSchema)
                .Join(JoinType.InnerJoin, Permission.TableSchema, @"map", new JoinColumnPair(UserPermissionMap.TableSchema, UserPermissionMap.Columns.PermissionId, Permission.Columns.PermissionId))
                .Where(UserPermissionMap.TableSchema.SchemaName, UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId)
                .AND(@"map", Permission.Columns.Key, WhereComparision.In, PermissionKeys)
                .GetCount(UserPermissionMap.TableSchema.SchemaName, UserPermissionMap.Columns.PermissionId) > 0;
        }
        public static bool UserHasAllPermissionIn(Int64 UserId, params string[] PermissionKeys)
        {
            if (PermissionKeys.Length == 0) return true;
            PermissionKeys = PermissionKeys.GetUniqueArray(); // DB won't count duplicates, we won't either.
            return new Query(UserPermissionMap.TableSchema)
                .Join(JoinType.InnerJoin, Permission.TableSchema, @"map", new JoinColumnPair(UserPermissionMap.TableSchema, UserPermissionMap.Columns.PermissionId, Permission.Columns.PermissionId))
                .Where(UserPermissionMap.TableSchema.SchemaName, UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId)
                .AND(@"map", Permission.Columns.Key, WhereComparision.In, PermissionKeys)
                .GetCount(UserPermissionMap.TableSchema.SchemaName, UserPermissionMap.Columns.PermissionId) == PermissionKeys.Length;
        }
        public static bool UserLessPriviledgedThanOtherUser(Int64 UserId, Int64 OtherUserId)
        {
            Int64 countMissing = new Query(UserPermissionMap.TableSchema)
                .Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, OtherUserId)
                .AND(UserPermissionMap.Columns.PermissionId, WhereComparision.NotIn,
                    new Query(UserPermissionMap.TableSchema)
                    .Select(UserPermissionMap.Columns.PermissionId)
                    .Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId)
                )
                .GetCount(UserPermissionMap.Columns.PermissionId);
            if (countMissing > 0)
            { // OtherUserId has permissions not in UserId
                return true;
            }
            else
            {
                countMissing = new Query(UserPermissionMap.TableSchema)
                    .Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId)
                    .AND(UserPermissionMap.Columns.PermissionId, WhereComparision.NotIn,
                        new Query(UserPermissionMap.TableSchema)
                        .Select(UserPermissionMap.Columns.PermissionId)
                        .Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, OtherUserId)
                    )
                    .GetCount(UserPermissionMap.Columns.PermissionId);
                if (countMissing == 0)
                { // Same permissions
                    return false;
                }
                else
                { // UserId has permissions not in OtherUserId
                    return false;
                }
            }
        }
        public static bool UserMorePriviledgedThanOtherUser(Int64 UserId, Int64 OtherUserId)
        {
            Int64 countMissing = new Query(UserPermissionMap.TableSchema)
                .Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, OtherUserId)
                .AND(UserPermissionMap.Columns.PermissionId, WhereComparision.NotIn,
                    new Query(UserPermissionMap.TableSchema)
                    .Select(UserPermissionMap.Columns.PermissionId)
                    .Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId)
                )
                .GetCount(UserPermissionMap.Columns.PermissionId);
            if (countMissing == 0)
            {
                Int64 countUserId = new Query(UserPermissionMap.TableSchema).Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId).GetCount(UserPermissionMap.Columns.PermissionId);
                Int64 countOtherUserId = new Query(UserPermissionMap.TableSchema).Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, OtherUserId).GetCount(UserPermissionMap.Columns.PermissionId);
                if (countUserId == countOtherUserId)
                {
                    return false;
                }
                else if (countUserId > countOtherUserId)
                {
                    // UserId has more permissions than in OtherUserId
                    return true;
                }
                else
                { // Won't actually happen. We already determined that countUserId >= countOtherUserId
                    return false;
                }
            }
            else
            { // OtherUserId has permissions not in UserId
                return false;
            }
        }
        public static bool UserMoreOrSamePriviledgedThanOtherUser(Int64 UserId, Int64 OtherUserId)
        {
            Int64 countMissing = new Query(UserPermissionMap.TableSchema)
                .Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, OtherUserId)
                .AND(UserPermissionMap.Columns.PermissionId, WhereComparision.NotIn,
                    new Query(UserPermissionMap.TableSchema)
                    .Select(UserPermissionMap.Columns.PermissionId)
                    .Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId)
                )
                .GetCount(UserPermissionMap.Columns.PermissionId);
            if (countMissing == 0)
            {
                Int64 countUserId = new Query(UserPermissionMap.TableSchema).Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId).GetCount(UserPermissionMap.Columns.PermissionId);
                Int64 countOtherUserId = new Query(UserPermissionMap.TableSchema).Where(UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, OtherUserId).GetCount(UserPermissionMap.Columns.PermissionId);
                if (countUserId == countOtherUserId)
                {
                    return true;
                }
                else if (countUserId > countOtherUserId)
                {
                    // UserId has more permissions than in OtherUserId
                    return true;
                }
                else
                { // Won't actually happen. We already determined that countUserId >= countOtherUserId
                    return false;
                }
            }
            else
            { // OtherUserId has permissions not in UserId
                return false;
            }
        }

        public static string[] PermissionsForUser(Int64 UserId)
        {
            Query qry = new Query(UserPermissionMap.TableSchema)
                .Select(Permission.TableSchema.SchemaName, Permission.Columns.Key, null, true)
                .Join(JoinType.InnerJoin, Permission.TableSchema, Permission.TableSchema.SchemaName, new JoinColumnPair(
                        UserPermissionMap.TableSchema, UserPermissionMap.Columns.PermissionId, Permission.Columns.PermissionId
                    ))
                .Where(UserPermissionMap.TableSchema.SchemaName, UserPermissionMap.Columns.UserId, WhereComparision.EqualsTo, UserId);
            WhereList wl = new WhereList();
            ArrayList permissions = new ArrayList();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    permissions.Add(reader.GetString(0));
                }
            }
            return (string[])permissions.ToArray(typeof(string));
        }
        public static Int32 PermissionIdByKey(string permission)
        {
            Query qry = new Query(Permission.TableSchema)
                .Select(Permission.Columns.PermissionId)
                .Where(Permission.Columns.Key, permission);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
            return 0;
        }

        public struct PermissionKeys
        {
            //public static string sys_view_users = @"sys_view_users";
            //public static string sys_edit_users = @"sys_edit_users";
            //public static string sys_view_appusers = @"sys_view_appusers";
            //public static string sys_edit_appusers = @"sys_edit_appusers";
            //public static string sys_settings = @"sys_settings";
            //public static string sys_backups = @"sys_backups";
            //public static string sys_view_emails = @"sys_view_emails";
            //public static string sys_edit_emails = @"sys_edit_emails";
            public static string sys_perm = @"sys_perm";
        }

        public static string[] SystemPermissionKeys = new string[] {
            //PermissionKeys.sys_view_users,
            //PermissionKeys.sys_edit_users,
            //PermissionKeys.sys_view_appusers,
            //PermissionKeys.sys_edit_appusers,
            //PermissionKeys.sys_settings,
            //PermissionKeys.sys_backups,
            //PermissionKeys.sys_view_emails,
            //PermissionKeys.sys_edit_emails,
            PermissionKeys.sys_perm,
        };
    }
}
