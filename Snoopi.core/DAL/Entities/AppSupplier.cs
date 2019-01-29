using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * AppSupplier
 * AppSupplier
 * SupplierId:                      INT64; PRIMARY KEY; AUTOINCREMENT;
 * Email:                           FIXEDSTRING(64); Email
 * UniqueIdString:                  FIXEDSTRING(64); NULLABLE;Normalized email address, or other ID means
 * LastLogin:                       DATETIME; DEFAULT DateTime.MinValue; Last login date/time
 * IsLocked:                        BOOL; DEFAULT false; Is locked, due to deliberate locking or to password abuse
 * IsVerified:                      BOOL; DEFAULT true; Is this user verified
 * BadLoginTries:                   DEFAULT 0; Number of recorded bad logins, since last successful login
 * Password:                        FIXEDSTRING(128); Scrambled password
 * PasswordSalt:                    FIXEDSTRING(64); Password salt
 * PasswordRecoveryKey:             FIXEDSTRING(128); DEFAULT string.Empty; Password recovery key
 * PasswordRecoveryDate:            DATETIME; DEFAULT DateTime.MinValue; The date that the password recovery request was issued
 * LangCode:                        FIXEDSTRING(16); DEFAULT string.Empty;
 * MastercardCode:                  FIXEDSTRING(16); DEFAULT "0";
 * UnreadNotificationCount:         INT32; DEFAULT 0;
 * ProfileImage:                    FIXEDSTRING(255); DEFAULT string.Empty;
 * ContactName:                     FIXEDSTRING(64); DEFAULT string.Empty;
 * BusinessName:                    FIXEDSTRING(64); DEFAULT string.Empty;
 * HouseNum:                        FIXEDSTRING(16); DEFAULT string.Empty;
 * Street:                          FIXEDSTRING(16); DEFAULT string.Empty;
 * CityId:                          INT64;
 * ContactPhone:                    FIXEDSTRING(64); DEFAULT string.Empty;
 * Phone:                           FIXEDSTRING(64); DEFAULT string.Empty;
 * AddressLocation:                 POINT; DEFAULT null;
 * IsService:                       BOOL; DEFAULT false;
 * IsProduct:                       BOOL; DEFAULT false;
 * IsPremium:                       BOOL; DEFAULT false;
 * Precent:                         INT; DEFAULT 0;
 * SumPerMonth:                     INT; DEFAULT 0;
 * MaxWinningsNum:                  INT; DEFAULT 0;
 * Status:                          BOOL; DEFAULT true;
 * StatusJoinBid:                   BOOL; DEFAULT false;
 * AllowChangeStatusJoinBid:        BOOL; DEFAULT false;
 * IsDeleted:                       BOOL; DEFAULT false;
 * IsAdv:                           BOOL; DEFAULT false;
 * CreateDate:                      DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow; The date that the user was created
 * @INDEX:                          NAME(ix_AppSupplier_UniqueIdString); [UniqueIdString]; UNIQUE;
 * @INDEX:                          NAME(ix_AppSupplier_SupplierId);[SupplierId];
 * @INDEX:                          NAME(ix_AppSupplier_CityId);[CityId];
 * @FOREIGNKEY:                     NAME(fk_AppSupplier_CityId); FOREIGNTABLE(City); COLUMNS[CityId]; FOREIGNCOLUMNS[CityId];;
 * */

namespace Snoopi.core.DAL
{
    public partial class AppSupplierCollection : AbstractRecordList<AppSupplier, AppSupplierCollection>
    {
    }

