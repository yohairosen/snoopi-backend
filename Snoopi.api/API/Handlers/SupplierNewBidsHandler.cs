using Newtonsoft.Json;
using Snoopi.core.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Snoopi.api
{
    public class SupplierNewBidsHandler : ApiHandlerBase
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

                         List<MainBid> lstMainBid = SupplierController.GetAllNewBid(SupplierId);
                         if (lstMainBid == null)
                             lstMainBid = new List<MainBid>();
                         jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"bids");
                        jsonWriter.WriteStartArray();
                        lstMainBid = lstMainBid.OrderBy(r => r.DateOrder).ToList();
                        foreach (MainBid item in lstMainBid)
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


                            jsonWriter.WriteEndObject();
                        }

                        jsonWriter.WriteEndArray();

                         jsonWriter.WriteEndObject();
                     }
                 }
             }
            }
            catch (Exception) { }



        }

    }
}
