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
    public class BidCloseHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {     
                Int64 AppUserId;
                Int64 offer_id = (Request.QueryString["offer_id"] != null ? Int64.Parse(Request.QueryString["offer_id"].ToString()) : 0 );
                if (IsAuthorizedRequest(Request, Response,true, out AppUserId))
                {
                    Response.ContentType = @"application/json";
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {

                            Dictionary<string, string> result = BidController.GetDiscount(offer_id, AppUserId);
                            
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"total_price");
                            jsonWriter.WriteValue(result["TotalPrice"]);
                            jsonWriter.WritePropertyName(@"price_after_discount");
                            jsonWriter.WriteValue(result["PriceAfterDiscount"]);
                            jsonWriter.WritePropertyName(@"gift_content");
                            jsonWriter.WriteValue(result["GiftContent"]);
                            jsonWriter.WritePropertyName(@"campaign_id");
                            jsonWriter.WriteValue(result["CampaignId"]);

                            jsonWriter.WriteEndObject();
                        }
                    }
                }
            }
            catch (Exception) { }



        }

    }
}
