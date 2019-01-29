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
    public class OffersServiceHandler : ApiHandlerBase
    {

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
                         Int64 bid_id = 0;
                         IsAuthorizedRequest(Request, Response, false, out AppUserId);
                         Int64 TempAppUserId = (Request.QueryString["temp_app_user_id"] != null ? Convert.ToInt64(Request.QueryString["temp_app_user_id"]) : 0);
                         Geometry.Point point = new Geometry.Point();
                         if (TempAppUserId != 0) { 
                             TempAppUser temp = TempAppUser.FetchByID(TempAppUserId);
                             point = (temp != null ? temp.Location : new Geometry.Point(0,0));
                             //Bid b = Bid.FetchByTempAppUserId(TempAppUserId);
                             BidService bService = BidService.FetchByTempAppUserId(TempAppUserId);
                             if (bService != null)
                             {
                                 bid_id = bService.BidId;
                             }
                         }
                         else if (AppUserId != 0) { 
                             AppUser user = AppUser.FetchByID(AppUserId);
                             point = (user != null ? user.AddressLocation : new Geometry.Point(0,0));
                             //Bid b = Bid.FetchByAppUserId(AppUserId);
                             BidService bService = BidService.FetchByAppUserId(AppUserId);                            
                             if (bService != null)
                             {
                                 bid_id = bService.BidId;
                             }
                         }                                           

                         BidService bidService = BidService.FetchByID(bid_id);

                         List<OfferServiceUI> lstOfferUI = bid_id != 0 ? OfferServiceController.GetAllOfferByBidId(bid_id, bidService.EndDate ,point) :  new List<OfferServiceUI>();
                         jsonWriter.WriteStartObject();

                         jsonWriter.WritePropertyName(@"offers");
                         jsonWriter.WriteStartArray();

                         foreach (OfferServiceUI item in lstOfferUI)
                         {
                             jsonWriter.WriteStartObject();

                             jsonWriter.WritePropertyName(@"offer_id");
                             jsonWriter.WriteValue(item.OfferId);
                             jsonWriter.WritePropertyName(@"supplier_id");
                             jsonWriter.WriteValue(item.SupplierId);
                             jsonWriter.WritePropertyName(@"supplier_name");
                             jsonWriter.WriteValue(item.SupplierName);
                             jsonWriter.WritePropertyName(@"price");
                             jsonWriter.WriteValue(item.Price);
                             jsonWriter.WritePropertyName(@"address");
                             jsonWriter.WriteValue(item.Address);
                             jsonWriter.WritePropertyName(@"phone");
                             jsonWriter.WriteValue(item.Phone);

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
