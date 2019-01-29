using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities.Serialization;
using Newtonsoft;
using Newtonsoft.Json;
using System.Collections;
using dg.Utilities;

/*
 * User
 * User
 * UserId:                  PRIMARY KEY; INT64; AUTOINCREMENT;
 * Email:                   FIXEDSTRING(64); Email
 * UniqueEmail:             FIXEDSTRING(64); Normalized email address
 * LastLogin:               DATETIME; DEFAULT DateTime.MinValue; Last login date/time
 * IsLocked:                BOOL; DEFAULT false; Is locked, due to deliberate locking or to password abuse
 * BadLoginTries:           DEFAULT 0; Number of recorded bad logins, since last successful login
 * Password:                FIXEDSTRING(128); Scrambled password
 * PasswordSalt:            FIXEDSTRING(64); Password salt
 * PasswordRecoveryKey:     FIXEDSTRING(128); DEFAULT string.Empty; Password recovery key
 * PasswordRecoveryDate:    DATETIME; DEFAULT DateTime.MinValue; The date that the password recovery request was issued
 * @INDEX:                  NAME(ix_User_UniqueEmail); [UniqueEmail]; UNIQUE;
 * */

namespace Snoopi.core.DAL
{
    public partial class UserCollection : AbstractRecordList<User, UserCollection>
    {
    }

    public partial class User : AbstractRecord<User>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string UserId = "UserId";
            public static string Email = "Email"; // Email
            public static string UniqueEmail = "UniqueEmail"; // Normalized email address
            public static string LastLogin = "LastLogin"; // Last login date/time
            public static string IsLocked = "IsLocked"; // Is locked, due to deliberate locking or to password abuse
            public static string BadLoginTries = "BadLoginTries"; // Number of recorded bad logins, since last successful login
            public static string Password = "Password"; // Scrambled password
            public static string PasswordSalt = "PasswordSalt"; // Password salt
            public static string PasswordRecoveryKey = "PasswordRecoveryKey"; // Password recovery key
            public static string PasswordRecoveryDate = "PasswordRecoveryDate"; // The date that the password recovery request was issued
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"User";
                schema.AddColumn(Columns.UserId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Email, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.UniqueEmail, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.LastLogin, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.IsLocked, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.BadLoginTries, typeof(int), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.Password, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PasswordSalt, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PasswordRecoveryKey, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.PasswordRecoveryDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("ix_User_UniqueEmail", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.UniqueEmail);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _UserId = 0;
        internal string _Email = string.Empty;
        internal string _UniqueEmail = string.Empty;
        internal DateTime _LastLogin = DateTime.MinValue;
        internal bool _IsLocked = false;
        internal int _BadLoginTries = 0;
        internal string _Password = string.Empty;
        internal string _PasswordSalt = string.Empty;
        internal string _PasswordRecoveryKey = string.Empty;
        internal DateTime _PasswordRecoveryDate = DateTime.MinValue;
        #endregion

        #region Properties
        public Int64 UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        public string UniqueEmail
        {
            get { return _UniqueEmail; }
            set { _UniqueEmail = value; }
        }
        public DateTime LastLogin
        {
            get { return _LastLogin; }
            set { _LastLogin = value; }
        }
        public bool IsLocked
        {
            get { return _IsLocked; }
            set { _IsLocked = value; }
        }
        public int BadLoginTries
        {
            get { return _BadLoginTries; }
            set { _BadLoginTries = value; }
        }
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        public string PasswordSalt
        {
            get { return _PasswordSalt; }
            set { _PasswordSalt = value; }
        }
        public string PasswordRecoveryKey
        {
            get { return _PasswordRecoveryKey; }
            set { _PasswordRecoveryKey = value; }
        }
        public DateTime PasswordRecoveryDate
        {
            get { return _PasswordRecoveryDate; }
            set { _PasswordRecoveryDate = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return UserId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Email, Email);
            qry.Insert(Columns.UniqueEmail, UniqueEmail);
            qry.Insert(Columns.LastLogin, LastLogin);
            qry.Insert(Columns.IsLocked, IsLocked);
            qry.Insert(Columns.BadLoginTries, BadLoginTries);
            qry.Insert(Columns.Password, Password);
            qry.Insert(Columns.PasswordSalt, PasswordSalt);
            qry.Insert(Columns.PasswordRecoveryKey, PasswordRecoveryKey);
            qry.Insert(Columns.PasswordRecoveryDate, PasswordRecoveryDate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                UserId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Email, Email);
            qry.Update(Columns.UniqueEmail, UniqueEmail);
            qry.Update(Columns.LastLogin, LastLogin);
            qry.Update(Columns.IsLocked, IsLocked);
            qry.Update(Columns.BadLoginTries, BadLoginTries);
            qry.Update(Columns.Password, Password);
            qry.Update(Columns.PasswordSalt, PasswordSalt);
            qry.Update(Columns.PasswordRecoveryKey, PasswordRecoveryKey);
            qry.Update(Columns.PasswordRecoveryDate, PasswordRecoveryDate);
            qry.Where(Columns.UserId, UserId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            UserId = Convert.ToInt64(reader[Columns.UserId]);
            Email = (string)reader[Columns.Email];
            UniqueEmail = (string)reader[Columns.UniqueEmail];
            LastLogin = Convert.ToDateTime(reader[Columns.LastLogin]);
            IsLocked = Convert.ToBoolean(reader[Columns.IsLocked]);
            BadLoginTries = Convert.ToInt32(reader[Columns.BadLoginTries]);
            Password = (string)reader[Columns.Password];
            PasswordSalt = (string)reader[Columns.PasswordSalt];
            PasswordRecoveryKey = (string)reader[Columns.PasswordRecoveryKey];
            PasswordRecoveryDate = Convert.ToDateTime(reader[Columns.PasswordRecoveryDate]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static User FetchByID(Int64 UserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UserId, UserId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    User item = new User();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 UserId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UserId, UserId);
            return qry.Execute();
        }
        public static User FetchByID(ConnectorBase conn, Int64 UserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UserId, UserId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    User item = new User();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 UserId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UserId, UserId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class User
    {
        static public User FetchByEmail(string email)
        {
            using (DataReaderBase reader = new Query(TableSchema).Where(Columns.UniqueEmail, email.NormalizeEmail()).ExecuteReader())
            {
                if (reader.Read())
                {
                    User ret = new User();
                    ret.Read(reader);
                    return ret;
                }
            }
            return null;
        }

        static public string EmailForUserId(Int64 UserId)
        {
            object value = new Query(TableSchema).Select(Columns.Email).Where(Columns.UserId, UserId).ExecuteScalar();
            return value as string;
        }

        public string[] PermissionsForUser()
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
    }
}
