using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snoopi.core.DAL;
using dg.Sql;
using dg.Sql.Connector;
using System.IO;

namespace Snoopi.core.BL
{
    public class AppUserUI
    {
        private Int64 _AppUserId;
        private string _ProfileImage;
        private string _Email;
        private string _UniqueEmail;
        private bool _IsBlocked;
        private string _Password;
        private Int32 _UnreadNotificationCount;
        private string _FirstName;
        private string _LastName;
        private string _Phone;
        private string _LastLogin;
        //private string _Address;
        private string _CityName;
        private string _Street;
        private string _HouseNum;
        private string _Floor;
        private string _ApartmentNumber;
        private bool _IsAdv;
        private string _CreateDate;

        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }

        public string ProfileImage
        {
            get { return _ProfileImage; }
            set { _ProfileImage = value; }
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

        public bool IsLocked
        {
            get { return _IsBlocked; }
            set { _IsBlocked = value; }
        }

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public Int32 UnreadNotificationCount
        {
            get { return _UnreadNotificationCount; }
            set { _UnreadNotificationCount = value; }
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

        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }

        public string LastLogin
        {
            get { return _LastLogin; }
            set { _LastLogin = value; }
        }

        public string CityName
        {
            get { return _CityName; }
            set { _CityName = value; }
        }

        public string Street
        {
            get { return _Street; }
            set { _Street = value; }
        }

        public string HouseNum
        {
            get { return _HouseNum; }
            set { _HouseNum = value; }
        }

        public string Floor
        {
            get { return _Floor; }
            set { _Floor = value; }
        }

        public string ApartmentNumber
        {
            get { return _ApartmentNumber; }
            set { _ApartmentNumber = value; }
        }

        //public string Address
        //{
        //    get { return Street + " " + HouseNum + " " + Floor + " " + ApartmentNumber +"\n" + CityName; }
        //}

        public bool IsAdv
        {
            get { return _IsAdv; }
            set { _IsAdv = value; }
        }

        public string CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }


