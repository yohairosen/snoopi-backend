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
using System.Text.RegularExpressions;

namespace Snoopi.api
{
    public class ServiceHandler : ApiHandlerBase
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



            Response.ContentType = @"application/json";

            JToken jt;

            Int64 AppUserId;
            IsAuthorizedRequest(Request, Response, false, out AppUserId);
            if (AppUserId != 0)
            {
                
                bool _locked = AppUser.FetchByID(AppUserId) != null ? AppUser.FetchByID(AppUserId).IsLocked : true;
                if (_locked)
                {
                    RespondError(Response, HttpStatusCode.BadRequest, @"appuser-locked");
                    return;
                }
            }

            Int64 TempAppUserId = 0;
            if (inputData.TryGetValue(@"temp_app_user_id", out jt)) TempAppUserId = jt.Value<Int64>();

            Int64 service_id = 0;
            if (inputData.TryGetValue(@"service_id", out jt)) service_id = jt.Value<Int64>();

            string customer_comment = "";
            if (inputData.TryGetValue(@"customer_comment", out jt)) customer_comment = Regex.Replace(jt.Value<string>(), @"\p{Cs}", ""); ;
            Int64 BidId = ServiceController.CreateBidService(AppUserId, TempAppUserId, service_id, customer_comment);

            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    if (BidId == 0)
                    {
                        RespondError(Response, HttpStatusCode.BadRequest, "only-one-bid");
                    }
                    else
                    {
                        jsonWriter.WriteStartObject();
                        jsonWriter.WritePropertyName("bid_id");
                        jsonWriter.WriteValue(BidId);
                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }    

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        Int64 OfferId = Request.QueryString["offer_id"] != null ? Convert.ToInt64(Request.QueryString["offer_id"]) : 0 ;
                        OfferServiceUI offerServiceUI = OfferServiceController.GetOfferByOfferId(OfferId);
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"offer_id");
                        jsonWriter.WriteValue(offerServiceUI.OfferId);
                        jsonWriter.WritePropertyName(@"supplier_id");
                        jsonWriter.WriteValue(offerServiceUI.SupplierId);
                        jsonWriter.WritePropertyName(@"supplier_name");
                        jsonWriter.WriteValue(offerServiceUI.SupplierName ?? "");
                        jsonWriter.WritePropertyName(@"price");
                        jsonWriter.WriteValue(offerServiceUI.Price ?? "");
                        jsonWriter.WritePropertyName(@"address");
                        jsonWriter.WriteValue(offerServiceUI.Address ?? "");
                        jsonWriter.WritePropertyName(@"phone");
                        jsonWriter.WriteValue(offerServiceUI.Phone ?? "");
                        jsonWriter.WritePropertyName(@"supplier_remarks");
                        jsonWriter.WriteValue(offerServiceUI.Remarks ?? "");

                        jsonWriter.WriteEndObject();
                    }
                }
            }
            catch (Exception) { }



        }

    }
}
