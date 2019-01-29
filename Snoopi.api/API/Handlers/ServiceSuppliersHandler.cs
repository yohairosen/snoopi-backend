using dg.Sql;
using GoogleMaps.LocationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Snoopi.api
{
    public class ServiceSuppliersHandler : ApiHandlerBase
    {

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


        }
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                //Int64 bid_id = (Request.QueryString["bid_id"] != null ? Int64.Parse(Request.QueryString["bid_id"].ToString()) : 0 );            

                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        Int64 AppUserId;
                        Int64 tempAppUserId = 0;
                        IsAuthorizedRequest(Request, Response, false, out AppUserId);
                        AppUser user = null;
                        if (AppUserId != 0)
                        {
                            user = AppUser.FetchByID(AppUserId);
                            bool _locked =  user!= null ? user.IsLocked : true;
                            if (_locked)
                            {
                                RespondError(Response, HttpStatusCode.BadRequest, @"appuser-locked");
                                return;
                            }
                        }
                        TempAppUser tempUser = null;
                        tempAppUserId = Request.QueryString["temp_app_user_id"] != null ? Int64.Parse(Request.QueryString["temp_app_user_id"].ToString()) : 0;
                         if (tempAppUserId > 0)                        
                             tempUser = TempAppUser.FetchByID(tempAppUserId);
                         
                        int service_id = 0;
                        service_id = Request.QueryString["service_id"] != null ? int.Parse(Request.QueryString["service_id"].ToString()) : 0;

                        long cityId;
                        cityId = Request.QueryString["city_id"] != null ? Int64.Parse(Request.QueryString["city_id"].ToString()) : 0;
                        if (cityId <=0)
                            cityId = user != null ? user.CityId : tempUser.CityId;

                        string cityName = Request.QueryString["city_name"];                      
                       
                        var promotedSuppliers = SupplierPromotedController.GetSuppliersPromotedOfCity(cityId, service_id, 5);
                
                        Geometry.Point location = null;
                        if (!String.IsNullOrWhiteSpace(cityName))
                        {
                            var locationService = new GoogleLocationService();
                            var point = locationService.GetLatLongFromAddress(cityName);
                            location = (point != null ? new dg.Sql.Geometry.Point(point.Latitude, point.Longitude) : new dg.Sql.Geometry.Point(0, 0));
                        }

                        if (location == null)
                        {
                            if (user != null)
                                location = user.AddressLocation;
                            else
                                location = tempUser.Location;
                        }
                        var regularSuppliers = ServiceController.GetServiceSuppliersByDistance(service_id,cityId, location, promotedSuppliers);
                        
                        int index = 1;

                        jsonWriter.WriteStartObject();
                         jsonWriter.WritePropertyName(@"promoted_suppliers");
                        jsonWriter.WriteStartArray();
                        foreach (var item in promotedSuppliers)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"supplier_id");
                            jsonWriter.WriteValue(item.SupplierId);
                            jsonWriter.WritePropertyName(@"supplier_name");
                            jsonWriter.WriteValue(item.BusinessName ?? "");
                            jsonWriter.WritePropertyName(@"phone");
                            jsonWriter.WriteValue(item.Phone ?? "");
                            jsonWriter.WritePropertyName(@"city");
                            jsonWriter.WriteValue(item.CityName ?? "");
                            jsonWriter.WritePropertyName(@"street");
                            jsonWriter.WriteValue(item.Street ?? "");
                            jsonWriter.WritePropertyName(@"house_num");
                            jsonWriter.WriteValue(item.HouseNum ?? "");
                            jsonWriter.WritePropertyName(@"avg_rate");
                            jsonWriter.WriteValue(item.AvgRate);
                            jsonWriter.WritePropertyName(@"index");
                            jsonWriter.WriteValue(index++);
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(item.Description);
                            jsonWriter.WritePropertyName(@"image");
                            jsonWriter.WriteValue(item.ProfileImage);
                            jsonWriter.WritePropertyName(@"comments_number");
                            jsonWriter.WriteValue(item.NumberOfComments);
                            jsonWriter.WriteEndObject();
                        }
                        jsonWriter.WriteEndArray();

                        jsonWriter.WritePropertyName(@"suppliers");
                        jsonWriter.WriteStartArray();
                        foreach (var item in regularSuppliers)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"supplier_id");
                            jsonWriter.WriteValue(item.SupplierId);
                            jsonWriter.WritePropertyName(@"supplier_name");
                            jsonWriter.WriteValue(item.BusinessName ?? "");
                            jsonWriter.WritePropertyName(@"phone");
                            jsonWriter.WriteValue(item.Phone ?? "");
                            jsonWriter.WritePropertyName(@"city");
                            jsonWriter.WriteValue(item.CityName ?? "");
                            jsonWriter.WritePropertyName(@"street");
                            jsonWriter.WriteValue(item.Street ?? "");
                            jsonWriter.WritePropertyName(@"house_num");
                            jsonWriter.WriteValue(item.HouseNum ?? "");
                            jsonWriter.WritePropertyName(@"avg_rate");
                            jsonWriter.WriteValue(item.AvgRate);
                            jsonWriter.WritePropertyName(@"index");
                            jsonWriter.WriteValue(index++);
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(item.Description);
                            jsonWriter.WritePropertyName(@"image");
                            jsonWriter.WriteValue(item.ProfileImage);
                            jsonWriter.WritePropertyName(@"comments_number");
                            jsonWriter.WriteValue(item.NumberOfComments);
                            jsonWriter.WriteEndObject();
                        }
                        jsonWriter.WriteEndArray();
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            catch (Exception) { }

        }
    }








}
