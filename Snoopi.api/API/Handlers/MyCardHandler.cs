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
    public class MyCardHandler : ApiHandlerBase
    {

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
                        AppUserCard appUserCard = AppUserCard.FetchByAppUserId(AppUserId);
                        if (appUserCard != null)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"last_4digit");
                            jsonWriter.WriteValue(appUserCard.Last4Digit ?? "");
                            jsonWriter.WritePropertyName(@"card_token");
                            jsonWriter.WriteValue(appUserCard.CardToken ?? "");
                            jsonWriter.WritePropertyName(@"expiry_date");
                            jsonWriter.WriteValue(appUserCard.ExpiryDate ?? "");

                            jsonWriter.WriteEndObject();
                        }
                        else
                        {

                            RespondError(Response, HttpStatusCode.NotFound, "no-card-found");
                        }
                        

                    }
                }
            }
        }

    }
}
