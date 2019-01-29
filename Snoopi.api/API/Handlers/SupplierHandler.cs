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
    public class SupplierHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                
             //Int64 AppUserId;
             //if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
             //{
                 Int64 SupplierId = Request.QueryString["supplier_id"] != null ? Int64.Parse(Request.QueryString["supplier_id"].ToString()) : 0;
                 Response.ContentType = @"application/json";
                 using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                 {
                     using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                     {

                         SupplierUI supplierUI = SupplierController.GetSupplierForAppById(SupplierId);
                         jsonWriter.WriteStartObject();
;
                        jsonWriter.WritePropertyName(@"supplier_id");
                        jsonWriter.WriteValue(supplierUI.SupplierId);
                        jsonWriter.WritePropertyName(@"avg_rate");
                        jsonWriter.WriteValue(supplierUI.AvgRate);
                        jsonWriter.WritePropertyName(@"supplier_name");
                        jsonWriter.WriteValue(supplierUI.BusinessName);
                        if (supplierUI.IsService)
                        {
                            jsonWriter.WritePropertyName(@"phone");
                            jsonWriter.WriteValue(supplierUI.Phone);
                        }
                        jsonWriter.WritePropertyName(@"city");
                        jsonWriter.WriteValue(supplierUI.CityName);
                        jsonWriter.WritePropertyName(@"street");
                        jsonWriter.WriteValue(supplierUI.Street);
                        jsonWriter.WritePropertyName(@"house_num");
                        jsonWriter.WriteValue(supplierUI.HouseNum);
                        jsonWriter.WritePropertyName(@"profile_image");
                        jsonWriter.WriteValue(supplierUI.ProfileImage);
                        jsonWriter.WritePropertyName(@"description");
                        jsonWriter.WriteValue(supplierUI.Description);
                        jsonWriter.WritePropertyName(@"discount");
                        jsonWriter.WriteValue(supplierUI.Discount);
                        
                        jsonWriter.WritePropertyName(@"comments");
                        jsonWriter.WriteStartArray();

                        foreach (Comment item in supplierUI.LstComment)
                        {
                            jsonWriter.WriteStartObject();


                            jsonWriter.WritePropertyName(@"content");
                            jsonWriter.WriteValue(item.Content);
                            jsonWriter.WritePropertyName(@"sender_name");
                            jsonWriter.WriteValue(item.Name);
                            jsonWriter.WritePropertyName(@"rate");
                            jsonWriter.WriteValue(item.Rate);

                            jsonWriter.WriteEndObject();
                        }

                        jsonWriter.WriteEndArray();

                         jsonWriter.WriteEndObject();
                     }
                 }
             }
            //}
            catch (Exception) { }



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

            string email = inputData.Value<string>(@"email") ?? "";
            string password = inputData.Value<string>(@"password") ?? "";

            Response.ContentType = @"application/json";
            Int64 SupplierId;
            if (IsAuthorizedRequestSupplier(Request, Response,true, out SupplierId))
            {
                JToken jt;
                bool? status = null, allow_change_status_join_bids = null, is_auto_join_bid = null;
                int? max_winning_num = null;
                if (inputData.TryGetValue(@"status", out jt)) status = jt.Value<bool>();
                if (inputData.TryGetValue(@"allow_change_status_join_bids", out jt)) allow_change_status_join_bids = jt.Value<bool>();
                if (inputData.TryGetValue(@"is_auto_join_bid", out jt)) is_auto_join_bid = jt.Value<bool>();
                if (inputData.TryGetValue(@"max_winning_num", out jt)) max_winning_num = jt.Value<int>();

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {

                        Query q = new Query(AppSupplier.TableSchema).Where(AppSupplier.Columns.SupplierId, SupplierId);
                        if (status != null) q.Update(AppSupplier.Columns.Status, status);
                        if (allow_change_status_join_bids != null) q.Update(AppSupplier.Columns.AllowChangeStatusJoinBid, allow_change_status_join_bids);
                        if (is_auto_join_bid != null) q.Update(AppSupplier.Columns.StatusJoinBid, is_auto_join_bid);
                        if (max_winning_num != null) q.Update(AppSupplier.Columns.MaxWinningsNum, max_winning_num);

                        q.Execute();

                        jsonWriter.WriteStartObject();
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
