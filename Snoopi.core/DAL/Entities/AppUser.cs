using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * AppUser
 * AppUser
 * AppUserId:               PRIMARY KEY; INT64; AUTOINCREMENT;
 * Email:                   FIXEDSTRING(64); Email
 * UniqueIdString:          FIXEDSTRING(64); NULLABLE; Normalized email address, or other ID means
 * LastLogin:               DATETIME; DEFAULT DateTime.MinValue; Last login date/time
 * CreateDate:              DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow; The date that the user was created
 * IsLocked:                BOOL; DEFAULT false; Is locked, due to deliberate locking or to password abuse
 * IsVerified:              BOOL; DEFAULT true; Is this user verified
 * BadLoginTries:           DEFAULT 0; Number of recorded bad logins, since last successful login
 * FacebookId:              INT64; NULLABLE; DEFAULT null; Facebook id
 * FacebookToken:           FIXEDSTRING(255); Facebook token
 * Password:                FIXEDSTRING(128); Scrambled password
 * PasswordSalt:            FIXEDSTRING(64); Password salt
 * PasswordRecoveryKey:     FIXEDSTRING(128); DEFAULT string.Empty; Password recovery key
 * PasswordRecoveryDate:    DATETIME; DEFAULT DateTime.MinValue; The date that the password recovery request was issued
 * LangCode:                FIXEDSTRING(16); DEFAULT string.Empty;
 * Gender:                  DEFAULT AppUserGender.Unknown; AppUserGender:
 *                                   "AppUserGender"
 *                                  - Unknown = 0
 *                                  - Male = 1
 *                                  - Female = 2
 * UnreadNotificationCount: INT32; DEFAULT 0;
 * ProfileImage:            FIXEDSTRING(255); DEFAULT string.Empty;
 * FirstName:               FIXEDSTRING(64); DEFAULT string.Empty;
 * LastName:                FIXEDSTRING(64); DEFAULT string.Empty;
 * HouseNum:                FIXEDSTRING(16); DEFAULT string.Empty;
 * Street:                  FIXEDSTRING(16); DEFAULT string.Empty;
 * CityId:                  INT64; DEFAULT null;
 * ApartmentNumber:         FIXEDSTRING(64); DEFAULT string.Empty;
 * Phone:                   FIXEDSTRING(64); DEFAULT string.Empty;
 * Floor:                   FIXEDSTRING(64); DEFAULT string.Empty;
 * AddressLocation:         POINT; DEFAULT null;
 * IsAdv:                   BOOL; DEFAULT false;
 * IsDeleted:               BOOL; DEFAULT false;
 * OrderDisplay:            INT(32); DEFAULT 0;
 * @INDEX:                  NAME(ix_AppUser_UniqueIdString); [UniqueIdString]; UNIQUE;
 * @INDEX:                  NAME(ix_AppUser_FacebookId); [FacebookId]; UNIQUE;
 * @INDEX:                  NAME(ix_AppUser_AddressLocation);SPATIAL;[AddressLocation]; 
 * */

namespace Snoopi.core.DAL
{
    public partial class AppUserCollection : AbstractRecordList<AppUser, AppUserCollection>
    {
    }

