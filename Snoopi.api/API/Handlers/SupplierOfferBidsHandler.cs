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
    public class SupplierOfferBidsHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                
             Int64 SupplierId;
             if (IsAuthorizedRequestSupplier(Request, Response, true, out SupplierId))
             {

                 Response.ContentType = @"application/json";
                 using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                 {
                     using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                     {

                         List<MainOffer> lstMainOffer = SupplierController.GetAllOfferBid(SupplierId);
                         jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"bids");
                        jsonWriter.WriteStartArray();

                        foreach (MainOffer item in lstMainOffer)
                        {
                            jsonWriter.WriteStartObject();


                            jsonWriter.WritePropertyName(@"bid_id");
                            jsonWriter.WriteValue(item.BidId);
                            jsonWriter.WritePropertyName(@"end_time");
                            jsonWriter.WriteValue(item.EndBid);
                            jsonWriter.WritePropertyName(@"is_service");
                            jsonWriter.WriteValue(item.IsService);
                            jsonWriter.WritePropertyName(@"city");
                            jsonWriter.WriteValue(item.City);
                            jsonWriter.WritePropertyName(@"offer_id");
                            jsonWriter.WriteValue(item.OfferId);
                            jsonWriter.WritePropertyName(@"total_price");
                            jsonWriter.WriteValue(item.TotalPrice);
                            jsonWriter.WritePropertyName(@"num_of_payments");
                            jsonWriter.WriteValue(item.NumOfPayments);
                            jsonWriter.WritePropertyName(@"gift");
                            jsonWriter.WriteValue(item.Gift);
                            jsonWriter.WritePropertyName(@"customer_comment");
                            jsonWriter.WriteValue(item.CustomerComment);
                            jsonWriter.WritePropertyName(@"status");
                            jsonWriter.WriteValue(item.Status);
                            jsonWriter.WriteEndObject();
                        }

                        jsonWriter.WriteEndArray();

                         jsonWriter.WriteEndObject();
                     }
                 }
             }
            }
            catch (Exception ex) 
                {
            }



        }

    }
}
