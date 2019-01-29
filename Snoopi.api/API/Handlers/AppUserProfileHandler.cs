using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using Snoopi.core.BL;
using System.IO;
using dg.Utilities.Imaging;
using dg.Utilities.Encryption;
using dg.Utilities.WebApiServices;
using Snoopi.core.DAL;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using dg.Sql;
using dg.Sql.Connector;
using System.Globalization;
using GoogleMaps.LocationServices;

namespace Snoopi.api
{
    public class AppUserProfileHandler : ApiHandlerBase
    {
        private static string[] AcceptedImageExtensions = new string[] { @".jpg", @".jpeg", @".png" };
        public static bool IsAcceptedImageExtension(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return AcceptedImageExtensions.Contains(ext);
        }

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        Query qry = Query.New<AppUser>()
                            .ClearSelect()
                            .Where(AppUser.Columns.AppUserId, AppUserId)
                            .LimitRows(1);

                        jsonWriter.WriteStartObject();

                        using (DataReaderBase reader = qry.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                jsonWriter.WritePropertyName(@"email");
                                jsonWriter.WriteValue(reader[AppUser.Columns.Email] ?? "");
                                jsonWriter.WritePropertyName(@"first_name");
                                jsonWriter.WriteValue(reader[AppUser.Columns.FirstName] ?? "");
                                jsonWriter.WritePropertyName(@"last_name");
                                jsonWriter.WriteValue(reader[AppUser.Columns.LastName] ?? "");
                                jsonWriter.WritePropertyName(@"phone");
                                jsonWriter.WriteValue(reader[AppUser.Columns.Phone] ?? "");
                                jsonWriter.WritePropertyName(@"city_id");
                                jsonWriter.WriteValue(reader[AppUser.Columns.CityId] != null ? reader[AppUser.Columns.CityId].ToString() : "0");
                                jsonWriter.WritePropertyName(@"street");
                                jsonWriter.WriteValue(reader[AppUser.Columns.Street] ?? "");
                                jsonWriter.WritePropertyName(@"house_num");
                                jsonWriter.WriteValue(reader[AppUser.Columns.HouseNum] ?? "");
                                jsonWriter.WritePropertyName(@"floor");
                                jsonWriter.WriteValue(reader[AppUser.Columns.Floor] ?? "");
                                jsonWriter.WritePropertyName(@"apartment_number");
                                jsonWriter.WriteValue(reader[AppUser.Columns.ApartmentNumber] ?? "");
                                jsonWriter.WritePropertyName(@"is_adv");
                                jsonWriter.WriteValue(reader[AppUser.Columns.IsAdv] ?? "");
                            }
                        }
                    }
                }
            }
        }

        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            JObject inputData = null;
            try
            {
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        inputData = JObject.Load(jsonReader);
                    }
                }
            }
            catch
            {
                RespondBadRequest(Response);
            }

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                JToken jt;

                string first_name = null, last_name = null, email = null, gender = null, is_adv = null, password = null, city_id = null, house_num = null, apartment_number = null,
                    street = null, phone = null, lang_code = null, floor = null;
                if (inputData.TryGetValue(@"first_name", out jt)) first_name = jt.Value<string>();
                if (inputData.TryGetValue(@"last_name", out jt)) last_name = jt.Value<string>();
                if (inputData.TryGetValue(@"email", out jt)) email = jt.Value<string>();
                if (inputData.TryGetValue(@"gender", out jt)) gender = jt.Value<string>();
                if (inputData.TryGetValue(@"city_id", out jt)) city_id = jt.Value<string>();
                if (inputData.TryGetValue(@"street", out jt)) street = jt.Value<string>();
                if (inputData.TryGetValue(@"house_num", out jt)) house_num = jt.Value<string>();
                if (inputData.TryGetValue(@"apartment_number", out jt)) apartment_number = jt.Value<string>();
                if (inputData.TryGetValue(@"floor", out jt)) floor = jt.Value<string>();
                if (inputData.TryGetValue(@"password", out jt)) password = jt.Value<string>();
                if (inputData.TryGetValue(@"phone", out jt)) phone = jt.Value<string>();
                if (inputData.TryGetValue(@"lang_code", out jt)) lang_code = jt.Value<string>();
                if (inputData.TryGetValue(@"is_adv", out jt)) is_adv = jt.Value<string>();

                string currentEmail = Query.New<AppUser>().Select(AppUser.Columns.Email).Where(AppUser.Columns.AppUserId, AppUserId).ExecuteScalar() as string;
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        if (email != null && !email.IsValidEmail())
                        {
                            RespondBadRequest(Response, @"invalid-email", @"the supplied email address is invalid");
                        }
                        if (password != null && password != "")
                        {
                            AppMembership.AppUserPasswordChangeResults result = AppMembership.ChangeAppUserPassword(currentEmail, password);
                            switch (result)
                            {
                                default:
                                case AppMembership.AppUserPasswordChangeResults.AppUserDoesNotExist:
                                    RespondInternalServerError(Response);
                                    break;
                                case AppMembership.AppUserPasswordChangeResults.PasswordDoNotMatch:
                                    RespondBadRequest(Response, @"old-password-does-not-match", @"the supplied password does not match the old password");
                                    break;
                                case AppMembership.AppUserPasswordChangeResults.Success:
                                    break;
                            }
                        }

                        Query qry = Query.New<AppUser>().Where(AppUser.Columns.AppUserId, AppUserId);

                        if (first_name != null)
                        {
                            qry.Update(AppUser.Columns.FirstName, first_name);
                        }
                        if (last_name != null)
                        {
                            qry.Update(AppUser.Columns.LastName, last_name);
                        }
                        if (gender != null)
                        {
                            switch (gender)
                            {
                                default:
                                case @"unknown":
                                    qry.Update(AppUser.Columns.Gender, AppUserGender.Unknown);
                                    break;
                                case @"male":
                                    qry.Update(AppUser.Columns.Gender, AppUserGender.Male);
                                    break;
                                case @"female":
                                    qry.Update(AppUser.Columns.Gender, AppUserGender.Female);
                                    break;
                            }
                        }
                        if (city_id != null)
                        {
                            qry.Update(AppUser.Columns.CityId, int.Parse(city_id));
                        }
                        if (phone != null)
                        {
                            qry.Update(AppUser.Columns.Phone, phone);
                        }
                        if (lang_code != null)
                        {
                            qry.Update(AppUser.Columns.LangCode, lang_code);
                        }
                        if (floor != null)
                        {
                            qry.Update(AppUser.Columns.Floor, floor);
                        }
                        if (street != null)
                        {
                            qry.Update(AppUser.Columns.Street, street);
                        }

                        if (house_num != null)
                        {
                            qry.Update(AppUser.Columns.HouseNum, house_num);
                        }

                        if (apartment_number != null)
                        {
                            qry.Update(AppUser.Columns.ApartmentNumber, apartment_number);
                        }

                        if (is_adv != null)
                        {
                            qry.Update(AppUser.Columns.IsAdv, bool.Parse(is_adv));
                        }

                        if (city_id != null)
                        {
                            try
                            {
                                City c = City.FetchByID(int.Parse(city_id));
                                var locationService = new GoogleLocationService();
                                var point = locationService.GetLatLongFromAddress(c.CityName);
                                qry.Update(AppUser.Columns.AddressLocation, new Geometry.Point(point.Latitude, point.Longitude));
                            }
                            catch (Exception)
                            {
                                qry.Update(AppUser.Columns.AddressLocation, new Geometry.Point(0, 0));
                            }

                        }

                        if (qry.QueryMode == QueryMode.Update)
                        {
                            qry.Execute();
                        }

                        if (email != null)
                        {
                            if (email != currentEmail)
                            {
                                if (email.NormalizeEmail() == currentEmail.NormalizeEmail())
                                {
                                    Query.New<AppUser>()
                                        .Update(AppUser.Columns.Email, email)
                                        .Where(AppUser.Columns.AppUserId, AppUserId)
                                        .AND(AppUser.Columns.UniqueIdString, email.NormalizeEmail()).Execute();
                                }
                                else
                                {
                                    Query.New<AppUser>()
                                        .Update(AppUser.Columns.UniqueIdString, email.NormalizeEmail())
                                        .Update(AppUser.Columns.Email, email)
                                        .Where(AppUser.Columns.AppUserId, AppUserId).Execute();
                                }
                            }
                        }

                        jsonWriter.WriteStartObject();

                        Int64? cityId = AppUser.FetchByID(AppUserId).CityId;
                        jsonWriter.WritePropertyName(@"is_city_exists");
                        jsonWriter.WriteValue(cityId != null && cityId != 0);

                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }
    }
}
