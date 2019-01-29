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
    public class BidCheckSupplierWinningsHandler:ApiHandlerBase
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

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                try
                {
                    JToken jt;
                    Int64 bid_id = 0, offer_id = 0;
                    if (inputData.TryGetValue(@"offer_id", out jt)) offer_id = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"bid_id", out jt)) bid_id = jt.Value<Int64>();  
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            int maxBysupplier = 0;
                            Offer _offer = Offer.FetchByID(offer_id);
                            if (_offer != null)
                                maxBysupplier = AppSupplier.FetchByID(_offer.SupplierId).MaxWinningsNum;

                            if (maxBysupplier == 0)
                            {
                               // RespondError(Response, HttpStatusCode.InternalServerError, @"supplier-maxwinningsnum-zero");
                              //  return;
                                throw new InvalidDataException(@"supplier-maxwinningsnum-zero");
                            }
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"max_winnings_num");
                            jsonWriter.WriteValue(maxBysupplier);
                            jsonWriter.WritePropertyName(@"supplier_id");
                            jsonWriter.WriteValue(_offer.SupplierId);
                            jsonWriter.WriteEndObject();
                        }
                    }
                }
                catch (InvalidDataException e)
                {

                    RespondError(Response, HttpStatusCode.InternalServerError, @"supplier-maxwinningsnum-zero");

                }

                catch (Exception e)
                {
                    RespondError(Response, HttpStatusCode.InternalServerError, @"db-error");
                }

            }


        }
    }
}
