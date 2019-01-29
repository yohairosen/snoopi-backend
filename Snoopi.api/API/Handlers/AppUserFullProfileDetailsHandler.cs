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
using System;

namespace Snoopi.api
{
    class AppUserFullProfileDetailsHandler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            System.Int64 AppUserId;
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
                        Boolean is_missing = true;
                        using (DataReaderBase reader = qry.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                is_missing = ((reader[AppUser.Columns.Email] != null ? (reader[AppUser.Columns.Email].ToString() == "" ? true : false) : true) ||
                                             (reader[AppUser.Columns.FirstName] != null ? (reader[AppUser.Columns.FirstName].ToString() == "" ? true : false) : true) ||
                                             (reader[AppUser.Columns.LastName] != null ? (reader[AppUser.Columns.LastName].ToString() == "" ? true : false) : true) ||
                                             (reader[AppUser.Columns.Phone] != null ? (reader[AppUser.Columns.Phone].ToString() == "" ? true : false) : true) ||
                                             (reader[AppUser.Columns.CityId] != null ? (reader[AppUser.Columns.CityId].ToString() == "" ? true : (reader[AppUser.Columns.CityId].ToString() == "0" ? true : false)) : true) ||
                                             (reader[AppUser.Columns.Street] != null ? (reader[AppUser.Columns.Street].ToString() == "" ? true : false) : true) ||
                                             (reader[AppUser.Columns.HouseNum] != null ? (reader[AppUser.Columns.HouseNum].ToString() == "" ? true : false) : true)) == true ? true : false;
                            }
                            jsonWriter.WritePropertyName(@"is_missing");
                            jsonWriter.WriteValue(is_missing);
                        }
                    }
                }
            }
        }

    }
}