    public partial class AppSupplier : AbstractRecord<AppSupplier>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public const string SupplierId = "SupplierId";
            public const string Email = "Email"; // Email
            public const string UniqueIdString = "UniqueIdString"; // Normalized email address, or other ID means
            public const string LastLogin = "LastLogin"; // Last login date/time
            public const string IsLocked = "IsLocked"; // Is locked, due to deliberate locking or to password abuse
            public const string IsVerified = "IsVerified"; // Is this user verified
            public const string BadLoginTries = "BadLoginTries"; // Number of recorded bad logins, since last successful login
            public const string Password = "Password"; // Scrambled password
            public const string PasswordSalt = "PasswordSalt"; // Password salt
            public const string PasswordRecoveryKey = "PasswordRecoveryKey"; // Password recovery key
            public const string PasswordRecoveryDate = "PasswordRecoveryDate"; // The date that the password recovery request was issued
            public const string LangCode = "LangCode";
            public const string MastercardCode = "MastercardCode";
            public const string UnreadNotificationCount = "UnreadNotificationCount";
            public const string ProfileImage = "ProfileImage";
            public const string ContactName = "ContactName";
            public const string BusinessName = "BusinessName";
            public const string HouseNum = "HouseNum";
            public const string Street = "Street";
            public const string CityId = "CityId";
            public const string ContactPhone = "ContactPhone";
            public const string Phone = "Phone";
            public const string AddressLocation = "AddressLocation";
            public const string IsService = "IsService";
            public const string IsProduct = "IsProduct";
            public const string IsPremium = "IsPremium";
            public const string Precent = "Precent";
            public const string SumPerMonth = "SumPerMonth";
            public const string MaxWinningsNum = "MaxWinningsNum";
            public const string Status = "Status";
            public const string StatusJoinBid = "StatusJoinBid";
            public const string AllowChangeStatusJoinBid = "AllowChangeStatusJoinBid";
            public const string IsDeleted = "IsDeleted";
            public const string IsAdv = "IsAdv";
            public const string CreateDate = "CreateDate"; // The date that the user was created
            public const string Description = "Description";
            public const string Discount = "Discount";
            public const string ApprovedTermsDate = "ApprovedTermsDate";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppSupplier";
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Email, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.UniqueIdString, typeof(string), DataType.Char, 64, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.LastLogin, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.IsLocked, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsVerified, typeof(bool), 0, 0, 0, false, false, false, true);
                schema.AddColumn(Columns.BadLoginTries, typeof(int), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.Password, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PasswordSalt, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PasswordRecoveryKey, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.PasswordRecoveryDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.LangCode, typeof(string), DataType.Char, 16, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.MastercardCode, typeof(string), DataType.Char, 16, 0, 0, false, false, false, "0");
                schema.AddColumn(Columns.UnreadNotificationCount, typeof(Int32), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.ProfileImage, typeof(string), DataType.Char, 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.ContactName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.BusinessName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.HouseNum, typeof(string), DataType.Char, 16, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Street, typeof(string), DataType.Char, 16, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.CityId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ContactPhone, typeof(string), DataType.Char, 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Phone, typeof(string), DataType.Char, 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.AddressLocation, typeof(Geometry.Point), DataType.Point, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.IsService, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsProduct, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsPremium, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.Precent, typeof(int), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.SumPerMonth, typeof(int), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.MaxWinningsNum, typeof(int), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.Status, typeof(bool), 0, 0, 0, false, false, false, true);
                schema.AddColumn(Columns.StatusJoinBid, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.AllowChangeStatusJoinBid, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsDeleted, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsAdv, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.Description, typeof(string), DataType.Char, 1024, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Discount, typeof(string), DataType.Char, 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.ApprovedTermsDate, typeof(DateTime?), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_AppSupplier_UniqueIdString", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.UniqueIdString);
                schema.AddIndex("ix_AppSupplier_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_AppSupplier_CityId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CityId);

                schema.AddForeignKey("fk_AppSupplier_CityId", AppSupplier.Columns.CityId, City.TableSchema.SchemaName, City.Columns.CityId, TableSchema.ForeignKeyReference.None, TableSchema.ForeignKeyReference.None);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private Int64 _supplierId;
        internal string _Email = string.Empty;
        internal string _UniqueIdString = null;
        internal DateTime _LastLogin = DateTime.MinValue;
        internal bool _IsLocked = false;
        internal bool _IsVerified = true;
        internal int _BadLoginTries = 0;
        internal string _Password = string.Empty;
        internal string _PasswordSalt = string.Empty;
        internal string _PasswordRecoveryKey = string.Empty;
        internal DateTime _PasswordRecoveryDate = DateTime.MinValue;
        internal string _LangCode = string.Empty;
        internal string _MastercardCode = "0";
        internal Int32 _UnreadNotificationCount = 0;
        internal string _ProfileImage = string.Empty;
        internal string _ContactName = string.Empty;
        internal string _BusinessName = string.Empty;
        internal string _HouseNum = string.Empty;
        internal string _Street = string.Empty;
        internal Int64 _CityId = 0;
        internal string _ContactPhone = string.Empty;
        internal string _Phone = string.Empty;
        internal Geometry.Point _AddressLocation = null;
        internal bool _IsService = false;
        internal bool _IsPremium = false;
        internal bool _IsProduct = false;
        internal int _Precent = 0;
        internal int _SumPerMonth = 0;
        internal int _MaxWinningsNum = 0;
        internal bool _Status = true;
        internal bool _StatusJoinBid = false;
        internal bool _AllowChangeStatusJoinBid = false;
        internal bool _IsDeleted = false;
        internal bool _IsAdv = false;
        internal DateTime _CreateDate = DateTime.UtcNow;
        internal string _Description = string.Empty;
        internal string _Discount = string.Empty;
        internal DateTime? _approvedTermsDate;


        #endregion

        #region Properties
        public Int64 SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
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

        public DateTime? ApprovedTermsDate
        {
            get { return _approvedTermsDate; }
            set { _approvedTermsDate = value; }
        }

        public string LangCode
        {
            get { return _LangCode; }
            set { _LangCode = value; }
        }
        public string MastercardCode
        {
            get { return _MastercardCode; }
            set { _MastercardCode = value; }
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
        public string ContactName
        {
            get { return _ContactName; }
            set { _ContactName = value; }
        }
        public string BusinessName
        {
            get { return _BusinessName; }
            set { _BusinessName = value; }
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
        public string ContactPhone
        {
            get { return _ContactPhone; }
            set { _ContactPhone = value; }
        }
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }
        public Geometry.Point AddressLocation
        {
            get { return _AddressLocation; }
            set { _AddressLocation = value; }
        }
        public bool IsService
        {
            get { return _IsService; }
            set { _IsService = value; }
        }

        public bool IsPremium
        {
            get { return _IsPremium; }
            set { _IsPremium = value; }
        }

        public bool IsProduct
        {
            get { return _IsProduct; }
            set { _IsProduct = value; }
        }
        public int Precent
        {
            get { return _Precent; }
            set { _Precent = value; }
        }
        public int SumPerMonth
        {
            get { return _SumPerMonth; }
            set { _SumPerMonth = value; }
        }
        public int MaxWinningsNum
        {
            get { return _MaxWinningsNum; }
            set { _MaxWinningsNum = value; }
        }
        public bool Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        public bool StatusJoinBid
        {
            get { return _StatusJoinBid; }
            set { _StatusJoinBid = value; }
        }
        public bool AllowChangeStatusJoinBid
        {
            get { return _AllowChangeStatusJoinBid; }
            set { _AllowChangeStatusJoinBid = value; }
        }
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set { _IsDeleted = value; }
        }
        public bool IsAdv
        {
            get { return _IsAdv; }
            set { _IsAdv = value; }
        }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public string Discount
        {
            get { return _Discount; }
            set { _Discount = value; }
        }

        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return SupplierId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Email, Email);
            qry.Insert(Columns.UniqueIdString, UniqueIdString);
            qry.Insert(Columns.LastLogin, LastLogin);
            qry.Insert(Columns.IsLocked, IsLocked);
            qry.Insert(Columns.IsVerified, IsVerified);
            qry.Insert(Columns.BadLoginTries, BadLoginTries);
            qry.Insert(Columns.Password, Password);
            qry.Insert(Columns.PasswordSalt, PasswordSalt);
            qry.Insert(Columns.PasswordRecoveryKey, PasswordRecoveryKey);
            qry.Insert(Columns.PasswordRecoveryDate, PasswordRecoveryDate);
            qry.Insert(Columns.LangCode, LangCode);
            qry.Insert(Columns.MastercardCode, MastercardCode);
            qry.Insert(Columns.UnreadNotificationCount, UnreadNotificationCount);
            qry.Insert(Columns.ProfileImage, ProfileImage);
            qry.Insert(Columns.ContactName, ContactName);
            qry.Insert(Columns.BusinessName, BusinessName);
            qry.Insert(Columns.HouseNum, HouseNum);
            qry.Insert(Columns.Street, Street);
            qry.Insert(Columns.CityId, CityId);
            qry.Insert(Columns.ContactPhone, ContactPhone);
            qry.Insert(Columns.Phone, Phone);
            qry.Insert(Columns.AddressLocation, AddressLocation);
            qry.Insert(Columns.IsService, IsService);
            qry.Insert(Columns.IsProduct, IsProduct);
            qry.Insert(Columns.IsPremium, IsProduct);
            qry.Insert(Columns.Precent, Precent);
            qry.Insert(Columns.SumPerMonth, SumPerMonth);
            qry.Insert(Columns.MaxWinningsNum, MaxWinningsNum);
            qry.Insert(Columns.Status, Status);
            qry.Insert(Columns.StatusJoinBid, StatusJoinBid);
            qry.Insert(Columns.AllowChangeStatusJoinBid, AllowChangeStatusJoinBid);
            qry.Insert(Columns.IsDeleted, IsDeleted);
            qry.Insert(Columns.IsAdv, IsAdv);
            qry.Insert(Columns.CreateDate, CreateDate);
            qry.Insert(Columns.ApprovedTermsDate, ApprovedTermsDate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                SupplierId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Email, Email);
            qry.Update(Columns.UniqueIdString, UniqueIdString);
            qry.Update(Columns.LastLogin, LastLogin);
            qry.Update(Columns.IsLocked, IsLocked);
            qry.Update(Columns.IsVerified, IsVerified);
            qry.Update(Columns.BadLoginTries, BadLoginTries);
            qry.Update(Columns.Password, Password);
            qry.Update(Columns.PasswordSalt, PasswordSalt);
            qry.Update(Columns.PasswordRecoveryKey, PasswordRecoveryKey);
            qry.Update(Columns.PasswordRecoveryDate, PasswordRecoveryDate);
            qry.Update(Columns.LangCode, LangCode);
            qry.Update(Columns.MastercardCode, MastercardCode);
            qry.Update(Columns.UnreadNotificationCount, UnreadNotificationCount);
            qry.Update(Columns.ProfileImage, ProfileImage);
            qry.Update(Columns.ContactName, ContactName);
            qry.Update(Columns.BusinessName, BusinessName);
            qry.Update(Columns.HouseNum, HouseNum);
            qry.Update(Columns.Street, Street);
            qry.Update(Columns.CityId, CityId);
            qry.Update(Columns.ContactPhone, ContactPhone);
            qry.Update(Columns.Phone, Phone);
            qry.Update(Columns.AddressLocation, AddressLocation);
            qry.Update(Columns.IsService, IsService);
            qry.Update(Columns.IsProduct, IsProduct);
            qry.Update(Columns.IsPremium, IsPremium);
            qry.Update(Columns.Precent, Precent);
            qry.Update(Columns.SumPerMonth, SumPerMonth);
            qry.Update(Columns.MaxWinningsNum, MaxWinningsNum);
            qry.Update(Columns.Status, Status);
            qry.Update(Columns.StatusJoinBid, StatusJoinBid);
            qry.Update(Columns.AllowChangeStatusJoinBid, AllowChangeStatusJoinBid);
            qry.Update(Columns.IsDeleted, IsDeleted);
            qry.Update(Columns.IsAdv, IsAdv);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Update(Columns.Discount, Discount);
            qry.Update(Columns.Description, Description);
            qry.Update(Columns.ApprovedTermsDate, ApprovedTermsDate);
            qry.Where(Columns.SupplierId, SupplierId);
            

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            Email = (string)reader[Columns.Email];
            UniqueIdString = StringOrNullFromDb(reader[Columns.UniqueIdString]);
            LastLogin = Convert.ToDateTime(reader[Columns.LastLogin]);
            IsLocked = Convert.ToBoolean(reader[Columns.IsLocked]);
            IsVerified = Convert.ToBoolean(reader[Columns.IsVerified]);
            BadLoginTries = Convert.ToInt32(reader[Columns.BadLoginTries]);
            Password = (string)reader[Columns.Password];
            PasswordSalt = (string)reader[Columns.PasswordSalt];
            PasswordRecoveryKey = (string)reader[Columns.PasswordRecoveryKey];
            PasswordRecoveryDate = Convert.ToDateTime(reader[Columns.PasswordRecoveryDate]);
            LangCode = (string)reader[Columns.LangCode];
            MastercardCode = (string)reader[Columns.MastercardCode];
            UnreadNotificationCount = Convert.ToInt32(reader[Columns.UnreadNotificationCount]);
            ProfileImage = (string)reader[Columns.ProfileImage];
            ContactName = (string)reader[Columns.ContactName];
            BusinessName = (string)reader[Columns.BusinessName];
            HouseNum = (string)reader[Columns.HouseNum];
            Street = (string)reader[Columns.Street];
            CityId = Convert.ToInt64(reader[Columns.CityId]);
            ContactPhone = (string)reader[Columns.ContactPhone];
            Phone = (string)reader[Columns.Phone];
            AddressLocation = reader.GetGeometry(Columns.AddressLocation) as Geometry.Point;
            IsService = Convert.ToBoolean(reader[Columns.IsService]);
            IsProduct = Convert.ToBoolean(reader[Columns.IsProduct]);
            IsPremium = Convert.ToBoolean(reader[Columns.IsPremium]);
            Precent = Convert.ToInt32(reader[Columns.Precent]);
            SumPerMonth = Convert.ToInt32(reader[Columns.SumPerMonth]);
            MaxWinningsNum = Convert.ToInt32(reader[Columns.MaxWinningsNum]);
            Status = Convert.ToBoolean(reader[Columns.Status]);
            StatusJoinBid = Convert.ToBoolean(reader[Columns.StatusJoinBid]);
            AllowChangeStatusJoinBid = Convert.ToBoolean(reader[Columns.AllowChangeStatusJoinBid]);
            IsDeleted = Convert.ToBoolean(reader[Columns.IsDeleted]);
            IsAdv = Convert.ToBoolean(reader[Columns.IsAdv]);
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);
            Description =reader[Columns.Description].ToString();
            Discount = reader[Columns.Discount].ToString();
            ApprovedTermsDate = reader[Columns.ApprovedTermsDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.ApprovedTermsDate]);
            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppSupplier FetchByID(Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppSupplier item = new AppSupplier();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SupplierId, SupplierId);
            return qry.Execute();
        }
        public static AppSupplier FetchByID(ConnectorBase conn, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppSupplier item = new AppSupplier();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SupplierId, SupplierId);
            return qry.Execute(conn);
        }
        #endregion
    }
    public partial class AppSupplier
    {
        static public AppSupplier FetchByEmail(string email)
        {
           // using (DataReaderBase reader = new Query(TableSchema).Where(Columns.UniqueIdString, email.NormalizeEmail()).ExecuteReader())
            using (DataReaderBase reader = new Query(TableSchema).Where(Columns.Email, email.NormalizeEmail()).AddWhere(Columns.UniqueIdString,WhereComparision.IsNot,null).ExecuteReader())
            {
                if (reader.Read())
                {
                    AppSupplier ret = new AppSupplier();
                    ret.Read(reader);
                    return ret;
                }
            }
            return null;
        }
    }
}