    public enum AppUserGender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
    }

    public partial class AppUser : AbstractRecord<AppUser>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public const string AppUserId = "AppUserId";
            public const string Email = "Email"; // Email
            public const string UniqueIdString = "UniqueIdString"; // Normalized email address, or other ID means
            public const string LastLogin = "LastLogin"; // Last login date/time
            public const string CreateDate = "CreateDate"; // The date that the user was created
            public const string IsLocked = "IsLocked"; // Is locked, due to deliberate locking or to password abuse
            public const string IsVerified = "IsVerified"; // Is this user verified
            public const string BadLoginTries = "BadLoginTries"; // Number of recorded bad logins, since last successful login
            public const string FacebookId = "FacebookId"; // Facebook id
            public const string FacebookToken = "FacebookToken"; // Facebook token
            public const string Password = "Password"; // Scrambled password
            public const string PasswordSalt = "PasswordSalt"; // Password salt
            public const string PasswordRecoveryKey = "PasswordRecoveryKey"; // Password recovery key
            public const string PasswordRecoveryDate = "PasswordRecoveryDate"; // The date that the password recovery request was issued
            public const string LangCode = "LangCode";
            public const string Gender = "Gender"; // AppUserGender
            public const string UnreadNotificationCount = "UnreadNotificationCount";
            public const string ProfileImage = "ProfileImage";
            public const string FirstName = "FirstName";
            public const string LastName = "LastName";
            public const string HouseNum = "HouseNum";
            public const string Street = "Street";
            public const string CityId = "CityId";
            public const string ApartmentNumber = "ApartmentNumber";
            public const string Phone = "Phone";
            public const string Floor = "Floor";
            public const string AddressLocation = "AddressLocation";
            public const string IsAdv = "IsAdv";
            public const string IsDeleted = "IsDeleted";
            public const string OrderDisplay = "OrderDisplay";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppUser";
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Email, typeof(string), 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.UniqueIdString, typeof(string), 64, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.LastLogin, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.IsLocked, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsVerified, typeof(bool), 0, 0, 0, false, false, false, true);
                schema.AddColumn(Columns.BadLoginTries, typeof(int), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.FacebookId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.FacebookToken, typeof(string), 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Password, typeof(string), 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PasswordSalt, typeof(string), 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PasswordRecoveryKey, typeof(string), 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.PasswordRecoveryDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.LangCode, typeof(string), 16, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Gender, typeof(AppUserGender), 0, 0, 0, false, false, false, AppUserGender.Unknown);
                schema.AddColumn(Columns.UnreadNotificationCount, typeof(Int32), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.ProfileImage, typeof(string), 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.FirstName, typeof(string), 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.LastName, typeof(string), 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.HouseNum, typeof(string), 16, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Street, typeof(string), 16, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.CityId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ApartmentNumber, typeof(string), 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Phone, typeof(string), 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Floor, typeof(string), 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.AddressLocation, typeof(Geometry.Point), DataType.Point, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.IsAdv, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsDeleted, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.OrderDisplay, typeof(int), 0, 0, 0, false, false, false, 0);

                _TableSchema = schema;

                schema.AddIndex("ix_AppUser_UniqueIdString", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.UniqueIdString);
                schema.AddIndex("ix_AppUser_FacebookId", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.FacebookId);
                schema.AddIndex("ix_AppUser_AddressLocation", TableSchema.ClusterMode.None, TableSchema.IndexMode.Spatial, TableSchema.IndexType.None, Columns.AddressLocation);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _AppUserId = 0;
        internal string _Email = string.Empty;
        internal string _UniqueIdString = null;
        internal DateTime _LastLogin = DateTime.MinValue;
        internal DateTime _CreateDate = DateTime.UtcNow;
        internal bool _IsLocked = false;
        internal bool _IsVerified = true;
        internal int _BadLoginTries = 0;
        internal Int64? _FacebookId = null;
        internal string _FacebookToken = string.Empty;
        internal string _Password = string.Empty;
        internal string _PasswordSalt = string.Empty;
        internal string _PasswordRecoveryKey = string.Empty;
        internal DateTime _PasswordRecoveryDate = DateTime.MinValue;
        internal string _LangCode = string.Empty;
        internal AppUserGender _Gender = AppUserGender.Unknown;
        internal Int32 _UnreadNotificationCount = 0;
        internal string _ProfileImage = string.Empty;
        internal string _FirstName = string.Empty;
        internal string _LastName = string.Empty;
        internal string _HouseNum = string.Empty;
        internal string _Street = string.Empty;
        internal Int64 _CityId = 0;
        internal string _ApartmentNumber = string.Empty;
        internal string _Phone = string.Empty;
        internal string _Floor = string.Empty;
        internal Geometry.Point _AddressLocation = null;
        internal bool _IsAdv = false;
        internal bool _IsDeleted = false;
        internal int _OrderDisplay = 0;
        #endregion

        #region Properties
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        public string UniqueIdString
        {
            get { return _UniqueIdString; }
            set { _UniqueIdString = value; }
        }
        public DateTime LastLogin
        {
            get { return _LastLogin; }
            set { _LastLogin = value; }
        }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }
        public bool IsLocked
        {
            get { return _IsLocked; }
            set { _IsLocked = value; }
        }
        public bool IsVerified
        {
            get { return _IsVerified; }
            set { _IsVerified = value; }
        }
        public int BadLoginTries
        {
            get { return _BadLoginTries; }
            set { _BadLoginTries = value; }
        }
        public Int64? FacebookId
        {
            get { return _FacebookId; }
            set { _FacebookId = value; }
        }
        public string FacebookToken
        {
            get { return _FacebookToken; }
            set { _FacebookToken = value; }
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
        public string LangCode
        {
            get { return _LangCode; }
            set { _LangCode = value; }
        }
        public AppUserGender Gender
        {
            get { return _Gender; }
            set { _Gender = value; }
        }
        public Int32 UnreadNotificationCount
        {
            get { return _UnreadNotificationCount; }
            set { _UnreadNotificationCount = value; }
        }
        public string ProfileImage
        {
            get { return _ProfileImage; }
            set { _ProfileImage = value; }
        }
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }
        public string HouseNum
        {
            get { return _HouseNum; }
            set { _HouseNum = value; }
        }
        public string Street
        {
            get { return _Street; }
            set { _Street = value; }
        }
        public Int64 CityId
        {
            get { return _CityId; }
            set { _CityId = value; }
        }
        public string ApartmentNumber
        {
            get { return _ApartmentNumber; }
            set { _ApartmentNumber = value; }
        }
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }
        public string Floor
        {
            get { return _Floor; }
            set { _Floor = value; }
        }
        public Geometry.Point AddressLocation
        {
            get { return _AddressLocation; }
            set { _AddressLocation = value; }
        }
        public bool IsAdv
        {
            get { return _IsAdv; }
            set { _IsAdv = value; }
        }
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set { _IsDeleted = value; }
        }
        public int OrderDisplay
        {
            get { return _OrderDisplay; }
            set { _OrderDisplay = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return AppUserId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Email, Email);
            qry.Insert(Columns.UniqueIdString, UniqueIdString);
            qry.Insert(Columns.LastLogin, LastLogin);
            qry.Insert(Columns.CreateDate, CreateDate);
            qry.Insert(Columns.IsLocked, IsLocked);
            qry.Insert(Columns.IsVerified, IsVerified);
            qry.Insert(Columns.BadLoginTries, BadLoginTries);
            qry.Insert(Columns.FacebookId, FacebookId);
            qry.Insert(Columns.FacebookToken, FacebookToken);
            qry.Insert(Columns.Password, Password);
            qry.Insert(Columns.PasswordSalt, PasswordSalt);
            qry.Insert(Columns.PasswordRecoveryKey, PasswordRecoveryKey);
            qry.Insert(Columns.PasswordRecoveryDate, PasswordRecoveryDate);
            qry.Insert(Columns.LangCode, LangCode);
            qry.Insert(Columns.Gender, Gender);
            qry.Insert(Columns.UnreadNotificationCount, UnreadNotificationCount);
            qry.Insert(Columns.ProfileImage, ProfileImage);
            qry.Insert(Columns.FirstName, FirstName);
            qry.Insert(Columns.LastName, LastName);
            qry.Insert(Columns.HouseNum, HouseNum);
            qry.Insert(Columns.Street, Street);
            qry.Insert(Columns.CityId, CityId);
            qry.Insert(Columns.ApartmentNumber, ApartmentNumber);
            qry.Insert(Columns.Phone, Phone);
            qry.Insert(Columns.Floor, Floor);
            qry.Insert(Columns.AddressLocation, AddressLocation);
            qry.Insert(Columns.IsAdv, IsAdv);
            qry.Insert(Columns.IsDeleted, IsDeleted);
            qry.Insert(Columns.OrderDisplay, OrderDisplay);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                AppUserId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Email, Email);
            qry.Update(Columns.UniqueIdString, UniqueIdString);
            qry.Update(Columns.LastLogin, LastLogin);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Update(Columns.IsLocked, IsLocked);
            qry.Update(Columns.IsVerified, IsVerified);
            qry.Update(Columns.BadLoginTries, BadLoginTries);
            qry.Update(Columns.FacebookId, FacebookId);
            qry.Update(Columns.FacebookToken, FacebookToken);
            qry.Update(Columns.Password, Password);
            qry.Update(Columns.PasswordSalt, PasswordSalt);
            qry.Update(Columns.PasswordRecoveryKey, PasswordRecoveryKey);
            qry.Update(Columns.PasswordRecoveryDate, PasswordRecoveryDate);
            qry.Update(Columns.LangCode, LangCode);
            qry.Update(Columns.Gender, Gender);
            qry.Update(Columns.UnreadNotificationCount, UnreadNotificationCount);
            qry.Update(Columns.ProfileImage, ProfileImage);
            qry.Update(Columns.FirstName, FirstName);
            qry.Update(Columns.LastName, LastName);
            qry.Update(Columns.HouseNum, HouseNum);
            qry.Update(Columns.Street, Street);
            qry.Update(Columns.CityId, CityId);
            qry.Update(Columns.ApartmentNumber, ApartmentNumber);
            qry.Update(Columns.Phone, Phone);
            qry.Update(Columns.Floor, Floor);
            qry.Update(Columns.AddressLocation, AddressLocation);
            qry.Update(Columns.IsAdv, IsAdv);
            qry.Update(Columns.IsDeleted, IsDeleted);
            qry.Update(Columns.OrderDisplay, OrderDisplay);
            qry.Where(Columns.AppUserId, AppUserId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            Email = (string)reader[Columns.Email];
            UniqueIdString = StringOrNullFromDb(reader[Columns.UniqueIdString]);
            LastLogin = Convert.ToDateTime(reader[Columns.LastLogin]);
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);
            IsLocked = Convert.ToBoolean(reader[Columns.IsLocked]);
            IsVerified = Convert.ToBoolean(reader[Columns.IsVerified]);
            BadLoginTries = Convert.ToInt32(reader[Columns.BadLoginTries]);
            FacebookId = IsNull(reader[Columns.FacebookId]) ? (Int64?)null : Convert.ToInt64(reader[Columns.FacebookId]);
            FacebookToken = (string)reader[Columns.FacebookToken];
            Password = (string)reader[Columns.Password];
            PasswordSalt = (string)reader[Columns.PasswordSalt];
            PasswordRecoveryKey = (string)reader[Columns.PasswordRecoveryKey];
            PasswordRecoveryDate = Convert.ToDateTime(reader[Columns.PasswordRecoveryDate]);
            LangCode = (string)reader[Columns.LangCode];
            Gender = (AppUserGender)Convert.ToInt32(reader[Columns.Gender]);
            UnreadNotificationCount = Convert.ToInt32(reader[Columns.UnreadNotificationCount]);
            ProfileImage = (string)reader[Columns.ProfileImage];
            FirstName = (string)reader[Columns.FirstName];
            LastName = (string)reader[Columns.LastName];
            HouseNum = (string)reader[Columns.HouseNum];
            Street = (string)reader[Columns.Street];
            CityId = Convert.ToInt64(reader[Columns.CityId]);
            ApartmentNumber = (string)reader[Columns.ApartmentNumber];
            Phone = (string)reader[Columns.Phone];
            Floor = (string)reader[Columns.Floor];
            AddressLocation = reader.GetGeometry(Columns.AddressLocation) as Geometry.Point;
            IsAdv = Convert.ToBoolean(reader[Columns.IsAdv]);
            IsDeleted = Convert.ToBoolean(reader[Columns.IsDeleted]);
            OrderDisplay = Convert.ToInt32(reader[Columns.OrderDisplay]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppUser FetchByID(Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.AppUserId, AppUserId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUser item = new AppUser();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.AppUserId, AppUserId);
            return qry.Execute();
        }
        public static AppUser FetchByID(ConnectorBase conn, Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.AppUserId, AppUserId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppUser item = new AppUser();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.AppUserId, AppUserId);
            return qry.Execute(conn);
        }
        #endregion
    }
    public partial class AppUser
    {
        static public AppUser FetchByFacebookId(Int64 facebookId)
        {
            using (DataReaderBase reader = new Query(TableSchema).Where(Columns.FacebookId, facebookId).ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUser ret = new AppUser();
                    ret.Read(reader);
                    return ret;
                }
            }
            return null;
        }
        static public AppUser FetchByEmail(string email)
        {
            using (DataReaderBase reader = new Query(TableSchema).Where(Columns.UniqueIdString, email.NormalizeEmail()).ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUser ret = new AppUser();
                    ret.Read(reader);
                    return ret;
                }
            }
            return null;
        }
    }
}