        public static AppUserUI ConvertAppUserToAppUserUI(AppUser app_user)
        {

            AppUserUI appUserUI = new AppUserUI
            {
                AppUserId = app_user.AppUserId,
                ProfileImage = (app_user.ProfileImage) != null ? app_user.ProfileImage.ToString() : "",
                Email = (app_user.Email) != null ? app_user.Email.ToString() : "",
                IsLocked = app_user.IsLocked,
                FirstName = (app_user.FirstName) != null ? app_user.FirstName : "",
                LastName = (app_user.LastName) != null ? app_user.LastName.ToString() : "",
                Phone = (app_user.Phone) != null ? app_user.Phone.ToString() : "",
            };

            return new AppUserUI { };

        }
        public static AppUserUI GetAppUserUI(Int64 AppUserId)
        {
            Query q = new Query(AppUser.TableSchema)
                .Select(AppUser.Columns.AppUserId)
                .AddSelect(AppUser.Columns.ProfileImage)
                .AddSelect(AppUser.Columns.Email)
                .AddSelect(AppUser.Columns.IsLocked)
                .AddSelect(AppUser.Columns.FirstName)
                .AddSelect(AppUser.Columns.LastName)
                .AddSelect(AppUser.Columns.Phone)
                .AddSelect(AppUser.Columns.LastLogin)
                .AddSelect(AppUser.Columns.IsAdv)
                .AddSelect(AppUser.Columns.CreateDate)
                .AddSelect(AppUser.Columns.Street)
                .AddSelect(AppUser.Columns.HouseNum)
                .AddSelect(AppUser.Columns.Floor)
                .AddSelect(AppUser.Columns.ApartmentNumber)
                .AddWhere(AppUser.Columns.IsDeleted, false)
                .Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName)
                .AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName)
                .Where(AppUser.Columns.AppUserId, AppUserId);

            AppUserUI app_user_ui = new AppUserUI();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                if (reader.Read())
                {                    
                    app_user_ui.AppUserId = Convert.ToInt64(reader[AppUser.Columns.AppUserId]);
                    app_user_ui.Email = reader[AppUser.Columns.Email] == null ? "" : reader[AppUser.Columns.Email].ToString();
                    app_user_ui.IsLocked = Convert.ToBoolean(reader[AppUser.Columns.IsLocked]);
                    app_user_ui.FirstName = reader[AppUser.Columns.FirstName] == null ? "" : reader[AppUser.Columns.FirstName].ToString();
                    app_user_ui.LastName = reader[AppUser.Columns.LastName] == null ? "" : reader[AppUser.Columns.LastName].ToString();
                    app_user_ui.Phone = reader[AppUser.Columns.Phone] == null ? "" : reader[AppUser.Columns.Phone].ToString();
                    app_user_ui.LastLogin = reader[AppUser.Columns.LastLogin] == null ? "" : Convert.ToDateTime(reader[AppUser.Columns.LastLogin]).ToLocalTime().ToString(@"dd/MM/yyyy HH:mm");
                    app_user_ui.IsAdv = Convert.ToBoolean(reader[AppUser.Columns.IsAdv]);
                    app_user_ui.ApartmentNumber = reader[AppUser.Columns.ApartmentNumber] == null ? "" : reader[AppUser.Columns.ApartmentNumber].ToString();
                    app_user_ui.Street = reader[AppUser.Columns.Street] == null ? "" : reader[AppUser.Columns.Street].ToString();
                    app_user_ui.HouseNum = reader[AppUser.Columns.HouseNum] == null ? "" : reader[AppUser.Columns.HouseNum].ToString();
                    app_user_ui.Floor = reader[AppUser.Columns.Floor] == null ? "" : reader[AppUser.Columns.Floor].ToString();
                    app_user_ui.CityName = reader[City.Columns.CityName] == null ? "" : reader[City.Columns.CityName].ToString();
                    app_user_ui.CreateDate = reader[AppUser.Columns.CreateDate] == null ? "" : Convert.ToDateTime(reader[AppUser.Columns.CreateDate]).ToLocalTime().ToString(@"dd/MM/yyyy");                    
                }
            }
            return app_user_ui;
        }

        public static List<AppUserUI> GetAllAppUserUI(DateTime from = new DateTime(), DateTime to = new DateTime(), string SearchName = "", string SearchPhone = "", int PageSize = 0, int CurrentPageIndex = 0)
        {
            List<AppUserUI> app_users = new List<AppUserUI>();
            Query q = new Query(AppUser.TableSchema)
                .Select(AppUser.Columns.AppUserId)
                .AddSelect(AppUser.Columns.ProfileImage)
                .AddSelect(AppUser.Columns.Email)
                .AddSelect(AppUser.Columns.IsLocked)
                .AddSelect(AppUser.Columns.FirstName)
                .AddSelect(AppUser.Columns.LastName)
                .AddSelect(AppUser.Columns.Phone)
                .AddSelect(AppUser.Columns.LastLogin)
                .AddSelect(AppUser.Columns.IsAdv)
                .AddSelect(AppUser.Columns.CreateDate)
                .AddSelect(AppUser.Columns.Street)
                .AddSelect(AppUser.Columns.HouseNum)
                .AddSelect(AppUser.Columns.Floor)
                .AddSelect(AppUser.Columns.ApartmentNumber)
                .AddWhere(AppUser.Columns.IsDeleted, false)
                .Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName)
                .AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            if (SearchName != "" && SearchPhone != "")
            {
                WhereList wl = new WhereList();
                wl.OR(AppUser.Columns.FirstName, WhereComparision.Like, SearchName)
                   .OR(AppUser.Columns.LastName, WhereComparision.Like, SearchName);
                q.AND(wl);
                q.AddWhere(AppUser.Columns.Phone, WhereComparision.Like, SearchPhone);
            }
            if (from != DateTime.MinValue)
            {
                q.AddWhere(AppUser.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, from);
            }
            if (to != DateTime.MinValue)
            {
                to = to.Date + new TimeSpan(23, 59, 59);
                q.AddWhere(AppUser.Columns.CreateDate, WhereComparision.LessThanOrEqual, to);
            }
            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    AppUserUI app_user_ui = new AppUserUI();
                    app_user_ui.AppUserId = Convert.ToInt64(reader[AppUser.Columns.AppUserId]);
                    app_user_ui.Email = reader[AppUser.Columns.Email] == null ? "" : reader[AppUser.Columns.Email].ToString();
                    app_user_ui.IsLocked = Convert.ToBoolean(reader[AppUser.Columns.IsLocked]);
                    app_user_ui.FirstName = reader[AppUser.Columns.FirstName] == null ? "" : reader[AppUser.Columns.FirstName].ToString();
                    app_user_ui.LastName = reader[AppUser.Columns.LastName] == null ? "" : reader[AppUser.Columns.LastName].ToString();
                    app_user_ui.Phone = reader[AppUser.Columns.Phone] == null ? "" : reader[AppUser.Columns.Phone].ToString();
                    app_user_ui.LastLogin = reader[AppUser.Columns.LastLogin] == null ? "" : Convert.ToDateTime(reader[AppUser.Columns.LastLogin]).ToLocalTime().ToString(@"dd/MM/yyyy HH:mm");
                    app_user_ui.IsAdv = Convert.ToBoolean(reader[AppUser.Columns.IsAdv]);
                    app_user_ui.ApartmentNumber = reader[AppUser.Columns.ApartmentNumber] == null ? "" : reader[AppUser.Columns.ApartmentNumber].ToString();
                    app_user_ui.Street = reader[AppUser.Columns.Street] == null ? "" : reader[AppUser.Columns.Street].ToString();
                    app_user_ui.HouseNum = reader[AppUser.Columns.HouseNum] == null ? "" : reader[AppUser.Columns.HouseNum].ToString();
                    app_user_ui.Floor = reader[AppUser.Columns.Floor] == null ? "" : reader[AppUser.Columns.Floor].ToString();
                    app_user_ui.CityName = reader[City.Columns.CityName] == null ? "" : reader[City.Columns.CityName].ToString();
                    app_user_ui.CreateDate = reader[AppUser.Columns.CreateDate] == null ? "" : Convert.ToDateTime(reader[AppUser.Columns.CreateDate]).ToLocalTime().ToString(@"dd/MM/yyyy");

                    app_users.Add(app_user_ui);
                }
            }
            return app_users;
        }

    }

}

