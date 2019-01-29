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
    public class DonationHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                
             Int64 AppUserId;
             if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
             {

                 Response.ContentType = @"application/json";
                 using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                 {
                     using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                     {

                         Donation donation = Donation.GetLastDonation();
                         jsonWriter.WriteStartObject();

                         jsonWriter.WritePropertyName(@"donation_id");
                         jsonWriter.WriteValue(donation.DonationId);
                         jsonWriter.WritePropertyName(@"donation_item");
                        jsonWriter.WriteValue(donation.DonationItem);
                        jsonWriter.WritePropertyName(@"donation_name");
                        jsonWriter.WriteValue(donation.DonationName);
                        jsonWriter.WritePropertyName(@"donation_price");
                        jsonWriter.WriteValue(donation.DonationPrice);

                        jsonWriter.WriteEndObject();
                     }
                 }
             }
            }
            catch (Exception) { }



        }

    }
}
